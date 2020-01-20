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
using Gurux.Service.Orm;
using Gurux.DLMS.AMI.Messages.DB;
using Gurux.DLMS.AMI.Messages.Enums;
using Gurux.DLMS.AMI.Messages.Rest;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Gurux.DLMS.AMI.Internal;

namespace DBService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScheduleController : ControllerBase
    {
        private readonly GXHost host;

        public ScheduleController(GXHost value)
        {
            host = value;
        }

        /// <summary>
        /// List Schedules.
        /// </summary>
        [HttpPost("ListSchedules")]
        public ActionResult<ListSchedulesResponse> Post(ListSchedules request)
        {
            ListSchedulesResponse ret = new ListSchedulesResponse();
            GXSelectArgs arg = GXSelectArgs.SelectAll<GXSchedule>();
            arg.Distinct = true;
            arg.Where.And<GXSchedule>(q => q.Removed == DateTime.MinValue);
            if (request.ObjectId != 0)
            {
                arg.Joins.AddInnerJoin<GXSchedule, GXScheduleToAttribute>(s => s.Id, o => o.ScheduleId);
                arg.Joins.AddInnerJoin<GXScheduleToAttribute, GXAttribute>(s => s.AttributeId, o => o.Id);
                arg.Where.And<GXAttribute>(q => q.ObjectId == request.ObjectId);
            }
            else if (request.DeviceId != 0)
            {
                arg.Joins.AddInnerJoin<GXSchedule, GXScheduleToAttribute>(s => s.Id, o => o.ScheduleId);
                arg.Joins.AddInnerJoin<GXScheduleToAttribute, GXAttribute>(s => s.AttributeId, o => o.Id);
                arg.Joins.AddInnerJoin<GXAttribute, GXObject>(s => s.ObjectId, o => o.Id);
                arg.Where.And<GXObject>(q => q.DeviceId == request.DeviceId);
            }
            /*
            if ((request.Targets & TargetType.Attribute) != 0)
            {
                arg.Columns.Add<GXObject>();
                arg.Columns.Add<GXScheduleToAttribute>();
                arg.Joins.AddLeftJoin<GXSchedule, GXScheduleToAttribute>(s => s.Id, o => o.ScheduleId);
                arg.Joins.AddLeftJoin<GXScheduleToAttribute, GXAttribute>(s => s.AttributeId, o => o.Id);
                arg.Joins.AddInnerJoin<GXAttribute, GXObject>(s => s.ObjectId, o => o.Id);
                arg.Where.And<GXObject>(q => q.Removed == DateTime.MinValue);
                arg.Where.And<GXAttribute>(q => q.Removed == DateTime.MinValue);
            }
            */
            ret.Schedules = host.Connection.Select<GXSchedule>(arg).ToArray();
            foreach (GXSchedule it in ret.Schedules)
            {
                arg = GXSelectArgs.SelectAll<GXObject>();
                arg.Columns.Add<GXAttribute>();
                arg.Distinct = true;
                arg.Joins.AddInnerJoin<GXSchedule, GXScheduleToAttribute>(s => s.Id, o => o.ScheduleId);
                arg.Joins.AddInnerJoin<GXScheduleToAttribute, GXAttribute>(s => s.AttributeId, o => o.Id);
                arg.Joins.AddInnerJoin<GXAttribute, GXObject>(s => s.ObjectId, o => o.Id);
                arg.Where.And<GXObject>(q => q.Removed == DateTime.MinValue);
                arg.Where.And<GXAttribute>(q => q.Removed == DateTime.MinValue);
                arg.Where.And<GXSchedule>(q => q.Id == it.Id);
                it.Objects = host.Connection.Select<GXObject>(arg);
            }
            return ret;
        }

        /// <summary>
        /// Update Schedule.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("UpdateSchedule")]
        public ActionResult<UpdateScheduleResponse> Post(UpdateSchedule request)
        {
            UpdateScheduleResponse ret = new UpdateScheduleResponse();
            List<UInt64> list = new List<UInt64>();
            List<GXScheduleToAttribute> list2 = new List<GXScheduleToAttribute>();
            foreach (var it in request.Schedules)
            {
                List<GXObject> tmp = it.Objects;
                it.Objects = null;
                if (it.Id == 0)
                {
                    host.Connection.Insert(GXInsertArgs.Insert(it));
                    list.Add(it.Id);
                    if (tmp != null)
                    {
                        foreach (GXObject o in tmp)
                        {
                            if (o.Attributes != null)
                            {
                                foreach (GXAttribute a in o.Attributes)
                                {
                                    list2.Add(new GXScheduleToAttribute() { ScheduleId = it.Id, AttributeId = a.Id });
                                }
                            }
                        }
                        host.Connection.Insert(GXInsertArgs.InsertRange(list2));
                    }
                }
                else
                {
                    host.Connection.Update(GXUpdateArgs.Update(it));
                }
            }
            ret.ScheduleIds = list.ToArray();
            host.SetChange(TargetType.Schedule, DateTime.Now);
            return ret;
        }


        /// <summary>
        /// Update Schedule execution time.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("UpdateScheduleExecutionTime")]
        public ActionResult<UpdateScheduleExecutionTimeResponse> Post(UpdateScheduleExecutionTime request)
        {
            GXSchedule s = host.Connection.SelectById<GXSchedule>(request.Id);
            s.ExecutionTime = request.Time;
            host.Connection.Update(GXUpdateArgs.Update(s, c => c.ExecutionTime));
            host.SetChange(TargetType.Schedule, DateTime.Now);
            return new UpdateScheduleExecutionTimeResponse();
        }

        [HttpPost("DeleteSchedule")]
        public ActionResult<DeleteScheduleResponse> Post(DeleteSchedule request)
        {
            //Remove mapping.
            if (request.AttributeIds != null && request.ScheduleIds != null)
            {
                for (int pos = 0; pos != request.AttributeIds.Length; ++pos)
                {
                    GXDeleteArgs arg = GXDeleteArgs.Delete<GXScheduleToAttribute>(q => request.ScheduleIds[pos] == q.ScheduleId);
                    arg.Where.And<GXScheduleToAttribute>(q => request.AttributeIds[pos] == q.AttributeId);
                    host.Connection.Delete(arg);
                }
                host.SetChange(TargetType.Schedule, DateTime.Now);
            }
            else if (request.ScheduleIds != null)
            {
                GXSelectArgs arg = GXSelectArgs.Select<GXSchedule>(a => a.Id, q => request.ScheduleIds.Contains(q.Id));
                List<GXSchedule> list = host.Connection.Select<GXSchedule>(arg);
                DateTime now = DateTime.Now;
                foreach (GXSchedule it in list)
                {
                    it.Removed = now;
                    host.Connection.Update(GXUpdateArgs.Update(it, q => q.Removed));
                }
                host.SetChange(TargetType.Schedule, now);
            }
            return new DeleteScheduleResponse();
        }

        /// <summary>
        /// Update Schedule.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("AddScheduleTarget")]
        public ActionResult<AddScheduleTargetResponse> Post(AddScheduleTarget request)
        {
            AddScheduleTargetResponse ret = new AddScheduleTargetResponse();
            List<GXScheduleToAttribute> list = new List<GXScheduleToAttribute>();
            foreach (UInt64 s in request.Schedules)
            {
                if (request.DeviceId != 0)
                {
                    ListObjects req = new ListObjects();
                    req.DeviceId = request.DeviceId;
                    req.Targets = TargetType.Object | TargetType.Attribute;
                    req.Objects = request.Objects;
                    using (System.Net.Http.HttpClient cl = new System.Net.Http.HttpClient())
                    {
                        using (System.Net.Http.HttpResponseMessage response = cl.PostAsJsonAsync(this.Request.Scheme + "://" + this.Request.Host + "/api/object/ListObjects", req).Result)
                        {
                            Helpers.CheckStatus(response);
                            ListObjectsResponse objs = response.Content.ReadAsAsync<ListObjectsResponse>().Result;
                            foreach (GXObject obj in objs.Items)
                            {
                                if (obj.Attributes != null)
                                {
                                    foreach (GXAttribute a in obj.Attributes)
                                    {
                                        list.Add(new GXScheduleToAttribute() { ScheduleId = s, AttributeId = a.Id });
                                    }
                                }
                            }
                        }
                    }
                }
                else if (request.Objects != null && request.Objects.Length != 0)
                {
                    foreach (GXObject o in request.Objects)
                    {
                        foreach (GXAttribute a in o.Attributes)
                        {
                            list.Add(new GXScheduleToAttribute() { ScheduleId = s, AttributeId = a.Id });
                        }
                    }
                }
            }
            host.Connection.Insert(GXInsertArgs.InsertRange(list));
            host.SetChange(TargetType.Schedule, DateTime.Now);
            return ret;
        }

        [HttpPost("DeleteScheduleTarget")]
        public ActionResult<DeleteScheduleTargetResponse> Post(DeleteScheduleTarget request)
        {
            List<GXScheduleToAttribute> list = new List<GXScheduleToAttribute>();
            foreach (UInt64 s in request.Schedules)
            {
                foreach (UInt64 o in request.Attributes)
                {
                    list.Add(new GXScheduleToAttribute() { ScheduleId = s, AttributeId = o });
                }
            }
            host.Connection.Delete(GXDeleteArgs.DeleteRange(list));
            host.SetChange(TargetType.Schedule, DateTime.Now);
            return new DeleteScheduleTargetResponse();
        }
    }
}
