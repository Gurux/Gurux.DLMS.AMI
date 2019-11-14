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
using Gurux.Service.Orm;
using Gurux.DLMS.AMI.Messages.DB;
using Gurux.DLMS.AMI.Messages.Enums;
using Gurux.DLMS.AMI.Messages.Rest;
using Microsoft.AspNetCore.Mvc;

namespace DBService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ErrorController : ControllerBase
    {
        private readonly GXHost host;

        public ErrorController(GXHost value)
        {
            host = value;
        }

        /// <summary>
        /// Get available errors.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<ListErrorsResponse> Get()
        {
            return Post(new ListErrors());
        }

        /// <summary>
        /// Add device Error.
        /// </summary>
        [HttpPost("AddError")]
        public ActionResult<AddErrorResponse> Post(AddError request)
        {
            request.Error.Generation = DateTime.Now;
            lock (host)
            {
                host.Connection.Insert(GXInsertArgs.Insert(request.Error));
            }
            host.SetChange(TargetType.Error, DateTime.Now);
            return new AddErrorResponse();
        }

        /// <summary>
        /// List device Errors
        /// </summary>
        [HttpPost("ListErrors")]
        public ActionResult<ListErrorsResponse> Post(ListErrors request)
        {
            ListErrorsResponse ret = new ListErrorsResponse();
            GXSelectArgs arg = GXSelectArgs.SelectAll<GXError>();
            if (request.Ids != null && request.Ids.Length != 0)
            {
                arg.Where.And<GXError>(q => request.Ids.Contains(q.DeviceId));
            }
            ret.Errors = host.Connection.Select<GXError>(arg).ToArray();
            return ret;
        }

        [HttpPost("ClearErrors")]
        public ActionResult<ClearDeviceErrorResponse> Post(ClearDeviceError request)
        {
            if (request.Ids != null)
            {
                host.Connection.Delete(GXDeleteArgs.Delete<GXError>(q => request.Ids.Contains(q.DeviceId)));
            }
            else
            {
                host.Connection.Delete(GXDeleteArgs.DeleteAll<GXError>());
            }
            host.SetChange(TargetType.Error, DateTime.Now);
            return new ClearDeviceErrorResponse();
        }

    }
}
