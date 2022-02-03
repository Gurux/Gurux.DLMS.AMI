﻿//
// --------------------------------------------------------------------------
//  Gurux Ltd
//
//
//
// Filename:        $HeadURL$
//
// Version:         $Revision$,
//                  $Date$
//                  $Author$
//
// Copyright (c) Gurux Ltd
//
//---------------------------------------------------------------------------
//
//  DESCRIPTION
//
// This file is a part of Gurux Device Framework.
//
// Gurux Device Framework is Open Source software; you can redistribute it
// and/or modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; version 2 of the License.
// Gurux Device Framework is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU General Public License for more details.
//
// This code is licensed under the GNU General Public License v2.
// Full text may be retrieved at http://www.gnu.org/licenses/gpl-2.0.txt
//---------------------------------------------------------------------------
using Gurux.Common;
using Gurux.DLMS.Enums;
using Gurux.DLMS.Objects;
using Gurux.DLMS.Secure;
using Gurux.Net;
using Gurux.DLMS.AMI.Messages.DB;
using Gurux.DLMS.AMI.Messages.Enums;
using Gurux.DLMS.AMI.Messages.Rest;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Threading;
using Gurux.Serial;
using Microsoft.Extensions.Options;
using System.Reflection;
using Gurux.Terminal;
using Gurux.DLMS.AMI.Internal;

namespace Gurux.DLMS.AMI.Reader
{
    internal class ReaderService : IHostedService, IDisposable
    {
        private readonly CancellationToken _cancellationToken;
        private DateTime lastUpdated = DateTime.MinValue;
        private readonly ILogger _logger;
        private readonly System.Net.Http.HttpClient client = Helpers.client;
        private readonly Guid _guid;
        private readonly int _threads;
        private readonly string _name;
        private readonly int _waitTime;
        private readonly int _aliveTime;
        private readonly TraceLevel _traceLevel;


        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="logger"></param>
        public ReaderService(ILogger<ReaderService> logger, IOptions<ReaderOptions> optionsAccessor, IHostApplicationLifetime applicationLifetime)
        {
            _cancellationToken = applicationLifetime.ApplicationStopping;
            _logger = logger;
            _guid = Guid.Parse(optionsAccessor.Value.Id);
            _threads = optionsAccessor.Value.Threads;
            _name = optionsAccessor.Value.Name;
            _waitTime = optionsAccessor.Value.TaskWaitTime * 1000;
            _aliveTime = optionsAccessor.Value.AliveTime;
            _traceLevel = optionsAccessor.Value.TraceLevel;
        }

        /// <summary>
        /// Send notify that reader is alive.
        /// </summary>
        /// <param name="value"></param>
        private async void ListenerAlive(GXReaderInfo value)
        {
            //Give some time DB server to start up.
            Thread.Sleep(1000);
            System.Net.Http.HttpResponseMessage response;
            while (!_cancellationToken.IsCancellationRequested)
            {
                using (response = await client.PostAsJsonAsync(Startup.ServerAddress + "/api/reader/AddReader", new AddReader() { Reader = value }))
                {
                    Helpers.CheckStatus(response);
                }
                if (_aliveTime == 0)
                {
                    break;
                }
                Thread.Sleep(_aliveTime * 60000);
            }
        }
        public System.Threading.Tasks.Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Reader Service is starting.");
            GXReaderInfo r = new GXReaderInfo();
            r.Guid = _guid;
            r.Name = _name;
            FileVersionInfo info = FileVersionInfo.GetVersionInfo(typeof(ReaderService).Assembly.Location);
            r.Version = info.FileVersion;
            Thread t = new Thread(() => ListenerAlive(r));
            t.Start();
            for (int pos = 0; pos != _threads; ++pos)
            {
                t = new Thread(() => DoWork());
                t.Start();
            }
            return System.Threading.Tasks.Task.CompletedTask;
        }

        private async void DoWork()
        {
            //Give some time DB server to start up.
            Thread.Sleep(1000);
            GetNextTaskResponse ret = null;
            System.Net.Http.HttpResponseMessage response;
            _logger.LogInformation("Reader Service is started.");
            while (!_cancellationToken.IsCancellationRequested)
            {
                try
                {
                    using (response = await client.PostAsJsonAsync(Startup.ServerAddress + "/api/task/GetNextTask", new GetNextTask()))
                    {
                        Helpers.CheckStatus(response);
                        ret = await response.Content.ReadAsAsync<GetNextTaskResponse>();
                    }
                    if (ret.Tasks != null)
                    {
                        int pos = 0;
                        GXDevice dev;
                        GXDLMSSecureClient cl;
                        using (response = await client.PostAsJsonAsync(Startup.ServerAddress + "/api/device/ListDevices", new ListDevices() { Ids = new[] { ret.Tasks[0].Object.DeviceId } }))
                        {
                            Helpers.CheckStatus(response);
                            ListDevicesResponse r = await response.Content.ReadAsAsync<ListDevicesResponse>();
                            if (r.Devices == null || r.Devices.Length == 0)
                            {
                                continue;
                            }
                            dev = r.Devices[0];
                        }
                        IGXMedia media;
                        if (string.Compare(dev.MediaType, typeof(GXNet).FullName, true) == 0)
                        {
                            media = new GXNet();
                        }
                        else if (string.Compare(dev.MediaType, typeof(GXSerial).FullName, true) == 0)
                        {
                            media = new GXSerial();
                        }
                        else if (string.Compare(dev.MediaType, typeof(GXTerminal).FullName, true) == 0)
                        {
                            media = new GXTerminal();
                        }
                        else
                        {
                            Type type = Type.GetType(dev.MediaType);
                            if (type == null)
                            {
                                string ns = "";
                                pos = dev.MediaType.LastIndexOf('.');
                                if (pos != -1)
                                {
                                    ns = dev.MediaType.Substring(0, pos);
                                }
                                foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                                {
                                    if (assembly.GetName().Name == ns)
                                    {
                                        if (assembly.GetType(dev.MediaType, false, true) != null)
                                        {
                                            type = assembly.GetType(dev.MediaType);
                                        }
                                    }
                                }
                            }
                            if (type == null)
                            {
                                throw new Exception("Invalid media type: " + dev.MediaType);
                            }
                            media = (IGXMedia)Activator.CreateInstance(type);
                        }
                        if (media == null)
                        {
                            throw new Exception("Unknown media type '" + dev.MediaType + "'.");
                        }
                        media.Settings = dev.MediaSettings;
                        int deviceAddress;
                        if (dev.HDLCAddressing == ManufacturerSettings.HDLCAddressType.SerialNumber)
                        {
                            deviceAddress = GXDLMSClient.GetServerAddressFromSerialNumber(dev.PhysicalAddress, dev.LogicalAddress);
                        }
                        else
                        {
                            if (dev.LogicalAddress != 0 &&
                                (dev.InterfaceType == InterfaceType.HDLC || dev.InterfaceType == InterfaceType.HdlcWithModeE))
                            {
                                deviceAddress = GXDLMSClient.GetServerAddress(dev.LogicalAddress, dev.PhysicalAddress);
                            }
                            else
                            {
                                deviceAddress = dev.PhysicalAddress;
                            }
                        }
                        TraceLevel trace = _traceLevel;
                        if (dev.TraceLevel > trace)
                        {
                            trace = dev.TraceLevel;
                        }
                        UInt64 devId = 0;
                        if (dev.TraceLevel != TraceLevel.Off)
                        {
                            devId = dev.Id;
                        }
                        GXDLMSReader reader;
                        //Read frame counter from the meter.
                        if (dev.Security != 0)
                        {
                            cl = new GXDLMSSecureClient(dev.UseLogicalNameReferencing, 16, deviceAddress,
                                Authentication.None, null, (InterfaceType)dev.InterfaceType);
                            reader = new GXDLMSReader(cl, media, _logger, trace, dev.WaitTime, dev.ResendCount, devId);
                            media.Open();
                            reader.InitializeConnection();
                            //Read Innovation counter.
                            GXDLMSData d = new GXDLMSData(dev.FrameCounter);
                            reader.Read(d, 2);
                            dev.InvocationCounter = 1 + Convert.ToUInt32(d.Value);
                            reader.Disconnect();
                            media.Close();
                        }

                        cl = new GXDLMSSecureClient(dev.UseLogicalNameReferencing, dev.ClientAddress, deviceAddress,
                            (Authentication)dev.Authentication, dev.Password, (InterfaceType)dev.InterfaceType);
                        if (dev.HexPassword != null && dev.HexPassword.Length != 0)
                        {
                            cl.Password = dev.HexPassword;
                        }
                        cl.UseUtc2NormalTime = dev.UtcTimeZone;
                        cl.Standard = dev.Standard;
                        cl.Ciphering.SystemTitle = GXCommon.HexToBytes(dev.ClientSystemTitle);
                        if (cl.Ciphering.SystemTitle != null && cl.Ciphering.SystemTitle.Length == 0)
                        {
                            cl.Ciphering.SystemTitle = null;
                        }
                        cl.Ciphering.BlockCipherKey = GXCommon.HexToBytes(dev.BlockCipherKey);
                        if (cl.Ciphering.BlockCipherKey != null && cl.Ciphering.BlockCipherKey.Length == 0)
                        {
                            cl.Ciphering.BlockCipherKey = null;
                        }
                        cl.Ciphering.AuthenticationKey = GXCommon.HexToBytes(dev.AuthenticationKey);
                        if (cl.Ciphering.AuthenticationKey != null && cl.Ciphering.AuthenticationKey.Length == 0)
                        {
                            cl.Ciphering.AuthenticationKey = null;
                        }
                        cl.ServerSystemTitle = GXCommon.HexToBytes(dev.DeviceSystemTitle);
                        if (cl.ServerSystemTitle != null && cl.ServerSystemTitle.Length == 0)
                        {
                            cl.ServerSystemTitle = null;
                        }
                        cl.Ciphering.InvocationCounter = dev.InvocationCounter;
                        cl.Ciphering.Security = dev.Security;
                        reader = new GXDLMSReader(cl, media, _logger, trace, dev.WaitTime, dev.ResendCount, devId);
                        media.Open();
                        reader.InitializeConnection();
                        pos = 0;
                        int count = ret.Tasks.Length;
                        foreach (GXTask task in ret.Tasks)
                        {
                            ++pos;
                            try
                            {
                                GXDLMSObject obj = GXDLMSClient.CreateObject((ObjectType)task.Object.ObjectType);
                                obj.Version = task.Object.Version;
                                obj.LogicalName = task.Object.LogicalName;
                                obj.ShortName = task.Object.ShortName;
                                if (task.TaskType == TaskType.Write)
                                {
                                    if (obj is GXDLMSData && obj.LogicalName == "0.0.1.1.0.255" && task.Index == 2)
                                    {
                                        cl.UpdateValue(obj, task.Index, GXDateTime.ToUnixTime(DateTime.UtcNow));
                                    }
                                    else if (obj is GXDLMSClock && task.Index == 2)
                                    {
                                        cl.UpdateValue(obj, task.Index, DateTime.UtcNow);
                                    }
                                    else
                                    {
                                        cl.UpdateValue(obj, task.Index, GXDLMSTranslator.XmlToValue(task.Data));
                                    }
                                    reader.Write(obj, task.Index);
                                }
                                else if (task.TaskType == TaskType.Action)
                                {
                                    reader.Method(obj, task.Index, GXDLMSTranslator.XmlToValue(task.Data), DataType.None);
                                }
                                else if (task.TaskType == TaskType.Read)
                                {
                                    //Reading the meter.
                                    if (task.Object.Attributes[0].DataType != 0)
                                    {
                                        obj.SetDataType(task.Index, (DataType)task.Object.Attributes[0].DataType);
                                    }
                                    if (task.Object.Attributes[0].UIDataType != 0)
                                    {
                                        obj.SetUIDataType(task.Index, (DataType)task.Object.Attributes[0].UIDataType);
                                    }
                                    Reader.Read(_logger, client, reader, task, obj);
                                    if (task.Object.Attributes[0].DataType == 0)
                                    {
                                        task.Object.Attributes[0].DataType = (int)obj.GetDataType(task.Index);
                                        if (task.Object.Attributes[0].UIDataType == 0)
                                        {
                                            task.Object.Attributes[0].UIDataType = (int)obj.GetUIDataType(task.Index);
                                        }
                                        UpdateDatatype u = new UpdateDatatype() { Items = new GXAttribute[] { task.Object.Attributes[0] } };
                                        response = client.PostAsJsonAsync(Startup.ServerAddress + "/api/Object/UpdateDatatype", u).Result;
                                        Helpers.CheckStatus(response);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                task.Result = ex.Message;
                                AddError error = new AddError();
                                error.Error = new GXError()
                                {
                                    DeviceId = dev.Id,
                                    Error = "Failed to " + task.TaskType + " " + task.Object.LogicalName + ":" + task.Index + ". " + ex.Message
                                };
                                _logger.LogError(error.Error.Error);
                                using (response = await client.PostAsJsonAsync(Startup.ServerAddress + "/api/error/AddError", error))
                                {
                                    Helpers.CheckStatus(response);
                                    await response.Content.ReadAsAsync<AddErrorResponse>();
                                }
                            }
                            task.End = DateTime.Now;
                            //Close connection after last task is executed.
                            //This must done because there might be new task to execute.
                            if (count == pos)
                            {
                                try
                                {
                                    reader.Close();
                                }
                                catch (Exception ex)
                                {
                                    task.Result = ex.Message;
                                    AddError error = new AddError();
                                    error.Error = new GXError()
                                    {
                                        DeviceId = dev.Id,
                                        Error = "Failed to close the connection. " + ex.Message
                                    };
                                    _logger.LogError(error.Error.Error);
                                    using (response = await client.PostAsJsonAsync(Startup.ServerAddress + "/api/error/AddError", error))
                                    {
                                        Helpers.CheckStatus(response);
                                        await response.Content.ReadAsAsync<AddErrorResponse>();
                                    }
                                }
                            }
                            //Update execution time.
                            response = client.PostAsJsonAsync(Startup.ServerAddress + "/api/task/TaskReady", new TaskReady() { Tasks = new GXTask[] { task } }).Result;
                            Helpers.CheckStatus(response);
                        }
                    }
                    else
                    {
                        try
                        {
                            using (response = await client.PostAsJsonAsync(Startup.ServerAddress + "/api/task/WaitChange", new WaitChange() { Change = TargetType.Tasks, Time = lastUpdated, WaitTime = _waitTime }))
                            {
                                Helpers.CheckStatus(response);
                                {
                                    WaitChangeResponse r = await response.Content.ReadAsAsync<WaitChangeResponse>();
                                    if (r.Time > lastUpdated)
                                    {
                                        lastUpdated = r.Time;
                                    }
                                }
                            }
                        }
                        catch (Exception)
                        {
                            if (!_cancellationToken.IsCancellationRequested)
                            {
                                break;
                            }
                            _cancellationToken.WaitHandle.WaitOne(TimeSpan.FromSeconds(10));
                        }
                    }
                }
                catch (Exception ex)
                {
                    //If app is closing.
                    if (_cancellationToken.IsCancellationRequested)
                    {
                        break;
                    }
                    if (ret == null)
                    {
                        _logger.LogError("Failed to connect to the DB server.");
                    }
                    else
                    {
                        AddError error = new AddError();
                        error.Error = new GXError()
                        {
                            DeviceId = ret.Tasks[0].Object.DeviceId,
                            Error = "Failed to " + ret.Tasks[0].TaskType + " " + ret.Tasks[0].Object.LogicalName + ":" + ret.Tasks[0].Index + ". " + ex.Message
                        };
                        using (response = await client.PostAsJsonAsync(Startup.ServerAddress + "/api/error/AddError", error))
                        {
                            if (ret.Tasks != null)
                            {
                                DateTime now = DateTime.Now;
                                foreach (GXTask it in ret.Tasks)
                                {
                                    it.Result = ex.Message;
                                    it.End = now;
                                }
                                response = await client.PostAsJsonAsync(Startup.ServerAddress + "/api/task/TaskReady", new TaskReady() { Tasks = ret.Tasks });
                            }
                        }
                    }
                    _logger.LogError(ex.Message);
                }
            }
        }

        private void Cl_OnPdu(object sender, byte[] data)
        {
            string str = GXCommon.ToHex(data);
            Console.WriteLine(str);
            _logger.LogInformation(str);
            GXDLMSTranslator t = new GXDLMSTranslator(TranslatorOutputType.SimpleXml);
            str = t.PduToXml(data);
            Console.WriteLine(str);
            _logger.LogInformation(str);
        }

        public System.Threading.Tasks.Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogWarning("Reader Service is stopping.");
            return System.Threading.Tasks.Task.CompletedTask;
        }

        public void Dispose()
        {

        }
    }
}
