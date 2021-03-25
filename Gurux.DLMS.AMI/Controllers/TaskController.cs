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
using System.Threading;
using Gurux.Service.Orm;
using Gurux.DLMS.AMI.Messages.DB;
using Gurux.DLMS.AMI.Messages.Enums;
using Gurux.DLMS.AMI.Messages.Rest;
using Microsoft.AspNetCore.Mvc;
using Gurux.DLMS.AMI;
using Gurux.DLMS.Enums;
using Microsoft.Extensions.Hosting;

namespace DBService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly CancellationToken _cancellationToken;
        private readonly GXHost host;

        public TaskController(GXHost value, IHostApplicationLifetime applicationLifetime)
        {
            host = value;
            _cancellationToken = applicationLifetime.ApplicationStopping;
        }

        /// <summary>
        /// List Tasks
        /// </summary>
        [HttpPost("ListTasks")]

        public ActionResult<ListTasksResponse> Post(ListTasks request)
        {
            ListTasksResponse ret = new ListTasksResponse();
            GXSelectArgs arg = GXSelectArgs.SelectAll<GXTask>();
            arg.Count = (UInt32)request.Count;
            arg.Descending = true;
            if (request.AttributeId != 0)
            {
                arg.Columns.Add<GXDevice>();
                arg.Columns.Add<GXObject>();
                arg.Columns.Add<GXAttribute>();
                arg.Joins.AddInnerJoin<GXTask, GXObject>(a => a.Object, b => b.Id);
                arg.Joins.AddInnerJoin<GXObject, GXDevice>(a => a.DeviceId, b => b.Id);
                arg.Joins.AddInnerJoin<GXObject, GXAttribute>(a => a.Id, b => b.ObjectId);
                arg.Where.And<GXAttribute>(q => q.Id == request.AttributeId);
            }
            else if (request.ObjectId != 0)
            {
                arg.Columns.Add<GXDevice>();
                arg.Columns.Add<GXObject>();
                arg.Columns.Add<GXAttribute>();
                arg.Joins.AddInnerJoin<GXTask, GXObject>(a => a.Object, b => b.Id);
                arg.Joins.AddInnerJoin<GXObject, GXDevice>(a => a.DeviceId, b => b.Id);
                arg.Where.And<GXObject>(q => q.Id == request.ObjectId);
            }
            else if (request.DeviceId != 0)
            {
                arg.Columns.Add<GXDevice>();
                arg.Columns.Add<GXObject>();
                arg.Columns.Add<GXAttribute>();
                arg.Joins.AddInnerJoin<GXTask, GXObject>(a => a.Object, b => b.Id);
                arg.Joins.AddInnerJoin<GXObject, GXDevice>(a => a.DeviceId, b => b.Id);
                arg.Joins.AddInnerJoin<GXObject, GXAttribute>(a => a.Id, b => b.ObjectId);
                arg.Where.And<GXObject>(q => q.DeviceId == request.DeviceId);
            }
            else if ((request.Targets & TargetType.Attribute) != 0)
            {
                arg.Columns.Add<GXDevice>();
                arg.Columns.Add<GXObject>();
                arg.Columns.Add<GXAttribute>();
                arg.Joins.AddInnerJoin<GXTask, GXObject>(a => a.Object, b => b.Id);
                arg.Joins.AddInnerJoin<GXObject, GXDevice>(a => a.DeviceId, b => b.Id);
                arg.Joins.AddInnerJoin<GXObject, GXAttribute>(a => a.Id, b => b.ObjectId);
            }
            else if ((request.Targets & TargetType.Object) != 0)
            {
                arg.Columns.Add<GXDevice>();
                arg.Columns.Add<GXObject>();
                arg.Joins.AddInnerJoin<GXTask, GXObject>(a => a.Object.Id, b => b.Id);
                arg.Joins.AddInnerJoin<GXObject, GXDevice>(a => a.DeviceId, b => b.Id);
            }
            else if ((request.Targets & TargetType.Device) != 0)
            {
                arg.Columns.Add<GXDevice>();
                arg.Joins.AddInnerJoin<GXTask, GXObject>(a => a.Object.Id, b => b.Id);
                arg.Joins.AddInnerJoin<GXObject, GXDevice>(a => a.DeviceId, b => b.Id);
            }
            arg.OrderBy.Add<GXTask>(q => q.Id);
            ret.Tasks = host.Connection.Select<GXTask>(arg).ToArray();
            return ret;
        }

        /// <summary>
        /// New task is added.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("AddTask")]
        public ActionResult<AddTaskResponse> Post(AddTask request)
        {
            AddTaskResponse ret = new AddTaskResponse();
            DateTime now = DateTime.Now;
            foreach (var it in request.Actions)
            {
                if (it.Object.Id == 0)
                {
                    UInt64 dId = it.Object.DeviceId;
                    int ot = it.Object.ObjectType;
                    string ln = it.Object.LogicalName;
                    it.Object = host.Connection.SingleOrDefault<GXObject>(GXSelectArgs.Select<GXObject>(null, q => q.DeviceId == dId && q.ObjectType == ot && q.LogicalName == ln));
                    if (it.Object == null)
                    {
                        return BadRequest(string.Format("Invalid target {0}:{1} DeviceID {2}.", ot, ln, dId));
                    }
                }
                it.Generation = now;
                host.Connection.Insert(GXInsertArgs.Insert(it));
            }
            host.SetChange(TargetType.Tasks, DateTime.Now);
            return ret;
        }

        /// <summary>
        /// Get next task to execute.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("GetNextTask")]
        public ActionResult<GetNextTaskResponse> Post(GetNextTask request)
        {
            GetNextTaskResponse ret = new GetNextTaskResponse();
            lock (host)
            {
                GXSelectArgs arg = GXSelectArgs.Select<GXTask>(c => c.Id, q => q.Start == DateTime.MinValue);
                arg.Joins.AddInnerJoin<GXTask, GXObject>(a => a.Object, b => b.Id);
                arg.Joins.AddInnerJoin<GXObject, GXDevice>(a => a.DeviceId, b => b.Id);
                //Don't get meters that are mapped to other readers.
                arg.Joins.AddLeftJoin<GXDevice, GXDeviceToReader>(a => a.Id, b => b.DeviceId);
                if (request.DeviceId != 0)
                {
                    arg.Where.And<GXDevice>(q => q.Id == request.DeviceId);
                }
                if (!request.Listener)
                {
                    arg.Where.And<GXDevice>(q => q.Dynamic == false);
                }
                arg.OrderBy.Add<GXTask>(q => q.Id);
                GXSelectArgs onProgress = GXSelectArgs.Select<GXObject>(c => c.DeviceId, q => q.Removed == DateTime.MinValue);
                onProgress.Joins.AddInnerJoin<GXTask, GXObject>(a => a.Object, b => b.Id);
                onProgress.Where.And<GXTask>(q => q.Start != DateTime.MinValue && q.End == DateTime.MinValue);
                arg.Where.And<GXObject>(q => !GXSql.Exists<GXObject, GXDevice>(a => a.DeviceId, b => b.Id, onProgress));
                GXTask task = host.Connection.SingleOrDefault<GXTask>(arg);
                if (task != null)
                {
                    //Get task device ID and creation time.
                    arg = GXSelectArgs.SelectAll<GXTask>(q => q.Id == task.Id);
                    arg.Columns.Add<GXObject>();
                    arg.Joins.AddInnerJoin<GXTask, GXObject>(a => a.Object, b => b.Id);
                    arg.Joins.AddInnerJoin<GXObject, GXDevice>(a => a.DeviceId, b => b.Id);
                    task = host.Connection.SingleOrDefault<GXTask>(arg);
                    //Get all tasks that are created at the same time for same meter.
                    arg = GXSelectArgs.SelectAll<GXTask>(q => q.Generation == task.Generation);
                    arg.Where.And<GXDevice>(q => q.Id == task.Object.DeviceId);
                    arg.Where.And<GXObject>(q => q.Removed == DateTime.MinValue);
                    arg.Columns.Add<GXObject>();
                    arg.Columns.Add<GXDevice>();
                    arg.Columns.Add<GXAttribute>();
                    arg.Joins.AddInnerJoin<GXTask, GXObject>(a => a.Object, b => b.Id);
                    arg.Joins.AddInnerJoin<GXObject, GXDevice>(a => a.DeviceId, b => b.Id);
                    arg.Joins.AddInnerJoin<GXObject, GXAttribute>(a => a.Id, b => b.ObjectId);
                    ret.Tasks = host.Connection.Select<GXTask>(arg).ToArray();
                    DateTime now = DateTime.Now;
                    foreach (GXTask it in ret.Tasks)
                    {
                        it.Start = now;
                        host.Connection.Update(GXUpdateArgs.Update(it, q => q.Start));
                    }
                    host.SetChange(TargetType.Tasks, DateTime.Now);
                }
            }
            return ret;
        }

        /// <summary>
        /// Task is executed.
        /// </summary>
        [HttpPost("TaskReady")]
        public ActionResult<TaskReadyResponse> Post(TaskReady request)
        {
            if (request.Tasks != null)
            {
                foreach (GXTask it in request.Tasks)
                {
                    it.End = DateTime.Now;
                    if (it.TaskType == TaskType.Read)
                    {
                        host.Connection.Update(GXUpdateArgs.Update(it, q => new { q.End, q.Result, q.Data }));
                    }
                    else
                    {
                        host.Connection.Update(GXUpdateArgs.Update(it, q => new { q.End, q.Result }));
                    }
                }
                host.Connection.Update(GXUpdateArgs.UpdateRange(request.Tasks, q => new { q.End, q.Result }));
                host.SetChange(TargetType.Tasks, DateTime.Now);
            }
            return new TaskReadyResponse();
        }

        /// <summary>
        /// Task is executed.
        /// </summary>
        [HttpPost("WaitChange")]
        public ActionResult<WaitChangeResponse> Post(WaitChange request)
        {
            DateTime when;
            TargetType changed = host.GetChange(request.Change, request.Time, out when);
            if (changed == TargetType.None)
            {
                //Wait until there is a new value.
                AutoResetEvent h = new AutoResetEvent(false);
                try
                {
                    if (host.WaitChange(request.Change, h, _cancellationToken.WaitHandle, request.WaitTime))
                    {
                        changed = host.GetChange(request.Change, request.Time, out when);
                    }
                    else
                    {
                        //If timeout.
                        changed = TargetType.None;
                        when = request.Time;
                    }
                }
                finally
                {
                    host.CancelChange(h);
                }
            }
            WaitChangeResponse ret = new WaitChangeResponse();
            ret.Change = changed;
            ret.Time = when;
            return ret;
        }

        [HttpPost("TaskDelete")]
        public ActionResult<DeleteTaskResponse> Post(DeleteTask request)
        {
            if (request.Ids == null)
            {
                return BadRequest("Task list is null.");
            }
            host.Connection.Delete(GXDeleteArgs.Delete<GXTask>(q => request.Ids.Contains(q.Id)));
            host.SetChange(TargetType.Tasks, DateTime.Now);
            return new DeleteTaskResponse();
        }
    }
}
