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
using Gurux.DLMS.AMI.Messages.Enums;
using Gurux.DLMS.AMI.Messages.Rest;
using Microsoft.AspNetCore.Mvc;
using Gurux.DLMS.AMI.Messages.DB;

namespace DBService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReaderController : ControllerBase
    {
        private readonly GXHost host;

        public ReaderController(GXHost value)
        {
            host = value;
        }

        /// <summary>
        /// Get available readers.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<ListReadersResponse> Get()
        {
            return Post(new ListReaders());
        }

        /// <summary>
        /// Add reader.
        /// </summary>
        [HttpPost("AddReader")]
        public ActionResult<AddReaderResponse> Post(AddReader request)
        {
            GXSelectArgs arg = GXSelectArgs.SelectAll<GXReaderInfo>();
            arg.Where.And<GXReaderInfo>(q => q.Guid == request.Reader.Guid);
            GXReaderInfo i = host.Connection.SingleOrDefault<GXReaderInfo>(arg);
            if (i == null)
            {
                request.Reader.Detected = request.Reader.Generation = DateTime.Now;
                host.Connection.Insert(GXInsertArgs.Insert(request.Reader));
            }
            else
            {
                i.Detected = DateTime.Now;
                host.Connection.Update(GXUpdateArgs.Update(i, u => u.Detected));
            }
            host.SetChange(TargetType.Readers, DateTime.Now);
            return new AddReaderResponse();
        }

        /// <summary>
        /// List device Readers
        /// </summary>
        [HttpPost("ListReaders")]
        public ActionResult<ListReadersResponse> Post(ListReaders request)
        {
            ListReadersResponse ret = new ListReadersResponse();
            GXSelectArgs arg = GXSelectArgs.SelectAll<GXReaderInfo>();
            if (request.Ids != null && request.Ids.Length != 0)
            {
                arg.Where.And<GXReaderInfo>(q => request.Ids.Contains(q.Id));
            }
            ret.Readers = host.Connection.Select<GXReaderInfo>(arg).ToArray();
            return ret;
        }

        [HttpPost("ClearReaders")]
        public ActionResult<RemoveReaderResponse> Post(RemoveReader request)
        {
            if (request.Ids != null)
            {
                host.Connection.Delete(GXDeleteArgs.Delete<GXReaderInfo>(q => request.Ids.Contains(q.Id)));
            }
            else
            {
                host.Connection.Delete(GXDeleteArgs.DeleteAll<GXReaderInfo>());
            }
            host.SetChange(TargetType.Readers, DateTime.Now);
            return new RemoveReaderResponse();
        }

    }
}
