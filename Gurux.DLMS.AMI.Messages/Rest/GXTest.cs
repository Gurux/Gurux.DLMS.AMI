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

namespace Gurux.DLMS.AMI.Messages.Rest
{
    /// <summary>
    /// This interface can be used for testing Gurux.DLMS.AMI.
    /// </summary>
    [DataContract]
    [Description("This interface can be used for testing Gurux.DLMS.AMI.")]
    [Example("/api/Test/AddTestDevice", "{\r\tDevice: {\r\t\tTemplateId:1,\r\t\tIndex=1\r\t\tCount=2\r\t}\r}", "www.gurux.fi/Gurux.DLMS.AMI.Device")]
    public class AddTestDevice : IGXRequest<AddTestDeviceResponse>
    {
        /// <summary>
        /// Added device.
        /// </summary>
        [DataMember]
        [Description("Added device.")]
        public GXDevice Device
        {
            get;
            set;
        }

        /// <summary>
        /// Serial number of the first meter.
        /// </summary>
        /// <remarks>
        /// For each added meter is generated a new unique serial number.
        /// </remarks>
        [DataMember]
        [Description("Serial number of the first meter.")]
        public UInt16 Index
        {
            get;
            set;
        }

        /// <summary>
        /// Numer of added devices.
        /// </summary>
        [DataMember]
        [Description("Numer of added devices.")]
        public UInt16 Count
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Add test device response.
    /// </summary>
    [DataContract]
    [Description("Add test device response.")]
    public class AddTestDeviceResponse
    {
    }
}
