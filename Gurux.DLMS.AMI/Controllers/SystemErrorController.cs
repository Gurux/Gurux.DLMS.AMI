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
using Gurux.Service.Orm;
using Gurux.DLMS.AMI.Messages.DB;
using Gurux.DLMS.AMI.Messages.Enums;
using Gurux.DLMS.AMI.Messages.Rest;
using Microsoft.AspNetCore.Mvc;

namespace DBService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SystemErrorController : ControllerBase
    {
        private readonly GXHost host;

        public SystemErrorController(GXHost value)
        {
            host = value;
        }

        /// <summary>
        /// Get available system errors.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<ListSystemErrorsResponse> Get()
        {
            return Post(new ListSystemErrors());
        }


        /// <summary>
        /// Add system Error.
        /// </summary>
        [HttpPost("AddSystemError")]
        public ActionResult<AddSystemErrorResponse> Post(AddSystemError request)
        {
            request.Error.Generation = DateTime.Now;
            host.Connection.Insert(GXInsertArgs.Insert(request.Error));
            host.SetChange(TargetType.SystemError, DateTime.Now);
            return new AddSystemErrorResponse();
        }

        /// <summary>
        /// Get system errors.
        /// </summary>
        [HttpPost("ListSystemErrors")]
        public ActionResult<ListSystemErrorsResponse> Post(ListSystemErrors request)
        {
            ListSystemErrorsResponse ret = new ListSystemErrorsResponse();
            GXSelectArgs arg = GXSelectArgs.SelectAll<GXSystemError>();
            ret.Errors = host.Connection.Select<GXSystemError>(arg).ToArray();
            return ret;
        }

        [HttpPost("ClearSystemErrors")]
        public ActionResult<ClearSystemErrorResponse> Post(ClearSystemError request)
        {
            host.Connection.Delete(GXDeleteArgs.DeleteAll<GXSystemError>());
            host.SetChange(TargetType.SystemError, DateTime.Now);
            return new ClearSystemErrorResponse();
        }

    }
}
