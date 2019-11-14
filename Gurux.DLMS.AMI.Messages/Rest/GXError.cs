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
using Gurux.Common;
using System.Runtime.Serialization;
using System.ComponentModel;
using Gurux.DLMS.AMI.Messages.DB;
using System;

namespace Gurux.DLMS.AMI.Messages.Rest
{
    /// <summary>
    /// Get list from device errors.
    /// </summary>
    [DataContract]
    [Description("Get list from device errors.")]
    [Example("/api/Error/ListErrors", null, "www.gurux.fi/Gurux.DLMS.AMI.Error")]
    public class ListErrors : IGXRequest<ListErrorsResponse>
    {
        /// <summary>
        /// Device IDs.
        /// </summary>
        [DataMember]
        [Description("Device IDs.")]
        public UInt64[] Ids
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Get errors response.
    /// </summary>
    [DataContract]
    [Description("Get errors response.")]
    public class ListErrorsResponse
    {
        /// <summary>
        /// List of Device errors.
        /// </summary>
        [DataMember]
        [Description("List of Device errors.")]
        public GXError[] Errors
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Add new device error.
    /// </summary>
    [DataContract]
    [Description("Add new device error.")]
    [Example("/api/Error", "{\r\tError:\r\t{\r\t\t\"DeviceId:\"1,\r\t\t\"Error: \"Hello World!\"\"\r\t}\r}", "www.gurux.fi/Gurux.DLMS.AMI.Error")]
    public class AddError : IGXRequest<AddErrorResponse>
    {
        /// <summary>
        /// New device error.
        /// </summary>
        [DataMember]
        [Description("New device error.")]
        public GXError Error
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Add new device error response.
    /// </summary>
    [DataContract]
    [Description("Add new device error response.")]
    public class AddErrorResponse
    {
    }

    /// <summary>
    /// Clear device errors. All errors are removed from the given device.
    /// </summary>
    [DataContract]
    [Description("Clear device errors. All errors are removed from the given device.")]
    [Example("/api/Error/ClearErrors", "{\"Ids\": {1}}", "www.gurux.fi/Gurux.DLMS.AMI.Error")]
    public class ClearDeviceError : IGXRequest<ClearDeviceErrorResponse>
    {
        /// <summary>
        /// Device Ids where errors are removed.
        /// </summary>
        [Description("Device Ids where errors are removed.")]
        [DataMember]
        public UInt64[] Ids
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Clear device errors response.
    /// </summary>
    [DataContract]
    [Description("Clear device errors response.")]
    public class ClearDeviceErrorResponse
    {
    }
}
