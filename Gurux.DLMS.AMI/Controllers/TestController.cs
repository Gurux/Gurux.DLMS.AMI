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
    public class TestController : ControllerBase
    {
        private readonly GXHost host;

        public TestController(GXHost value)
        {
            host = value;
        }

        /// <summary>
        /// Update device.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("AddTestDevice")]
        public ActionResult<AddTestDeviceResponse> Post(AddTestDevice request)
        {
            if (request.Device == null)
            {
                return BadRequest("Device template is NULL.");
            }
            if (request.Device.TemplateId == 0)
            {
                return BadRequest("Device template ID is unknown.");
            }
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
            DateTime now = DateTime.Now;
            //Add new DC.
            request.Device.Generation = now;
            //We are accessing meters using serial number when test mode is used.
            request.Device.HDLCAddressing = ManufacturerSettings.HDLCAddressType.SerialNumber;
            request.Device.LogicalAddress = request.Index;
            for (UInt16 pos = 0; pos != request.Count; ++pos)
            {
                request.Device.Objects = null;
                request.Device.Name = "Test_" + request.Device.LogicalAddress;
                ++request.Device.LogicalAddress;
                request.Device.Id = 0;

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
            }
            host.SetChange(TargetType.Device, DateTime.Now);
            return new AddTestDeviceResponse();
        }
    }
}
