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
    /// Add or Update device information. Device is added if ID is zero.
    /// </summary>
    [DataContract]
    [Description("Add or Update device information. Device is added if ID is zero.")]
    [Example("/api/Device/UpdateDevice", "{\r\tDevice: {\r\t\tTemplateId:1,\r\t\tType=\"First device\"\r\t}\r}", "www.gurux.fi/Gurux.DLMS.AMI.Device")]
    public class UpdateDevice : IGXRequest<UpdateDeviceResponse>
    {
        /// <summary>
        /// Device info.
        /// </summary>
        [DataMember]
        [Description("Device info.")]
        public GXDevice Device
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Update device response.
    /// </summary>
    [DataContract]
    [Description("Update device response.")]
    public class UpdateDeviceResponse
    {
        /// <summary>
        /// New Device ID.
        /// </summary>
        [DataMember]
        [Description("new Device ID.")]
        public UInt64 DeviceId
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Get available devices.
    /// </summary>
    [DataContract]
    [Description("Get available devices.")]
    [Example("/api/Device/ListDevices", null, "www.gurux.fi/Gurux.DLMS.AMI.Device")]
    public class ListDevices : IGXRequest<ListDevicesResponse>
    {
        /// <summary>
        /// List of device IDs to retreave. Null if all devices are retreaved.
        /// </summary>
        [DataMember]
        [Description("List of device IDs to retreave. Null if all devices are retreaved.")]
        public UInt64[] Ids
        {
            get;
            set;
        }

        /// <summary>
        /// Data concentrator id.
        /// </summary>
        [DataMember]
        [Description("Data concentrator id. For data concentrator this is 0.")]
        public UInt64 Dc
        {
            get;
            set;
        }

        /// <summary>
        /// Tells information that is returned.
        /// </summary>
        [DataMember]
        [Description("Tells information that is returned.")]
        public TargetType Targets
        {
            get;
            set;
        }

        /// <summary>
        /// Get device by systemtitle.
        /// </summary>
        [DataMember]
        [Description("Get device by systemtitle.")]
        public string SystemTitle
        {
            get;
            set;
        }

        /// <summary>
        /// Filter devices by type.
        /// </summary>
        [DataMember]
        [Description("Filter devices by type.")]
        public UInt32 Filter
        {
            get;
            set;
        }

        /// <summary>
        /// Logical Device name.
        /// </summary>
        [DataMember]
        [Description("Logical Device name.")]
        public string Name
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Returns devices.
    /// </summary>
    [DataContract]
    [Description("Returns devices.")]
    public class ListDevicesResponse
    {
        /// <summary>
        /// List of devices.
        /// </summary>
        [DataMember]
        [Description("List of devices.")]
        public GXDevice[] Devices
        {
            get;
            set;
        }
    }


    /// <summary>
    /// Delete Device.
    /// </summary>
    [DataContract]
    [Description("Delete wanted Devices.")]
    [Example("/api/Device/DeviceDelete", null, "www.gurux.fi/Gurux.DLMS.AMI.Device")]
    public class DeviceDelete : IGXRequest<DeviceDeleteResponse>
    {
        /// <summary>
        /// Removed devices.
        /// </summary>
        [DataMember]
        [Description("Removed devices.")]
        public UInt64[] Ids
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Delete Device response.
    /// </summary>
    [DataContract]
    [Description("Delete Device response.")]
    public class DeviceDeleteResponse
    {
    }
}
