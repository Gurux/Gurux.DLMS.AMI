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
using Gurux.Service.Orm;
using Gurux.DLMS.AMI.Messages.DB;
using Gurux.DLMS.AMI.Messages.Enums;
using Gurux.DLMS.AMI.Messages.Rest;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using Gurux.DLMS.Enums;

namespace DBService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ObjectController : ControllerBase
    {
        private readonly GXHost host;

        public ObjectController(GXHost value)
        {
            host = value;
        }

        /// <summary>
        /// Add COSEM objects
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("AddObject")]
        public ActionResult<AddObjectResponse> Post(AddObject request)
        {
            AddObjectResponse ret = new AddObjectResponse();
            foreach (var it in request.Items)
            {
                it.Generation = DateTime.Now;
            }
            host.Connection.Insert(GXInsertArgs.InsertRange(request.Items));
            host.SetChange(TargetType.Object, DateTime.Now);
            ret.Ids = new UInt64[request.Items.Length];
            int pos = 0;
            foreach (var it in request.Items)
            {
                ret.Ids[pos] = it.Id;
                ++pos;
            }
            return ret;
        }


        /// <summary>
        /// Get available errors.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<ListObjectsResponse> Get()
        {
            return Post(new ListObjects());
        }

        /// <summary>
        /// List COSEM objects
        /// </summary>
        [HttpPost("ListObjects")]
        public ActionResult<ListObjectsResponse> Post(ListObjects request)
        {
            ListObjectsResponse ret = new ListObjectsResponse();
            GXSelectArgs arg = GXSelectArgs.Select<GXObject>(null);
            arg.Columns.Add<GXAttribute>();
            arg.Joins.AddInnerJoin<GXObject, GXAttribute>(a => a.Id, b => b.ObjectId);
            if (request.Objects != null)
            {
                List<int> indexes = new List<int>();
                foreach (GXObject it in request.Objects)
                {
                    if (it.Id != 0)
                    {
                        arg.Where.And<GXObject>(q => q.Id == it.Id);
                    }
                    else if (it.ObjectType != 0 && !string.IsNullOrEmpty(it.LogicalName))
                    {
                        if (it.Attributes != null)
                        {
                            foreach (GXAttribute a in it.Attributes)
                            {
                                if (a.Index != 0)
                                {
                                    indexes.Add(a.Index);
                                }
                            }
                        }
                        arg.Where.Or<GXObject>(q => q.Removed == DateTime.MinValue && q.ObjectType == it.ObjectType && q.LogicalName.Equals(it.LogicalName));
                    }
                }
                arg.Where.And<GXAttribute>(q => q.Removed == DateTime.MinValue);
                GXSelectArgs devices = GXSelectArgs.Select<GXObject>(q => q.DeviceId);
                if (request.DeviceId != 0)
                {
                    devices.Where.And<GXObject>(q => q.DeviceId == request.DeviceId);
                }
                arg.Where.And<GXObject>(q => GXSql.In(q.DeviceId, devices));
                ret.Items = host.Connection.Select<GXObject>(arg).ToArray();
                //Delete Removed attributes.
                foreach (GXObject o in ret.Items)
                {
                    List<GXAttribute> list = new List<GXAttribute>();
                    for (int pos = 0; pos < o.Attributes.Count; ++pos)
                    {
                        if (o.Attributes[pos].Removed != DateTime.MinValue || (indexes.Count != 0 && !indexes.Contains(o.Attributes[pos].Index)))
                        {
                            o.Attributes.RemoveAt(pos);
                            --pos;
                        }
                    }
                }
            }
            else if (request.DeviceId != 0)
            {
                arg.Where.And<GXObject>(q => q.DeviceId == request.DeviceId);
                arg.Where.And<GXAttribute>(q => q.Removed == DateTime.MinValue);
                if ((request.Targets & TargetType.Attribute) != 0)
                {
                    if (request.Index == 0 && request.Count == 0 && request.Start == DateTime.MinValue && request.End == DateTime.MinValue)
                    {
                        arg.Columns.Add<GXValue>();
                        arg.Joins.AddLeftJoin<GXAttribute, GXValue>(a => a.Id, v => v.AttributeId);
                    }
                }
                ret.Items = host.Connection.Select<GXObject>(arg).ToArray();
            }
            foreach (GXObject obj in ret.Items)
            {
                if (obj.ObjectType == (int)ObjectType.ProfileGeneric)
                {
                    foreach (GXAttribute a in obj.Attributes)
                    {
                        if (a.Index == 2)
                        {
                            arg = GXSelectArgs.SelectAll<GXValue>();
                            arg.Where.And<GXValue>(q => q.AttributeId == a.Id);
                            if (request.Index != 0)
                            {
                                arg.Index = (UInt32)(request.Index - 1);
                            }
                            arg.Count = (UInt32)request.Count;
                            if (request.Start != DateTime.MinValue)
                            {
                                arg.Where.And<GXValue>(q => q.Read >= request.Start);
                            }
                            if (request.End != DateTime.MinValue)
                            {
                                arg.Where.And<GXValue>(q => q.Read <= request.End);
                            }
                            arg.OrderBy.Add<GXValue>(o => o.Read);
                            List<GXValue> values = host.Connection.Select<GXValue>(arg);
                            StringBuilder sb = new StringBuilder();
                            sb.Append("<Array>");
                            foreach (GXValue v in values)
                            {
                                sb.Append(v.Value);
                            }
                            sb.Append("</Array>");
                            a.Value = sb.ToString();
                        }
                    }
                }
            }
            return ret;
        }

        /// <summary>
        /// Remove selected objects
        /// </summary>
        [HttpPost("ObjectDelete")]
        public ActionResult<ObjectDeleteResponse> Post(ObjectDelete request)
        {
            if (request.Ids != null)
            {
                GXSelectArgs arg = GXSelectArgs.Select<GXObject>(a => a.Id, q => request.Ids.Contains(q.Id));
                List<GXObject> list = host.Connection.Select<GXObject>(arg);
                if (list.Count != 0)
                {
                    foreach (GXObject it in list)
                    {
                        it.Removed = DateTime.Now;
                    }
                    host.Connection.Update(GXUpdateArgs.UpdateRange(list, q => q.Removed));
                    host.SetChange(TargetType.Object, DateTime.Now);
                }
            }
            return new ObjectDeleteResponse();
        }

        /// <summary>
        /// Update data type of the attribute.
        /// </summary>
        [HttpPost("UpdateDatatype")]
        public ActionResult<UpdateDatatypeResponse> Post(UpdateDatatype request)
        {
            if (request.Items != null && request.Items.Length != 0)
            {
                DateTime now = DateTime.Now;
                foreach (GXAttribute it in request.Items)
                {
                    it.Updated = now;
                }
                host.Connection.Update(GXUpdateArgs.UpdateRange(request.Items, c => new { c.DataType, c.Updated }));
                host.SetChange(TargetType.Object, now);
            }
            return new UpdateDatatypeResponse();
        }
    }
}
