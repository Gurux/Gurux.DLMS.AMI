//
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
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Net.Http;
using Gurux.DLMS.Enums;
using Gurux.Service.Orm;
using Gurux.DLMS.AMI.Messages.DB;
using Gurux.DLMS.AMI.Messages.Rest;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Oracle.ManagedDataAccess.Client;
using System.Data.SQLite;
using Gurux.Net;
using Gurux.DLMS.Objects;
using Gurux.DLMS.Secure;
using System.Threading;
using System.Diagnostics;
using Gurux.Common;
using Gurux.DLMS.AMI.Messages.Enums;
using Gurux.DLMS.AMI.Internal;
using Gurux.DLMS.AMI.Reader;
using Microsoft.Extensions.Options;
using System.IO;
using Microsoft.Extensions.Configuration.Json;

namespace Gurux.DLMS.AMI
{
    public class Startup
    {
        GXNet listener;

        private static void OnClientConnected(object sender, Gurux.Common.ConnectionEventArgs e)
        {
            Console.WriteLine("Client {0} is connected.", e.Info);
            GXNet server = (GXNet)sender;
            try
            {
                GXNet media = server.Attach(e.Info);
                Thread thread = new Thread(new ParameterizedThreadStart(ReadMeter));
                thread.Start(media);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Read data from the meter.
        /// </summary>
        private static void ReadMeter(object parameter)
        {
            GXDLMSReader reader = null;
            try
            {
                var config = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json", optional: true)
                   .Build();
                ListenerOptions listener = config.GetSection("Listener").Get<ListenerOptions>();

                HttpClient httpClient = new HttpClient();
                using (GXNet media = (GXNet)parameter)
                {
                    GXDLMSObjectCollection objects = new GXDLMSObjectCollection();
                    GXDLMSSecureClient client = new GXDLMSSecureClient(listener.UseLogicalNameReferencing, listener.ClientAddress, listener.ServerAddress, (Authentication) listener.Authentication, listener.Password, (InterfaceType) listener.Interface);
                    reader = new GXDLMSReader(client, media, TraceLevel.Verbose, null);
                    GXDLMSData ldn = new GXDLMSData("0.0.42.0.0.255");
                    ldn.SetUIDataType(2, DataType.String);
                    reader.InitializeConnection();
                    reader.Read(ldn, 2);
                    Console.WriteLine("Meter connected: " + ldn.Value);
                    //Find device.
                    GXDevice dev = null;
                    ListDevicesResponse devs = null;
                    using (HttpClient cl = new HttpClient())
                    {
                        using (HttpResponseMessage response = cl.PostAsJsonAsync(Startup.ServerAddress + "/api/device/ListDevices", new ListDevices() { Name = (string)ldn.Value }).Result)
                        {
                            Helpers.CheckStatus(response);
                            devs = response.Content.ReadAsAsync<ListDevicesResponse>().Result;
                        }
                        if (devs.Devices.Length == 0)
                        {
                            //If device is unknown.
                            Console.WriteLine("Unknown Meter: " + ldn.Value);
                            return;
                        }
                        else if (devs.Devices.Length != 1)
                        {
                            throw new Exception("There are multiple devices with same name: " + ldn.Value);
                        }
                        else
                        {
                            dev = devs.Devices[0];
                            Console.WriteLine("Reading frame counter.");
                            GXDLMSData fc = new GXDLMSData(listener.InvocationCounter);
                            reader.Read(fc, 2);
                            dev.InvocationCounter = 1 + Convert.ToUInt32(fc.Value);
                            reader.Release();
                            Console.WriteLine("Device ID: " + dev.Id + " LDN: " + (string)ldn.Value);
                            Console.WriteLine("Frame counter: " + dev.FrameCounter);
                            GetNextTaskResponse ret;
                            using (HttpResponseMessage response = cl.PostAsJsonAsync(Startup.ServerAddress + "/api/task/GetNextTask", new GetNextTask() { DeviceId = dev.Id }).Result)
                            {
                                Helpers.CheckStatus(response);
                                ret = response.Content.ReadAsAsync<GetNextTaskResponse>().Result;
                            }
                            if (ret.Tasks == null || ret.Tasks.Length == 0)
                            {
                                Console.WriteLine("No tasks to execute");
                            }
                            else
                            {
                                Console.WriteLine("Task count: " + ret.Tasks.Length);
                                client = new GXDLMSSecureClient(dev.UseLogicalNameReferencing, dev.ClientAddress, dev.PhysicalAddress, (Authentication)dev.Authentication, dev.Password, dev.InterfaceType);
                                client.UtcTimeZone = dev.UtcTimeZone;
                                client.Standard = (Standard)dev.Standard;
                                if (dev.Conformance != 0)
                                {
                                    client.ProposedConformance = (Conformance)dev.Conformance;
                                }
                                client.Priority = dev.Priority;
                                client.ServiceClass = dev.ServiceClass;
                                client.Ciphering.SystemTitle = GXCommon.HexToBytes(dev.ClientSystemTitle);
                                client.Ciphering.BlockCipherKey = GXCommon.HexToBytes(dev.BlockCipherKey);
                                client.Ciphering.AuthenticationKey = GXCommon.HexToBytes(dev.AuthenticationKey);
                                client.ServerSystemTitle = GXCommon.HexToBytes(dev.DeviceSystemTitle);
                                client.Ciphering.InvocationCounter = dev.InvocationCounter;
                                client.Ciphering.Security = (Security)dev.Security;
                                reader = new GXDLMSReader(client, media, TraceLevel.Verbose, null);
                                List<GXValue> values = new List<GXValue>();
                                foreach (GXTask task in ret.Tasks)
                                {
                                    GXDLMSObject obj = GXDLMSClient.CreateObject((ObjectType)task.Object.ObjectType);
                                    obj.LogicalName = task.Object.LogicalName;
                                    if (task.TaskType == TaskType.Read)
                                    {
                                        Reader.Reader.Read(null, httpClient, reader, task, media, obj);
                                    }
                                    try
                                    {
                                        Reader.Reader.Read(null, httpClient, reader, task, media, obj);
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
                                        using (HttpResponseMessage response = httpClient.PostAsJsonAsync(Startup.ServerAddress + "/api/error/AddError", error).Result)
                                        {
                                            Helpers.CheckStatus(response);
                                            response.Content.ReadAsAsync<AddErrorResponse>();
                                        }
                                    }
                                    using (HttpResponseMessage response = cl.PostAsJsonAsync(Startup.ServerAddress + "/api/task/TaskReady", new TaskReady() { Tasks = new GXTask[] { task } }).Result)
                                    {
                                        Helpers.CheckStatus(response);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Release();
                }
            }
        }

        public Startup(IConfiguration configuration)
        {
            int port = configuration.GetSection("Listener").Get<ListenerOptions>().Port;
            if (port != 0)
            {
                listener = new GXNet(NetworkType.Tcp, port);
                listener.OnClientConnected += OnClientConnected;
                Console.WriteLine("Listening port:" + listener.Port);
                listener.Open();
            }
            Configuration = configuration;
            ServerAddress = configuration.GetSection("Client").Get<ClientOptions>().Address;
            Console.WriteLine("RestAddress: " + ServerAddress);
        }

        public IConfiguration Configuration { get; }


        public static string ServerAddress
        {
            get;
            set;
        }

        private async void AddSchedule()
        {
            GXSchedule m = new GXSchedule();
            m.Name = "Minutely";
            GXDateTime dt = new GXDateTime(DateTime.Now.Date);
            dt.Skip = DateTimeSkips.Year | DateTimeSkips.Month | DateTimeSkips.Day | DateTimeSkips.Hour | DateTimeSkips.Minute;
            m.Start = dt.ToFormatString();
            UpdateSchedule us = new UpdateSchedule();
            us.Schedules.Add(m);
            GXSchedule h = new GXSchedule();
            h.Name = "Hourly";
            dt.Skip = DateTimeSkips.Year | DateTimeSkips.Month | DateTimeSkips.Day | DateTimeSkips.Hour;
            h.Start = dt.ToFormatString();
            us.Schedules.Add(h);
            GXSchedule d = new GXSchedule();
            d.Name = "Daily";
            dt.Skip = DateTimeSkips.Year | DateTimeSkips.Month | DateTimeSkips.Day;
            d.Start = dt.ToFormatString();
            us.Schedules.Add(d);
            using (HttpClient client = new HttpClient())
            {
                using (HttpResponseMessage response = await client.PostAsJsonAsync(ServerAddress + "/api/schedule/UpdateSchedule", us))
                {
                    Helpers.CheckStatus(response);
                    UpdateScheduleResponse r = await response.Content.ReadAsAsync<UpdateScheduleResponse>();
                }
            }
        }

        public static async void AddDevice(GXDevice dev)
        {
            UpdateDevice gw = new UpdateDevice();
            gw.Device = dev;
            using (HttpClient client = new HttpClient())
            {
                using (HttpResponseMessage response = await client.PostAsJsonAsync(ServerAddress + "/api/device/UpdateDevice", gw))
                {
                    Helpers.CheckStatus(response);
                    UpdateDeviceResponse r = await response.Content.ReadAsAsync<UpdateDeviceResponse>();
                    dev.Id = r.DeviceId;
                }
            }
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            if (!Configuration.GetSection("Scheduler").Get<SchedulerOptions>().Disabled)
            {
                services.AddHostedService<TimedHostedService>();
            }

            string settings = Configuration.GetSection("Database").Get<DatabaseOptions>().Settings;
            string type = Configuration.GetSection("Database").Get<DatabaseOptions>().Type;
            Console.WriteLine("Database type: " + type);
            Console.WriteLine("Connecting: " + settings);
            DbConnection connection;
            if (string.IsNullOrEmpty(type))
            {
                //Gurux.DLMS.AMI DB is defined elsewhere.
                connection = null;
            }
            if (string.Compare(type, "Oracle", true) == 0)
            {
                connection = new OracleConnection(settings);
            }
            else if (string.Compare(type, "MSSQL", true) == 0)
            {
                connection = new SqlConnection(settings);
            }
            else if (string.Compare(type, "MySQL", true) == 0)
            {
                connection = new MySql.Data.MySqlClient.MySqlConnection(settings);
            }
            else if (string.Compare(type, "SQLite", true) == 0)
            {
                connection = new SQLiteConnection(settings);
            }
            else
            {
                throw new Exception("Invalid connection type. " + type);
            }
            if (connection != null)
            {
                connection.Open();
                GXHost h = new GXHost()
                {
                    Connection = new GXDbConnection(connection, null)
                };
                if (!h.Connection.TableExist<GXDevice>())
                {
                    Console.WriteLine("Creating tables.");
                    h.Connection.CreateTable<GXSystemError>(false, false);
                    h.Connection.CreateTable<GXDeviceTemplate>(false, false);
                    h.Connection.CreateTable<GXDevice>(false, false);
                    h.Connection.CreateTable<GXObjectTemplate>(false, false);
                    h.Connection.CreateTable<GXAttributeTemplate>(false, false);
                    h.Connection.CreateTable<GXObject>(false, false);
                    h.Connection.CreateTable<GXAttribute>(false, false);
                    h.Connection.CreateTable<GXValue>(false, false);
                    h.Connection.CreateTable<GXTask>(false, false);
                    h.Connection.CreateTable<GXError>(false, false);
                    h.Connection.CreateTable<GXSchedule>(false, false);
                    h.Connection.CreateTable<GXScheduleToAttribute>(false, false);
                    h.Connection.CreateTable<GXSchedulerInfo>(false, false);
                    h.Connection.CreateTable<GXReaderInfo>(false, false);
                    h.Connection.CreateTable<GXDeviceToReader>(false, false);
                    AddSchedule();
                }
                h.Connection.Insert(GXInsertArgs.Insert(new GXSystemError()
                {
                    Generation = DateTime.Now,
                    Error = "Service started: " + ServerAddress
                })); ;
                Console.WriteLine("Service started: " + ServerAddress);
                services.AddScoped<GXHost>(q =>
                {
                    return h;
                });
            }
            services.Configure<ReaderOptions>(Configuration.GetSection("Reader"));
            if (!Configuration.GetSection("Scheduler").Get<SchedulerOptions>().Disabled)
            {
                services.AddHostedService<TimedHostedService>();
            }
            if (Configuration.GetSection("Reader").Get<ReaderOptions>().Threads != 0)
            {
                services.AddHostedService<ReaderService>();
            }
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            //Add exception handler.
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }
            app.UseMvc();
        }
    }
}
