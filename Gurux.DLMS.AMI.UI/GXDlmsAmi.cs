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
using Gurux.DLMS.Objects;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Gurux.DLMS.AMI.Messages.Rest;
using System.Net.Http;
using Gurux.DLMS.AMI.Messages.DB;
using Gurux.DLMS.Enums;
using Gurux.DLMS.AMI.Messages.Enums;
using Gurux.DLMS.ManufacturerSettings;

namespace Gurux.DLMS.AMI.UI
{
    public class GXDlmsAmi : IGXDataConcentrator
    {
        internal GXDeviceTemplate[] templates;
        private MeterAddEventHandler add;
        private MeterRemoveEventHandler remove;
        private MeterEditEventHandler edit;

        static GXDlmsAmi()
        {
            //Update previous installed settings.
            //If file is corrupted it's found from:
            //%USERPROFILE%\AppData\Local\Gurux_Ltd\
            //This might happen if Windows is not closed correctly.
            if (Properties.Settings.Default.UpdateSettings)
            {
                Properties.Settings.Default.Upgrade();
                Properties.Settings.Default.UpdateSettings = false;
                Properties.Settings.Default.Save();
            }
        }

        private string GetServerAddress(string address)
        {
            if (string.IsNullOrEmpty(Properties.Settings.Default.ServerAddress))
            {
                //Update addresses.
                string[] list = Helpers.GetServerAddress();
                //Select first item.
                if (list.Length != 0)
                {
                    Properties.Settings.Default.ServerAddress = list[0];
                }
            }
            if (!IsServerConfigured())
            {
                throw new Exception("Invalid Gurux.DLMS.AMI Server address.");
            }
            return new Uri(new Uri(Properties.Settings.Default.ServerAddress), address).ToString();
        }

        public bool IsServerConfigured()
        {
            return !string.IsNullOrEmpty(Properties.Settings.Default.ServerAddress);
        }

        public string Name => "Gurux.DLMS.AMI";

        public Actions Actions => Actions.All;

        public Functionality Functionality => Functionality.All;

        public event MeterAddEventHandler OnMeterAdd
        {
            add
            {
                add += value;
            }
            remove
            {
                add -= value;
            }
        }

        public event MeterRemoveEventHandler OnMeterRemove
        {
            add
            {
                remove += value;
            }
            remove
            {
                remove -= value;
            }
        }

        public event MeterEditEventHandler OnMeterEdit
        {
            add
            {
                edit += value;
            }
            remove
            {
                edit -= value;
            }
        }

        public GXDLMSMeterBase[] AddDevices(GXDLMSMeterBase[] devices)
        {
            if (add != null)
            {
                add(this, devices);
            }
            using (HttpClient cl = new HttpClient())
            {
                List<GXDLMSMeter> meters = new List<GXDLMSMeter>();
                foreach (GXDevice it in devices)
                {
                    using (HttpResponseMessage response = cl.PostAsJsonAsync(GetServerAddress("/api/device/UpdateDevice"), new UpdateDevice() { Device = it }).Result)
                    {
                        Helpers.CheckStatus(response);
                        UpdateDeviceResponse devs = response.Content.ReadAsAsync<UpdateDeviceResponse>().Result;
                        it.Id = devs.DeviceId;
                    }
                    using (HttpResponseMessage response = cl.PostAsJsonAsync(GetServerAddress("/api/device/ListDevices"),
                        new ListDevices() { Targets = TargetType.Device | TargetType.Object | TargetType.Attribute, Ids = new ulong[] { it.Id } }).Result)
                    {
                        Helpers.CheckStatus(response);
                        ListDevicesResponse devs = response.Content.ReadAsAsync<ListDevicesResponse>().Result;
                        meters.AddRange(DevicesToMeters(devs.Devices));
                    }
                }
                return meters.ToArray();
            }
        }

        public void AddObjects(GXDLMSMeterBase[] devices, GXDLMSObject[] objects)
        {
            throw new NotImplementedException();
        }

        public Form[] CustomPages(object target, object communication)
        {
            if (target is List<GXDLMSMeter>)
            {
                return new Form[] { new TemplatesForm(this), new SystemErrors(this), new ReadersInfoForm(this), new SchedulersForm(this, null) };
            }
            if (target is GXDevice)
            {
                return new Form[] { new TaskForm(target as GXDevice, this), new SchedulersForm(this, target) };
            }
            return null;
        }

        public void EditDevices(GXDLMSMeterBase[] devices)
        {
            if (edit != null)
            {
                edit(this, devices);
            }
            using (HttpClient cl = new HttpClient())
            {
                foreach (GXDLMSMeter it in devices)
                {
                    using (HttpResponseMessage response = cl.PostAsJsonAsync(GetServerAddress("/api/device/UpdateDevice"), new UpdateDevice() { Device = MeterToDevice(it) }).Result)
                    {
                        Helpers.CheckStatus(response);
                        UpdateDeviceResponse devs = response.Content.ReadAsAsync<UpdateDeviceResponse>().Result;
                    }
                }
            }
        }

        public void EditObjects(GXDLMSMeterBase[] devices, GXDLMSObject[] objects)
        {
            throw new NotImplementedException();
        }

        GXDLMSMeterBase[] IGXDataConcentrator.GetDevices(string name)
        {
            return GetDevices(name, TargetType.Device);
        }

        private static GXDevice MeterToDevice(GXDLMSMeter meter)
        {
            GXDevice d = new GXDevice();
            GXDevice.Copy(d, meter);
            d.Id = (UInt64)meter.Tag;
            return d;
        }

        private static GXDevice[] MetersToDevices(GXDLMSMeter[] meters)
        {
            List<GXDevice> devices = new List<GXDevice>();
            foreach (GXDLMSMeter it in meters)
            {
                devices.Add(MeterToDevice(it));
            }
            return devices.ToArray();
        }

        private static GXDLMSMeter DeviceToMeter(GXDevice device)
        {
            GXDLMSMeter m = new GXDLMSMeter();
            GXDevice.Copy(m, device);
            m.Tag = device.Id;
            return m;
        }

        private static GXDLMSMeter[] DevicesToMeters(GXDevice[] devices)
        {
            List<GXDLMSMeter> meters = new List<GXDLMSMeter>();
            foreach (GXDevice it in devices)
            {
                meters.Add(DeviceToMeter(it));
            }
            return meters.ToArray();
        }

        public GXDLMSMeter[] GetDevices(string name = null, TargetType targets = TargetType.Device | TargetType.Object | TargetType.Attribute)
        {
            using (HttpClient cl = new HttpClient())
            {
                using (HttpResponseMessage response = cl.PostAsJsonAsync(GetServerAddress("/api/device/ListDevices"),
                    new ListDevices() { Targets = targets, Name = name }).Result)
                {
                    Helpers.CheckStatus(response);
                    ListDevicesResponse devs = response.Content.ReadAsAsync<ListDevicesResponse>().Result;
                    return DevicesToMeters(devs.Devices);
                }
            }
        }

        public List<KeyValuePair<GXDLMSMeterBase, string[]>> GetErrors(GXDLMSMeterBase[] devices)
        {
            /*
            GXError[] errors = GetErrors2((GXDevice[]) devices);
            List<KeyValuePair<GXDLMSMeter, string[]>> list = new List<KeyValuePair<GXDLMSMeter, string[]>>();
            foreach (GXError it in errors)
            {
                foreach(KeyValuePair<GXDLMSMeter, string[]> p in list)
                {
                    if ((GXDLMSMeter) p.Key.id)
                }
                if (!list.Contains(it.DeviceId))
                {
                    list.Add(it.DeviceId);
                }
            }
            */
            throw new NotImplementedException();
        }

        public void MethodObjects(GXDLMSMeterBase[] devices, List<GXActionParameter> actions)
        {
            List<GXTask> list = new List<GXTask>();
            foreach (GXDLMSMeterBase m in devices)
            {
                foreach (GXActionParameter it in actions)
                {
                    {
                        GXTask t = new GXTask();
                        t.TaskType = TaskType.Action;
                        t.Object = new GXObject() { DeviceId = (UInt64)((GXDLMSMeter)m).Tag, LogicalName = it.Target.LogicalName, ObjectType = (int)it.Target.ObjectType };
                        t.Index = it.Index;
                        t.Data = (string)it.Data;
                        list.Add(t);
                    }
                }
            }
            AddTasks(list);
        }

        public void ReadObjects(GXDLMSMeterBase[] meters, List<KeyValuePair<GXDLMSObject, byte>> objects)
        {
            List<GXTask> list = new List<GXTask>();
            foreach (GXDLMSMeterBase m in meters)
            {
                foreach (KeyValuePair<GXDLMSObject, byte> it in objects)
                {
                    if ((it.Key is GXDLMSAssociationLogicalName && it.Value == 2) ||
                        (it.Key is GXDLMSAssociationShortName && it.Value == 2))
                    {
                        if ((it.Key is GXDLMSAssociationLogicalName))
                        {
                            ((GXDLMSAssociationLogicalName)it.Key).ObjectList.Clear();
                        }
                        else
                        {
                            ((GXDLMSAssociationShortName)it.Key).ObjectList.Clear();
                        }
                        using (HttpClient cl = new HttpClient())
                        {
                            using (HttpResponseMessage response = cl.PostAsJsonAsync(GetServerAddress("/api/device/ListDevices"),
                                new ListDevices() { Ids = new UInt64[] { (UInt64)((GXDLMSMeter)m).Tag }, Targets = TargetType.Object }).Result)
                            {
                                Helpers.CheckStatus(response);
                                ListDevicesResponse devs = response.Content.ReadAsAsync<ListDevicesResponse>().Result;
                                foreach (GXObject o in devs.Devices[0].Objects)
                                {
                                    GXDLMSObject obj = GXDLMSClient.CreateObject((ObjectType)o.ObjectType);
                                    obj.LogicalName = o.LogicalName;
                                    obj.Description = o.Name;
                                    obj.Version = o.Version;
                                    if (o.Attributes != null)
                                    {
                                        foreach (GXAttribute a in o.Attributes)
                                        {
                                            obj.SetAccess(a.Index, (AccessMode)a.AccessLevel);
                                            GXDLMSAttributeSettings att = obj.Attributes.Find(a.Index);
                                            att.Static = a.ExpirationTime == 0xFFFFFFFF;
                                            att.Type = (DataType)a.DataType;
                                            att.UIType = (DataType)a.UIDataType;
                                            att.Access = (AccessMode)a.AccessLevel;
                                        }
                                    }
                                    if (it.Key is GXDLMSAssociationLogicalName)
                                    {
                                        ((GXDLMSAssociationLogicalName)it.Key).ObjectList.Add(obj);
                                    }
                                    else
                                    {
                                        ((GXDLMSAssociationShortName)it.Key).ObjectList.Add(obj);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        GXTask t = new GXTask();
                        t.TaskType = TaskType.Read;
                        t.Object = new GXObject() { DeviceId = (UInt64)((GXDLMSMeter)m).Tag, LogicalName = it.Key.LogicalName, ObjectType = (int)it.Key.ObjectType };
                        t.Index = it.Value;
                        list.Add(t);
                    }
                }
            }
            AddTasks(list);
        }

        public void RemoveDevices(GXDLMSMeterBase[] devices)
        {
            if (remove != null)
            {
                remove(this, devices);
            }
            using (HttpClient cl = new HttpClient())
            {
                List<UInt64> list = new List<ulong>();
                foreach (GXDLMSMeter it in devices)
                {
                    list.Add((UInt64)it.Tag);
                }
                using (HttpResponseMessage response = cl.PostAsJsonAsync(GetServerAddress("/api/device/DeviceDelete"), new DeviceDelete() { Ids = list.ToArray() }).Result)
                {
                    Helpers.CheckStatus(response);
                    UpdateDeviceResponse devs = response.Content.ReadAsAsync<UpdateDeviceResponse>().Result;
                }
            }
        }

        public void RemoveObjects(GXDLMSMeterBase[] devices, GXDLMSObject[] objects)
        {
            throw new NotImplementedException();
        }

        public void WriteObjects(GXDLMSMeterBase[] devices, List<KeyValuePair<GXDLMSObject, byte>> objects)
        {
            List<GXTask> list = new List<GXTask>();
            GXDLMSTranslator tr = new GXDLMSTranslator(TranslatorOutputType.SimpleXml);
            foreach (GXDLMSMeterBase m in devices)
            {
                GXDLMSSettings s = new GXDLMSSettings();
                s.Standard = m.Standard;
                s.UseUtc2NormalTime = m.UtcTimeZone;
                foreach (KeyValuePair<GXDLMSObject, byte> it in objects)
                {
                    {
                        ValueEventArgs v = new ValueEventArgs(it.Key, it.Value, 0, null);
                        GXTask t = new GXTask();
                        t.TaskType = TaskType.Write;
                        object value = ((IGXDLMSBase)it.Key).GetValue(s, v);
                        DataType dt = ((IGXDLMSBase)it.Key).GetDataType(it.Value);
                        if (dt == DataType.Array || dt == DataType.Structure)
                        {
                            string xml = "";
                            tr.DataToXml((byte[])value, out xml);
                            t.Data = xml;
                        }
                        else
                        {
                            t.Data = GXDLMSTranslator.ValueToXml(it.Key.GetValues()[it.Value - 1]);
                        }
                        t.Object = new GXObject() { DeviceId = (UInt64)((GXDLMSMeter)m).Tag, LogicalName = it.Key.LogicalName, ObjectType = (int)it.Key.ObjectType };
                        t.Index = it.Value;
                        list.Add(t);
                    }
                }
            }
            AddTasks(list);
        }

        public void AddDeviceTemplates(GXDLMSMeterBase[] devices)
        {
            using (HttpClient cl = new HttpClient())
            {
                GXDeviceTemplate d = new GXDeviceTemplate();
                foreach (GXDLMSMeterBase it in devices)
                {
                    GXDevice.Copy(d, it);
                    if (it is GXDeviceTemplate)
                    {
                        d.Objects = ((GXDeviceTemplate)it).Objects;
                    }
                    else
                    {
                        d.Objects = new List<GXObjectTemplate>();
                        foreach (GXDLMSObject o in ((GXDLMSMeter)it).Objects)
                        {
                            GXObjectTemplate t = new GXObjectTemplate();
                            t.Attributes = new List<GXAttributeTemplate>();
                            t.LogicalName = o.LogicalName;
                            t.ObjectType = (int)o.ObjectType;
                            t.Version = o.Version;
                            d.Objects.Add(t);

                            for (int pos = 2; pos <= ((IGXDLMSBase)o).GetAttributeCount(); ++pos)
                            {
                                GXAttributeTemplate a = new GXAttributeTemplate();
                                a.Index = pos;
                                t.Attributes.Add(a);
                            }
                        }
                    }
                    using (HttpResponseMessage response = cl.PostAsJsonAsync(GetServerAddress("/api/template/AddTemplate"),
                        new UpdateDeviceTemplate() { Device = d }).Result)
                    {
                        Helpers.CheckStatus(response);
                        UpdateDeviceTemplateResponse res = response.Content.ReadAsAsync<UpdateDeviceTemplateResponse>().Result;
                        if (it is GXDeviceTemplate)
                        {
                            ((GXDeviceTemplate)it).Id = res.DeviceId;
                        }
                    }
                }
            }
            templates = GetDeviceTemplates();
            for (int pos = 0; pos != devices.Length; ++pos)
            {
                GXDLMSMeterBase it = devices[pos];
                if (it is GXDeviceTemplate)
                {
                    GXDeviceTemplate m = (GXDeviceTemplate)it;
                    //Update device, object and attribute IDs.
                    foreach (GXDeviceTemplate dt in templates)
                    {
                        if (dt.Id == m.Id)
                        {
                            m.Objects.Clear();
                            m.Objects.AddRange(dt.Objects);
                            break;
                        }
                    }
                }
            }
        }

        private void UpdateValue(GXDLMSSettings s, GXObject o, KeyValuePair<GXDLMSObject, byte> it)
        {
            foreach (var a in o.Attributes)
            {
                if (a.Index == it.Value)
                {
                    ValueEventArgs e = new ValueEventArgs(it.Key, it.Value, 0, null);
                    if (a.DataType == (int)DataType.OctetString)
                    {
                        if (a.UIDataType == (int)DataType.OctetString || a.UIDataType == (int)DataType.None)
                        {
                            if (a.Value != null && a.Value.Contains("."))
                            {
                                e.Value = GXDLMSConverter.LogicalNameToBytes(a.Value);
                            }
                            else
                            {
                                e.Value = GXDLMSTranslator.HexToBytes(a.Value);
                            }
                            if (((byte[])e.Value).Length == 0)
                            {
                                e.Value = null;
                            }
                        }
                        else
                        {
                            e.Value = a.Value;
                        }
                    }
                    else if (a.DataType == (int)DataType.Array || a.DataType == (int)DataType.Structure)
                    {
                        if (string.IsNullOrEmpty(a.Value))
                        {
                            e.Value = null;
                        }
                        else
                        {
                            e.Value = GXDLMSTranslator.XmlToValue(a.Value);
                        }
                    }
                    else if (a.DataType == (int)DataType.Enum)
                    {
                        if (a.Value != null)
                        {
                            e.Value = Convert.ChangeType(a.Value, typeof(int));
                        }
                        else
                        {
                            e.Value = 0;
                        }
                    }
                    else if (a.DataType == (int)DataType.DateTime ||
                                a.DataType == (int)DataType.Date ||
                                a.DataType == (int)DataType.Time)
                    {
                        if (!string.IsNullOrEmpty(a.Value))
                        {
                            e.Value = Convert.ChangeType(a.Value, GXDLMSConverter.GetDataType((DataType)a.DataType));
                        }
                        else
                        {
                            e.Value = null;
                        }
                    }
                    else if (a.DataType != 0)
                    {
                        if (a.Value == null)
                        {
                            if (a.DataType == (int)DataType.Boolean)
                            {
                                a.Value = "False";
                            }
                            else
                            {
                                a.Value = "0";
                            }
                        }
                        try
                        {
                            e.Value = Convert.ChangeType(a.Value, GXDLMSConverter.GetDataType((DataType)a.DataType));
                        }
                        catch
                        {
                            //IP4Setup ip address data type is UInt32, but value is in string. This causes convert failure.
                            e.Value = a.Value;
                        }
                    }
                    else
                    {
                        e.Value = a.Value;
                    }
                    ((IGXDLMSBase)it.Key).SetValue(s, e);
                    //Update last error.
                    ((GXDLMSObject)it.Key).SetLastReadTime(a.Index, a.Read);
                    if (string.IsNullOrEmpty(a.Exception))
                    {
                        ((GXDLMSObject)it.Key).SetLastError(a.Index, null);
                    }
                    else
                    {
                        ((GXDLMSObject)it.Key).SetLastError(a.Index, new Exception(a.Exception));
                    }
                    break;
                }
            }
        }

        public void GetValues(GXDLMSMeterBase[] devices, List<KeyValuePair<GXDLMSObject, byte>> objects, bool readAll)
        {
            ListObjects req = new ListObjects();
            req.Targets = TargetType.Object | TargetType.Attribute;
            foreach (GXDLMSMeter d in devices)
            {
                req.DeviceId = (UInt64)d.Tag;
                List<GXObject> list = new List<GXObject>();
                //Ignore this if we want to read all the objects.
                if (!readAll)
                {
                    foreach (KeyValuePair<GXDLMSObject, byte> it in objects)
                    {
                        GXObject o = new GXObject();
                        o.LogicalName = it.Key.LogicalName;
                        o.ObjectType = (int)it.Key.ObjectType;
                        bool exists = false;
                        foreach (GXObject t in list)
                        {
                            if (t.ObjectType == o.ObjectType && t.LogicalName == o.LogicalName)
                            {
                                o = t;
                                exists = true;
                                break;
                            }
                        }
                        if (!exists)
                        {
                            list.Add(o);
                        }
                        o.Attributes.Add(new GXAttribute() { Index = it.Value });
                    }
                }
                GXDLMSSettings s = new GXDLMSSettings();
                s.UseUtc2NormalTime = d.UtcTimeZone;
                s.Standard = d.Standard;
                if (list.Count != 0)
                {
                    req.Objects = list.ToArray();
                }
                using (HttpClient cl = new HttpClient())
                {
                    using (HttpResponseMessage response = cl.PostAsJsonAsync(GetServerAddress("/api/Object/ListObjects"), req).Result)
                    {
                        Helpers.CheckStatus(response);
                        ListObjectsResponse ret = response.Content.ReadAsAsync<ListObjectsResponse>().Result;
                        foreach (KeyValuePair<GXDLMSObject, byte> it in objects)
                        {
                            foreach (GXObject o in ret.Items)
                            {
                                if ((int)it.Key.ObjectType == o.ObjectType && it.Key.LogicalName == o.LogicalName)
                                {
                                    UpdateValue(s, o, it);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        public void UpdateDeviceTemplates(GXDeviceTemplate[] devices)
        {
            using (HttpClient cl = new HttpClient())
            {
                foreach (GXDeviceTemplate it in devices)
                {
                    using (HttpResponseMessage response = cl.PostAsJsonAsync(GetServerAddress("/api/template/AddTemplate"),
                        new UpdateDeviceTemplate() { Device = it }).Result)
                    {
                        Helpers.CheckStatus(response);
                        ListDeviceTemplatesResponse ret = response.Content.ReadAsAsync<ListDeviceTemplatesResponse>().Result;
                    }
                }
            }
            templates = GetDeviceTemplates();
        }

        public GXDeviceTemplate[] GetDeviceTemplates(string name = null, TargetType targets = TargetType.DeviceTemplate | TargetType.ObjectTemplate | TargetType.AttributeTemplate)
        {
            using (HttpClient cl = new HttpClient())
            {
                using (HttpResponseMessage response = cl.PostAsJsonAsync(GetServerAddress("/api/template/ListDeviceTemplates"),
                    new ListDeviceTemplates() { Targets = targets, Name = name }).Result)
                {
                    Helpers.CheckStatus(response);
                    ListDeviceTemplatesResponse ret = response.Content.ReadAsAsync<ListDeviceTemplatesResponse>().Result;
                    templates = ret.Devices;
                    return ret.Devices;
                }
            }
        }

        public void RemoveDeviceTemplates(GXDeviceTemplate[] devices)
        {
            using (HttpClient cl = new HttpClient())
            {
                List<UInt64> list = new List<UInt64>();
                foreach (GXDeviceTemplate it in devices)
                {
                    list.Add(it.Id);
                }
                using (HttpResponseMessage response = cl.PostAsJsonAsync(GetServerAddress("/api/template/DeviceTemplateDelete"),
                    new DeviceTemplateDelete() { Ids = list.ToArray() }).Result)
                {
                    Helpers.CheckStatus(response);
                    DeviceTemplateDeleteResponse ret = response.Content.ReadAsAsync<DeviceTemplateDeleteResponse>().Result;
                }
            }
        }

        public void RemoveObjectTemplates(GXObjectTemplate[] objects)
        {
            using (HttpClient cl = new HttpClient())
            {
                List<UInt64> list = new List<UInt64>();
                foreach (GXObjectTemplate it in objects)
                {
                    list.Add(it.Id);
                }
                using (HttpResponseMessage response = cl.PostAsJsonAsync(GetServerAddress("/api/template/ObjectTemplateDelete"),
                    new ObjectTemplateDelete() { Ids = list.ToArray() }).Result)
                {
                    Helpers.CheckStatus(response);
                    ObjectTemplateDeleteResponse devs = response.Content.ReadAsAsync<ObjectTemplateDeleteResponse>().Result;
                }
            }
        }

        public void RemoveAttributeTemplates(GXAttributeTemplate[] attributes)
        {
            using (HttpClient cl = new HttpClient())
            {
                List<UInt64> list = new List<UInt64>();
                foreach (GXAttributeTemplate it in attributes)
                {
                    list.Add(it.Id);
                }
                using (HttpResponseMessage response = cl.PostAsJsonAsync(GetServerAddress("/api/template/AttributeTemplateDelete"),
                    new AttributeTemplateDelete() { Ids = list.ToArray() }).Result)
                {
                    Helpers.CheckStatus(response);
                    AttributeTemplateDeleteResponse devs = response.Content.ReadAsAsync<AttributeTemplateDeleteResponse>().Result;
                }
            }
        }


        public void AddTasks(List<GXTask> tasks)
        {
            using (HttpClient cl = new HttpClient())
            {
                using (HttpResponseMessage response = cl.PostAsJsonAsync(GetServerAddress("/api/task/AddTask"),
                    new AddTask() { Actions = tasks.ToArray() }).Result)
                {
                    Helpers.CheckStatus(response);
                    DeviceTemplateDeleteResponse devs = response.Content.ReadAsAsync<DeviceTemplateDeleteResponse>().Result;
                }
            }
        }

        public GXTask[] GetTasks(object target, TargetType targets = TargetType.Tasks | TargetType.Device | TargetType.Object | TargetType.Attribute)
        {
            UInt64 deviceId = 0;
            UInt64 objectId = 0;
            if (target is GXDevice)
            {
                deviceId = ((GXDevice)target).Id;
            }
            else if (target is GXObject)
            {
                objectId = ((GXObject)target).Id;
            }
            using (HttpClient cl = new HttpClient())
            {
                using (HttpResponseMessage response = cl.PostAsJsonAsync(GetServerAddress("/api/task/ListTasks"),
                    new ListTasks() { Targets = targets, DeviceId = deviceId, ObjectId = objectId }).Result)
                {
                    Helpers.CheckStatus(response);
                    ListTasksResponse devs = response.Content.ReadAsAsync<ListTasksResponse>().Result;
                    return devs.Tasks;
                }
            }
        }

        public void RemoveTasks(GXTask[] tasks)
        {
            using (HttpClient cl = new HttpClient())
            {
                List<UInt64> list = new List<UInt64>();
                foreach (GXTask it in tasks)
                {
                    list.Add(it.Id);
                }
                using (HttpResponseMessage response = cl.PostAsJsonAsync(GetServerAddress("/api/task/TaskDelete"),
                    new DeleteTask() { Ids = list.ToArray() }).Result)
                {
                    Helpers.CheckStatus(response);
                    DeleteTaskResponse ret = response.Content.ReadAsAsync<DeleteTaskResponse>().Result;
                }
            }
        }

        public void AddSchedules(List<GXSchedule> schedules)
        {
            using (HttpClient cl = new HttpClient())
            {
                using (HttpResponseMessage response = cl.PostAsJsonAsync(GetServerAddress("/api/schedule/UpdateSchedule"),
                    new UpdateSchedule() { Schedules = schedules }).Result)
                {
                    Helpers.CheckStatus(response);
                    UpdateScheduleResponse ret = response.Content.ReadAsAsync<UpdateScheduleResponse>().Result;
                    for (int pos = 0; pos != schedules.Count; ++pos)
                    {
                        schedules[pos].Id = ret.ScheduleIds[pos];
                    }
                }
            }
        }

        public void UpdateSchedules(List<GXSchedule> schedules)
        {
            using (HttpClient cl = new HttpClient())
            {
                using (HttpResponseMessage response = cl.PostAsJsonAsync(GetServerAddress("/api/schedule/UpdateSchedule"),
                    new UpdateSchedule() { Schedules = schedules }).Result)
                {
                    Helpers.CheckStatus(response);
                    DeviceTemplateDeleteResponse ret = response.Content.ReadAsAsync<DeviceTemplateDeleteResponse>().Result;
                }
            }
        }

        public void AddScheduleTarget(GXSchedule[] schedules, GXObject[] targets)
        {
            List<UInt64> s = new List<UInt64>();
            foreach (GXSchedule it in schedules)
            {
                s.Add(it.Id);
            }
            List<GXObject> list = new List<GXObject>();
            foreach (GXObject it in targets)
            {
                GXObject o = new GXObject();
                list.Add(o);
                foreach (GXAttribute a in it.Attributes)
                {
                    o.Attributes.Add(new GXAttribute() { Id = a.Id });
                }
            }
            using (HttpClient cl = new HttpClient())
            {
                using (HttpResponseMessage response = cl.PostAsJsonAsync(GetServerAddress("/api/schedule/AddScheduleTarget"),
                    new AddScheduleTarget() { Schedules = s.ToArray(), Objects = list.ToArray() }).Result)
                {
                    Helpers.CheckStatus(response);
                    AddScheduleTargetResponse ret = response.Content.ReadAsAsync<AddScheduleTargetResponse>().Result;
                }
            }
        }

        public void RemoveScheduleTarget(GXSchedule[] schedules, GXAttribute[] attributes)
        {
            List<UInt64> s = new List<UInt64>();
            foreach (GXSchedule it in schedules)
            {
                s.Add(it.Id);
            }
            List<UInt64> t = new List<UInt64>();
            foreach (GXAttribute it in attributes)
            {
                t.Add(it.Id);
            }
            using (HttpClient cl = new HttpClient())
            {
                using (HttpResponseMessage response = cl.PostAsJsonAsync(GetServerAddress("/api/schedule/DeleteScheduleTarget"),
                    new DeleteScheduleTarget() { Schedules = s.ToArray(), Attributes = t.ToArray() }).Result)
                {
                    Helpers.CheckStatus(response);
                    DeleteScheduleTargetResponse ret = response.Content.ReadAsAsync<DeleteScheduleTargetResponse>().Result;
                }
            }
        }

        public GXSchedule[] GetSchedules(object target = null, TargetType targets = TargetType.Schedule | TargetType.Object | TargetType.Attribute)
        {
            UInt64 deviceId = 0;
            UInt64 objectId = 0;
            if (target is GXDevice)
            {
                deviceId = ((GXDevice)target).Id;
            }
            else if (target is GXObject)
            {
                objectId = ((GXObject)target).Id;
            }
            using (HttpClient cl = new HttpClient())
            {
                using (HttpResponseMessage response = cl.PostAsJsonAsync(GetServerAddress("/api/schedule/ListSchedules"),
                    new ListSchedules() { Targets = targets, DeviceId = deviceId, ObjectId = objectId }).Result)
                {
                    Helpers.CheckStatus(response);
                    ListSchedulesResponse ret = response.Content.ReadAsAsync<ListSchedulesResponse>().Result;
                    return ret.Schedules;
                }
            }
        }

        public void RemoveSchedules(GXSchedule[] tasks)
        {
            using (HttpClient cl = new HttpClient())
            {
                List<UInt64> list = new List<UInt64>();
                foreach (GXSchedule it in tasks)
                {
                    list.Add(it.Id);
                }
                using (HttpResponseMessage response = cl.PostAsJsonAsync(GetServerAddress("/api/schedule/DeleteSchedule"),
                    new DeleteSchedule() { ScheduleIds = list.ToArray() }).Result)
                {
                    Helpers.CheckStatus(response);
                    DeleteScheduleResponse ret = response.Content.ReadAsAsync<DeleteScheduleResponse>().Result;
                }
            }
        }

        public GXSystemError[] GetSystemErrors()
        {
            using (HttpClient cl = new HttpClient())
            {
                using (HttpResponseMessage response = cl.PostAsJsonAsync(GetServerAddress("/api/systemError/ListSystemErrors"),
                    new ListSystemErrors()).Result)
                {
                    Helpers.CheckStatus(response);
                    ListSystemErrorsResponse ret = response.Content.ReadAsAsync<ListSystemErrorsResponse>().Result;
                    return ret.Errors;
                }
            }
        }

        public void ClearSystemErrors()
        {
            using (HttpClient cl = new HttpClient())
            {
                using (HttpResponseMessage response = cl.PostAsJsonAsync(GetServerAddress("/api/systemError/ClearSystemErrors"),
                    new ClearSystemError()).Result)
                {
                    Helpers.CheckStatus(response);
                    ClearSystemErrorResponse ret = response.Content.ReadAsAsync<ClearSystemErrorResponse>().Result;
                }
            }
        }

        public GXError[] GetErrors2(GXDLMSMeterBase[] devices)
        {
            List<UInt64> list = new List<UInt64>();
            if (devices != null)
            {
                foreach (GXDevice it in devices)
                {
                    list.Add(it.Id);
                }
            }
            using (HttpClient cl = new HttpClient())
            {
                using (HttpResponseMessage response = cl.PostAsJsonAsync(GetServerAddress("/api/error/ListErrors"),
                    new ListErrors() { Ids = list.ToArray() }).Result)
                {
                    Helpers.CheckStatus(response);
                    ListErrorsResponse ret = response.Content.ReadAsAsync<ListErrorsResponse>().Result;
                    return ret.Errors;
                }
            }
        }

        public void ClearErrors(GXDevice[] devices)
        {
            List<UInt64> list = new List<UInt64>();
            if (devices != null)
            {
                foreach (GXDevice it in devices)
                {
                    list.Add(it.Id);
                }
            }
            using (HttpClient cl = new HttpClient())
            {
                using (HttpResponseMessage response = cl.PostAsJsonAsync(GetServerAddress("/api/error/ClearErrors"),
                    new ClearDeviceError() { Ids = list.ToArray() }).Result)
                {
                    Helpers.CheckStatus(response);
                    ClearDeviceErrorResponse ret = response.Content.ReadAsAsync<ClearDeviceErrorResponse>().Result;
                }
            }
        }


        public GXValue[] GetValue(GXObject[] objects)
        {
            List<UInt64> list = new List<UInt64>();
            if (objects != null)
            {
                foreach (GXObject it in objects)
                {
                    list.Add(it.Id);
                }
            }
            using (HttpClient cl = new HttpClient())
            {
                using (HttpResponseMessage response = cl.PostAsJsonAsync(GetServerAddress("/api/value/ListValues"),
                    new ListValues() { Ids = list.ToArray() }).Result)
                {
                    Helpers.CheckStatus(response);
                    ListValuesResponse ret = response.Content.ReadAsAsync<ListValuesResponse>().Result;
                    return ret.Items;
                }
            }
        }

        /// <summary>
        /// Add a new device.
        /// </summary>
        /// <param name="devices">Device to add.</param>
        public GXDLMSMeterBase AddDevice(IWin32Window owner)
        {
            if (templates == null)
            {
                templates = GetDeviceTemplates(null);
            }
            GXDevice meter = new GXDevice();
            DBDevicePropertiesForm dlg = new DBDevicePropertiesForm(templates, meter);
            if (dlg.ShowDialog(owner) == DialogResult.OK)
            {
                return AddDevices(new GXDLMSMeterBase[] { meter })[0];
            }
            return null;
        }

        /// <summary>
        /// Edit device.
        /// </summary>
        /// <param name="devices">Device to edit.</param>
        public bool EditDevice(IWin32Window owner, GXDLMSMeterBase device)
        {
            if (templates == null)
            {
                templates = GetDeviceTemplates(null);
            }
            DBDevicePropertiesForm dlg = new DBDevicePropertiesForm(templates, device);
            if (dlg.ShowDialog(owner) == DialogResult.OK)
            {
                EditDevices(new GXDLMSMeterBase[] { device });
                return true;
            }
            return false;
        }

        public GXReaderInfo[] GetReaders()
        {
            using (HttpClient cl = new HttpClient())
            {
                using (HttpResponseMessage response = cl.PostAsJsonAsync(GetServerAddress("/api/reader/ListReaders"),
                    new ListReaders()).Result)
                {
                    Helpers.CheckStatus(response);
                    ListReadersResponse ret = response.Content.ReadAsAsync<ListReadersResponse>().Result;
                    return ret.Readers;
                }
            }
        }

        public void AddToSchedule(IWin32Window owner, object target)
        {
            GXSchedule[] schedules = GetSchedules();
            ScheduleTargetsDlg dlg = new ScheduleTargetsDlg(schedules, schedules[0], target);
            if (dlg.ShowDialog(owner) == DialogResult.OK)
            {
                AddScheduleTarget req = new AddScheduleTarget();
                req.Schedules = new UInt64[] { dlg.Target.Id };
                List<GXObject> list = new List<GXObject>();
                if (target is GXObject)
                {
                    //Add only ID. This will make packet smaller.
                    req.Objects = new GXObject[] { new GXObject() { Id = ((GXObject)target).Id } };
                }
                else if (target is GXDLMSObject)
                {
                    //Add only Device ID and logical name and object type. This will make packet smaller.
                    req.DeviceId = (UInt64)(((GXDLMSObject)target).Parent.Tag as GXDLMSMeter).Tag;
                    req.Objects = new GXObject[] { new GXObject() {
                        ObjectType = (int)((GXDLMSObject)target).ObjectType,
                        LogicalName = ((GXDLMSObject)target).LogicalName } };
                }
                else if (target is GXDLMSObjectCollection)
                {
                    req.DeviceId = (UInt64) (((GXDLMSObjectCollection)target).Tag as GXDLMSMeter).Tag;
                    List<GXObject> objects = new List<GXObject>();
                    foreach (GXDLMSObject it in (GXDLMSObjectCollection)target)
                    {
                        objects.Add(new GXObject()
                        {
                            ObjectType = (int)it.ObjectType,
                            LogicalName = it.LogicalName
                        });
                    }
                    req.Objects = objects.ToArray();
                }
                else if (target is GXDLMSMeter)
                {
                    GXDLMSMeter m = target as GXDLMSMeter;
                    req.DeviceId = (UInt64)m.Tag;
                    List<GXObject> objects = new List<GXObject>();
                    foreach (GXDLMSObject it in m.Objects)
                    {
                        objects.Add(new GXObject()
                        {
                            ObjectType = (int)it.ObjectType,
                            LogicalName = it.LogicalName
                        });
                    }
                    req.Objects = objects.ToArray();
                }
                else
                {
                    throw new Exception("Invalid target.");
                }
                using (HttpClient cl = new HttpClient())
                {
                    using (HttpResponseMessage response = cl.PostAsJsonAsync(GetServerAddress("/api/schedule/AddScheduleTarget"),
                        req).Result)
                    {
                        Helpers.CheckStatus(response);
                        AddScheduleTargetResponse ret = response.Content.ReadAsAsync<AddScheduleTargetResponse>().Result;
                    }
                }
            }
        }

        public void GetRowsByRange(GXDLMSMeterBase device, GXDLMSProfileGeneric pg, DateTime start, DateTime end)
        {
            ListObjects req = new ListObjects();
            req.Targets = TargetType.Object | TargetType.Attribute;
            req.DeviceId = ((GXDevice)device).Id;
            GXDLMSSettings s = new GXDLMSSettings();
            s.UseUtc2NormalTime = device.UtcTimeZone;
            s.Standard = device.Standard;
            GXObject o = new GXObject() { ObjectType = (int)pg.ObjectType, LogicalName = pg.LogicalName };
            o.Attributes.Add(new GXAttribute() { Index = 2 });
            req.Objects = new GXObject[] { o };
            req.Start = start;
            req.End = end;
            using (HttpClient cl = new HttpClient())
            {
                using (HttpResponseMessage response = cl.PostAsJsonAsync(GetServerAddress("/api/Object/ListObjects"), req).Result)
                {
                    Helpers.CheckStatus(response);
                    ListObjectsResponse ret = response.Content.ReadAsAsync<ListObjectsResponse>().Result;
                    KeyValuePair<GXDLMSObject, byte> it = new KeyValuePair<GXDLMSObject, byte>(pg, 2);
                    UpdateValue(s, ret.Items[0], it);
                }
            }
        }

        public void GetRowsByEntry(GXDLMSMeterBase device, GXDLMSProfileGeneric pg, UInt64 index, UInt64 count)
        {
            ListObjects req = new ListObjects();
            req.Targets = TargetType.Object | TargetType.Attribute;
            req.DeviceId = ((GXDevice)device).Id;
            GXDLMSSettings s = new GXDLMSSettings();
            s.UseUtc2NormalTime = device.UtcTimeZone;
            s.Standard = device.Standard;
            GXObject o = new GXObject() { ObjectType = (int)pg.ObjectType, LogicalName = pg.LogicalName };
            o.Attributes.Add(new GXAttribute() { Index = 2 });
            req.Objects = new GXObject[] { o };
            req.Index = index;
            req.Count = count;
            using (HttpClient cl = new HttpClient())
            {
                using (HttpResponseMessage response = cl.PostAsJsonAsync(GetServerAddress("/api/Object/ListObjects"), req).Result)
                {
                    Helpers.CheckStatus(response);
                    ListObjectsResponse ret = response.Content.ReadAsAsync<ListObjectsResponse>().Result;
                    KeyValuePair<GXDLMSObject, byte> it = new KeyValuePair<GXDLMSObject, byte>(pg, 2);
                    UpdateValue(s, ret.Items[0], it);
                }
            }
        }

        /// <summary>
        /// Get next task to execute for given device.
        /// </summary>
        /// <param name="device">Device.</param>
        /// <returns></returns>
        public GXTask[] GetNextTasks(GXDevice device)
        {
            using (HttpClient cl = new HttpClient())
            {
                GetNextTask t = new GetNextTask();
                if (device != null)
                {
                    t.DeviceId = device.Id;
                }
                using (HttpResponseMessage response = cl.PostAsJsonAsync(GetServerAddress("/api/task/GetNextTask"),
                    t).Result)
                {
                    Helpers.CheckStatus(response);
                    GetNextTaskResponse ret = response.Content.ReadAsAsync<GetNextTaskResponse>().Result;
                    return ret.Tasks;
                }
            }
        }

        public void AddTestDevices(GXDevice meter, UInt16 index, UInt16 count)
        {
            if (templates == null)
            {
                templates = GetDeviceTemplates(null);
            }
            using (HttpClient cl = new HttpClient())
            {
                using (HttpResponseMessage response = cl.PostAsJsonAsync(GetServerAddress("/api/Test/AddTestDevice"),
                    new AddTestDevice() { Device = meter, Index = index, Count = count }).Result)
                {
                    Helpers.CheckStatus(response);
                    ListDeviceTemplatesResponse devs = response.Content.ReadAsAsync<ListDeviceTemplatesResponse>().Result;
                }
            }
        }
    }
}