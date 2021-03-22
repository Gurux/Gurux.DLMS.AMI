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
    public class LogController : ControllerBase
    {
        private readonly CancellationToken _cancellationToken;
        private readonly GXHost host;

        public LogController(GXHost value, IHostApplicationLifetime applicationLifetime)
        {
            host = value;
            _cancellationToken = applicationLifetime.ApplicationStopping;
        }

        /// <summary>
        /// List Device Logs.
        /// </summary>
        [HttpPost("ListDeviceLogs")]

        public ActionResult<ListDeviceLogResponse> Post(ListDeviceLog request)
        {
            ListDeviceLogResponse ret = new ListDeviceLogResponse();
            GXSelectArgs arg = GXSelectArgs.SelectAll<GXDeviceLog>();
            arg.Count = (UInt32)request.Count;
            arg.Descending = true;
            arg.Where.And<GXDeviceLog>(q => request.Ids.Contains(q.Id));
            arg.OrderBy.Add<GXDeviceLog>(q => q.Id);
            arg.OrderBy.Add<GXDeviceLog>(q => q.Generation);
            ret.Logs = host.Connection.Select<GXDeviceLog>(arg).ToArray();
            return ret;
        }

        /// <summary>
        /// New device log is added..
        /// </summary>
        /// <param name="request">Device log item,</param>
        /// <returns></returns>
        [HttpPost("AddDeviceLog")]
        public ActionResult<AddDeviceLogResponse> Post(AddDeviceLog request)
        {
            AddDeviceLogResponse ret = new AddDeviceLogResponse();
            DateTime now = DateTime.Now;
            foreach (var it in request.Logs)
            {
                it.Generation = now;
                host.Connection.Insert(GXInsertArgs.Insert(it));
            }
            host.SetChange(TargetType.DeviceLog, DateTime.Now);
            return ret;
        }

        [HttpPost("ClearDeviceLog")]
        public ActionResult<ClearDeviceLogResponse> Post(ClearDeviceLog request)
        {
            if (request.Ids != null)
            {
                host.Connection.Delete(GXDeleteArgs.Delete<GXError>(q => request.Ids.Contains(q.DeviceId)));
            }
            else
            {
                host.Connection.Delete(GXDeleteArgs.DeleteAll<GXError>());
            }
            host.SetChange(TargetType.DeviceLog, DateTime.Now);
            return new ClearDeviceLogResponse();
        }
    }
}
