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
using System.Linq;

namespace DBService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValueController : ControllerBase
    {
        private readonly GXHost host;

        public ValueController(GXHost value)
        {
            host = value;
        }

        /// <summary>
        /// Get concentrator value.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("ListValues")]
        public ActionResult<ListValuesResponse> Post(ListValues request)
        {
            ListValuesResponse ret = new ListValuesResponse();
            GXSelectArgs arg = GXSelectArgs.SelectAll<GXValue>();
            if (request.Ids != null && request.Ids.Length != 0)
            {
                arg.Where.And<GXValue>(q => request.Ids.Contains(q.AttributeId));
            }
            ret.Items = host.Connection.Select<GXValue>(arg).ToArray();
            return ret;
        }

        public static bool IsNumber(object value)
        {
            return value is sbyte
                    || value is byte
                    || value is short
                    || value is ushort
                    || value is int
                    || value is uint
                    || value is long
                    || value is ulong
                    || value is float
                    || value is double
                    || value is decimal;
        }

        /// <summary>
        /// Update value.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("AddValue")]
        public ActionResult<AddValueResponse> Post(AddValue request)
        {
            AddValueResponse ret = new AddValueResponse();
            GXUpdateArgs arg;
            if (request.Items != null)
            {
                DateTime now = DateTime.Now;
                foreach (GXValue it in request.Items)
                {
                    if (it.Read == DateTime.MinValue)
                    {
                        it.Read = now;
                    }
                    GXAttribute value = new GXAttribute();
                    value.Id = it.AttributeId;
                    value.Value = it.Value;
                    value.Read = it.Read;
                    arg = GXUpdateArgs.Update<GXAttribute>(value, q => new { q.Value, q.Read });
                    host.Connection.Update(arg);
                }
                host.Connection.Insert(GXInsertArgs.InsertRange(request.Items));
                host.SetChange(TargetType.Value, now);
            }
            return ret;
        }
    }
}
