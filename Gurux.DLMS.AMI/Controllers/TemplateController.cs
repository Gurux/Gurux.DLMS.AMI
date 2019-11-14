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
using Gurux.DLMS;
using Gurux.DLMS.AMI.Messages.DB;
using Gurux.DLMS.AMI.Messages.Enums;
using Gurux.DLMS.AMI.Messages.Rest;
using Microsoft.AspNetCore.Mvc;

namespace DBService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TemplateController : ControllerBase
    {
        private readonly GXHost host;

        public TemplateController(GXHost value)
        {
            host = value;
        }

        /// <summary>
        /// Update device template.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("AddTemplate")]
        public ActionResult<UpdateDeviceTemplateResponse> Post(UpdateDeviceTemplate request)
        {
            GXDeviceTemplate dev = request.Device;
            if (dev == null)
            {
                return BadRequest("Device is null.");
            }
            bool newDevice = dev.Id == 0;
            if (dev.DeviceSystemTitle != null)
            {
                dev.DeviceSystemTitle = dev.DeviceSystemTitle.Replace(" ", "");
            }
            if (dev.AuthenticationKey != null)
            {
                dev.AuthenticationKey = dev.AuthenticationKey.Replace(" ", "");
            }
            if (dev.BlockCipherKey != null)
            {
                dev.BlockCipherKey = dev.BlockCipherKey.Replace(" ", "");
            }
            dev.Generation = DateTime.Now;
            if (newDevice)
            {
                //Add new DC.
                List<GXObjectTemplate> tmp = dev.Objects;
                dev.Objects = null;
                DateTime now = DateTime.Now;
                dev.Generation = now;
                host.Connection.Insert(GXInsertArgs.Insert(dev));
                if (tmp != null)
                {
                    //Add default objects.
                    foreach (GXObjectTemplate it in tmp)
                    {
                        List<GXAttributeTemplate> tmp2 = it.Attributes;
                        it.Attributes = null;
                        it.DeviceTemplateId = dev.Id;
                        it.Generation = now;
                        host.Connection.Insert(GXInsertArgs.Insert(it));
                        if (it.Attributes != tmp2)
                        {
                            foreach (GXAttributeTemplate a in tmp2)
                            {
                                a.Generation = now;
                                a.ObjectTemplateId = it.Id;
                            }
                            host.Connection.Insert(GXInsertArgs.InsertRange(tmp2));
                        }
                    }
                }
                host.SetChange(TargetType.Object, DateTime.Now);
            }
            else
            {
                host.Connection.Update(GXUpdateArgs.Update(dev));
                host.SetChange(TargetType.DeviceTemplate, DateTime.Now);
            }
            return new UpdateDeviceTemplateResponse()
            {
                DeviceId = dev.Id
            };
        }


        /// <summary>
        /// Get available device templates.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<ListDeviceTemplatesResponse> Get()
        {
            return Post(new ListDeviceTemplates());
        }

        /// <summary>
        /// Get available device templates.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("ListDeviceTemplates")]
        public ActionResult<ListDeviceTemplatesResponse> Post(ListDeviceTemplates request)
        {
            ListDeviceTemplatesResponse ret = new ListDeviceTemplatesResponse();
            GXSelectArgs arg = GXSelectArgs.SelectAll<GXDeviceTemplate>();
            arg.Where.And<GXDeviceTemplate>(q => q.Removed == DateTime.MinValue);
            if (!string.IsNullOrEmpty(request.Name))
            {
                string name = request.Name;
                arg.Where.And<GXDeviceTemplate>(q => q.Name.Contains(name));
            }
            else if (request.Ids != null)
            {
                arg.Where.And<GXDeviceTemplate>(q => request.Ids.Contains(q.Id));
            }
            /*
            if ((request.Targets & TargetType.AttributeTemplate) != 0)
            {
                arg.Columns.Add<GXObjectTemplate>();
                arg.Columns.Add<GXAttributeTemplate>();

                arg.Joins.AddLeftJoin<GXDeviceTemplate, GXObjectTemplate>(d => d.Id, o => o.DeviceTemplateId);
                arg.Joins.AddLeftJoin<GXObjectTemplate, GXAttributeTemplate>(o => o.Id, a => a.ObjectTemplateId);
                arg.Where.And<GXObjectTemplate>(q => q.Removed == DateTime.MinValue);
                arg.Where.And<GXAttributeTemplate>(q => q.Removed == DateTime.MinValue);
            }
            else if ((request.Targets & TargetType.ObjectTemplate) != 0)
            {
                arg.Columns.Add<GXObjectTemplate>();
                arg.Joins.AddLeftJoin<GXDeviceTemplate, GXObjectTemplate>(d => d.Id, o => o.DeviceTemplateId);
                arg.Where.And<GXObjectTemplate>(q => q.Removed == DateTime.MinValue);
            }
            */
            ret.Devices = host.Connection.Select<GXDeviceTemplate>(arg).ToArray();

            if ((request.Targets & TargetType.AttributeTemplate) != 0)
            {
                foreach (GXDeviceTemplate it in ret.Devices)
                {
                    arg = GXSelectArgs.SelectAll<GXObjectTemplate>();
                    arg.Where.And<GXObjectTemplate>(q => q.DeviceTemplateId == it.Id);
                    arg.Where.And<GXObjectTemplate>(q => q.Removed == DateTime.MinValue);
                    it.Objects = host.Connection.Select<GXObjectTemplate>(arg);
                    if (it.Objects != null && (request.Targets & TargetType.ObjectTemplate) != 0)
                    {
                        foreach (GXObjectTemplate o in it.Objects)
                        {
                            arg = GXSelectArgs.SelectAll<GXAttributeTemplate>();
                            arg.Where.And<GXAttributeTemplate>(q => q.ObjectTemplateId == o.Id);
                            arg.Where.And<GXAttributeTemplate>(q => q.Removed == DateTime.MinValue);
                            o.Attributes = host.Connection.Select<GXAttributeTemplate>(arg);
                        }
                    }
                }
            }
            return ret;
        }

        /// <summary>
        /// Remove selected device template.
        /// </summary>
        [HttpPost("DeviceTemplateDelete")]
        public ActionResult<DeviceTemplateDeleteResponse> Post(DeviceTemplateDelete request)
        {
            if (request.Ids != null)
            {
                GXSelectArgs arg = GXSelectArgs.Select<GXDeviceTemplate>(a => a.Id, q => request.Ids.Contains(q.Id));
                List<GXDeviceTemplate> list = host.Connection.Select<GXDeviceTemplate>(arg);
                if (list.Count != 0)
                {
                    DateTime now = DateTime.Now;
                    foreach (GXDeviceTemplate it in list)
                    {
                        it.Removed = now;
                        host.Connection.Update(GXUpdateArgs.Update(it, q => q.Removed));
                    }
                    host.SetChange(TargetType.DeviceTemplate, DateTime.Now);
                }
            }
            return new DeviceTemplateDeleteResponse();
        }

        /// <summary>
        /// Remove selected object template.
        /// </summary>
        [HttpPost("ObjectTemplateDelete")]
        public ActionResult<ObjectTemplateDeleteResponse> Post(ObjectTemplateDelete request)
        {
            if (request.Ids != null)
            {
                GXSelectArgs arg = GXSelectArgs.Select<GXObjectTemplate>(a => a.Id, q => request.Ids.Contains(q.Id));
                List<GXObjectTemplate> list = host.Connection.Select<GXObjectTemplate>(arg);
                if (list.Count != 0)
                {
                    DateTime now = DateTime.Now;
                    foreach (GXObjectTemplate it in list)
                    {
                        it.Removed = now;
                        host.Connection.Update(GXUpdateArgs.Update(it, q => q.Removed));
                    }
                    host.SetChange(TargetType.ObjectTemplate, DateTime.Now);
                }
            }
            return new ObjectTemplateDeleteResponse();
        }

        /// <summary>
        /// Remove selected attribute template.
        /// </summary>
        [HttpPost("AttributeTemplateDelete")]
        public ActionResult<AttributeTemplateDeleteResponse> Post(AttributeTemplateDelete request)
        {
            if (request.Ids != null)
            {
                GXSelectArgs arg = GXSelectArgs.Select<GXAttributeTemplate>(a => a.Id, q => request.Ids.Contains(q.Id));
                List<GXAttributeTemplate> list = host.Connection.Select<GXAttributeTemplate>(arg);
                if (list.Count != 0)
                {
                    DateTime now = DateTime.Now;
                    foreach (GXAttributeTemplate it in list)
                    {
                        it.Removed = now;
                        host.Connection.Update(GXUpdateArgs.Update(it, q => q.Removed));
                    }
                    host.SetChange(TargetType.AttributeTemplate, DateTime.Now);
                }
            }
            return new AttributeTemplateDeleteResponse();
        }
    }
}
