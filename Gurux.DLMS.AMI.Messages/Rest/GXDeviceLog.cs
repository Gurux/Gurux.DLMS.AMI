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
using System;
using System.Runtime.Serialization;
using System.ComponentModel;
using Gurux.DLMS.AMI.Messages.DB;
using Gurux.DLMS.AMI.Messages.Enums;

namespace Gurux.DLMS.AMI.Messages.Rest
{
    /// <summary>
    /// Adds a new device log.
    /// </summary>
    [DataContract]
    [Description("Adds a new device log.")]
    [Example("/api/Log/AddDeviceLog", null, "www.gurux.fi/Gurux.DLMS.AMI.Logging")]

    public class AddDeviceLog : IGXRequest<AddDeviceLogResponse>
    {
        /// <summary>
        /// Added Device log items.
        /// </summary>
        [Description("Added Device log items.")]
        [DataMember]
        public GXDeviceLog[] Logs
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Add device log response.
    /// </summary>
    [DataContract]
    [Description("Add device log response.")]
    public class AddDeviceLogResponse
    {

    }

    /// <summary>
    /// Get list from the device logs.
    /// </summary>
    [DataContract]
    [Description("Get list from the device logs.")]
    [Example("/api/Log/ListDeviceLogs", null, "www.gurux.fi/Gurux.DLMS.AMI.Logging")]
    public class ListDeviceLog : IGXRequest<ListDeviceLogResponse>
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

        /// <summary>
        /// Start index.
        /// </summary>
        [DataMember]
        [Description("Start index.")]
        public UInt32 Index
        {
            get;
            set;
        }

        /// <summary>
        /// Maximum device log count to return.
        /// </summary>
        [DataMember]
        [Description("Maximum device log count to return.")]
        public UInt32 Count
        {
            get;
            set;
        }

        /// <summary>
        /// Start time is used to retreave log items after the start time.
        /// </summary>
        [DataMember]
        [Description("Start time is used to retreave log items after the start time.")]
        public DateTime StartTime
        {
            get;
            set;
        }

        /// <summary>
        /// End time is used to retreave log items before the end time.
        /// </summary>
        [DataMember]
        [Description("End time is used to retreave log items before the end time.")]
        public DateTime EndTime
        {
            get;
            set;
        }
    }

    /// <summary>
    /// List device log response.
    /// </summary>
    [Description("List device log response.")]
    [DataContract]
    public class ListDeviceLogResponse
    {
        /// <summary>
        /// List of device logs.
        /// </summary>
        [DataMember]
        [Description("List of device logs.")]
        public GXDeviceLog[] Logs
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Clear device log. All logs are removed from the given device.
    /// </summary>
    [DataContract]
    [Description("Clear device log. All logs are removed from the given device.")]
    [Example("/api/Log/ClearDeviceLog", "{\"Ids\": {1}}", "www.gurux.fi/Gurux.DLMS.AMI.Logging")]
    public class ClearDeviceLog : IGXRequest<ClearDeviceLogResponse>
    {
        /// <summary>
        /// Device Ids where device logs are removed.
        /// </summary>
        [Description("Device Ids where device logs are removed.")]
        [DataMember]
        public UInt64[] Ids
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Clear device log response.
    /// </summary>
    [DataContract]
    [Description("Clear device log response.")]
    public class ClearDeviceLogResponse
    {
    }
}
