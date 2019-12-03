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
using System.Linq;
using Gurux.Common;
using Gurux.Service.Orm;
using Gurux.DLMS.AMI.Messages.DB;
using Gurux.DLMS.AMI.Messages.Enums;
using Gurux.DLMS.AMI.Messages.Rest;
using Microsoft.AspNetCore.Mvc;

namespace Gurux.DLMS.AMI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeviceController : ControllerBase
    {
        private readonly GXHost host;

        public DeviceController(GXHost value)
        {
            host = value;
        }

        /// <summary>
        /// Update device.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("UpdateDevice")]
        public ActionResult<UpdateDeviceResponse> Post(UpdateDevice request)
        {
            if (request.Device.TemplateId == 0)
            {
                return BadRequest("Device template ID is unknown.");
            }
            bool newDevice = request.Device.Id == 0;
            if (request.Device.DeviceSystemTitle != null)
            {
                request.Device.DeviceSystemTitle = request.Device.DeviceSystemTitle.Replace(" ", "");
            }
            if (request.Device.AuthenticationKey != null)
            {
                request.Device.AuthenticationKey = request.Device.AuthenticationKey.Replace(" ", "");
            }
            if (request.Device.BlockCipherKey != null)
            {
                request.Device.BlockCipherKey = request.Device.BlockCipherKey.Replace(" ", "");
            }
            if (newDevice)
            {
                DateTime now = DateTime.Now;
                request.Device.Generation = now;
                //Add new DC.
                List<GXObject> tmp = request.Device.Objects;
                request.Device.Objects = null;
                request.Device.Generation = now;
                host.Connection.Insert(GXInsertArgs.Insert(request.Device));

                //Add default objects.
                GXSelectArgs arg = GXSelectArgs.SelectAll<GXObjectTemplate>(q => q.DeviceTemplateId == request.Device.TemplateId);
                arg.Columns.Add<GXAttributeTemplate>();
                arg.Joins.AddLeftJoin<GXObjectTemplate, GXAttributeTemplate>(o => o.Id, a => a.ObjectTemplateId);
                List<GXObjectTemplate> l = host.Connection.Select<GXObjectTemplate>(arg);
                foreach (GXObjectTemplate it in l)
                {
                    GXObject obj = new GXObject()
                    {
                        TemplateId = it.Id,
                        Generation = now,
                        DeviceId = request.Device.Id,
                        ObjectType = it.ObjectType,
                        Name = it.Name,
                        LogicalName = it.LogicalName,
                        ShortName = it.ShortName,
                    };
                    host.Connection.Insert(GXInsertArgs.Insert(obj));
                    foreach (GXAttributeTemplate ait in it.Attributes)
                    {
                        GXAttribute a = new GXAttribute();
                        a.ObjectId = obj.Id;
                        a.Index = ait.Index;
                        a.TemplateId = ait.Id;
                        a.AccessLevel = ait.AccessLevel;
                        a.DataType = ait.DataType;
                        a.UIDataType = ait.UIDataType;
                        a.Generation = now;
                        a.ExpirationTime = ait.ExpirationTime;
                        obj.Attributes.Add(a);
                    };
                    host.Connection.Insert(GXInsertArgs.InsertRange(obj.Attributes));
                }
                host.SetChange(TargetType.ObjectTemplate, DateTime.Now);
            }
            else
            {
                request.Device.Updated = DateTime.Now;
                host.Connection.Update(GXUpdateArgs.Update(request.Device));
                host.SetChange(TargetType.DeviceTemplate, DateTime.Now);
            }
            return new UpdateDeviceResponse()
            {
                DeviceId = request.Device.Id
            };
        }


        /// <summary>
        /// Get available devices.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<ListDevicesResponse> Get()
        {
            return Post(new ListDevices());
        }

        /// <summary>
        /// Get available devices.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("ListDevices")]
        public ActionResult<ListDevicesResponse> Post(ListDevices request)
        {
            ListDevicesResponse ret = new ListDevicesResponse();
            GXSelectArgs arg = GXSelectArgs.SelectAll<GXDevice>();
            arg.Where.And<GXDevice>(q => q.Removed == DateTime.MinValue);

            if ((request.Targets & TargetType.Attribute) != 0)
            {
                arg.Columns.Add<GXObject>();
                arg.Columns.Add<GXAttribute>();

                arg.Joins.AddLeftJoin<GXDevice, GXObject>(d => d.Id, o => o.DeviceId);
                arg.Joins.AddLeftJoin<GXObject, GXAttribute>(o => o.Id, a => a.ObjectId);
                arg.Where.And<GXObject>(q => q.Removed == DateTime.MinValue);
                arg.Where.And<GXAttribute>(q => q.Removed == DateTime.MinValue);
            }
            else if ((request.Targets & TargetType.Object) != 0)
            {
                arg.Columns.Add<GXObject>();
                arg.Joins.AddLeftJoin<GXDevice, GXObject>(d => d.Id, o => o.DeviceId);
                arg.Where.And<GXObject>(q => q.Removed == DateTime.MinValue);
            }

            byte[] st = GXCommon.HexToBytes(request.SystemTitle);
            if (st.Length == 8)
            {
                string str = GXCommon.ToHex(st, false);
                arg.Where.And<GXDevice>(q => q.DeviceSystemTitle.Equals(str));
            }
            else if (!string.IsNullOrEmpty(request.Name))
            {
                string name = request.Name;
                arg.Where.And<GXDevice>(q => q.Name.Contains(name));
            }
            else if (request.Ids != null)
            {
                arg.Where.And<GXDevice>(q => request.Ids.Contains(q.Id));
            }
            ret.Devices = host.Connection.Select<GXDevice>(arg).ToArray();
            return ret;
        }

        /// <summary>
        /// Remove selected device
        /// </summary>
        [HttpPost("DeviceDelete")]
        public ActionResult<DeviceDeleteResponse> Post(DeviceDelete request)
        {
            if (request.Ids != null)
            {
                GXSelectArgs arg = GXSelectArgs.Select<GXDevice>(a => a.Id, q => request.Ids.Contains(q.Id));
                List<GXDevice> list = host.Connection.Select<GXDevice>(arg);
                if (list.Count != 0)
                {
                    DateTime now = DateTime.Now;
                    foreach (GXDevice it in list)
                    {
                        it.Removed = now;
                        host.Connection.Update(GXUpdateArgs.Update(it, q => q.Removed));
                    }
                    host.SetChange(TargetType.Device, now);
                }
            }
            return new DeviceDeleteResponse();
        }
    }
}
