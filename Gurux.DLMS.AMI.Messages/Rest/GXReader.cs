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
    /// Add new reader.
    /// </summary>
    [DataContract]
    [Description("Add new reader.")]
    [Example("/api/Reader/AddReader", null, "www.gurux.fi/Gurux.DLMS.AMI.Reader")]

    public class AddReader : IGXRequest<AddReaderResponse>
    {
        /// <summary>
        /// Reader info to add.
        /// </summary>
        [Description("Reader info to add.")]
        [DataMember]
        public GXReaderInfo Reader
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Add reader response.
    /// </summary>
    [DataContract]
    [Description("Add reader response.")]
    public class AddReaderResponse
    {
    }

    /// <summary>
    /// Get list from readers.
    /// </summary>
    [DataContract]
    [Description("Get list from readers.")]
    [Example("/api/Reader/ListReaders", null, "www.gurux.fi/Gurux.DLMS.AMI.Reader")]
    public class ListReaders : IGXRequest<ListReadersResponse>
    {
        /// <summary>
        /// List of reader IDs to retreave. Null if all readers are retreaved.
        /// </summary>
        [DataMember]
        [Description("List of reader IDs to retreave. Null if all readers are retreaved.")]
        public UInt64[] Ids
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Get readers response.
    /// </summary>
    [DataContract]
    [Description("Get readers response.")]
    public class ListReadersResponse
    {
        /// <summary>
        /// List of readers.
        /// </summary>
        [DataMember]
        [Description("List of readers.")]
        public GXReaderInfo[] Readers
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Remove reader.
    /// </summary>
    [DataContract]
    [Description("Remove reader.")]
    [Example("/api/Reader/RemoveReader", "{Ids : {1}}", "www.gurux.fi/Gurux.DLMS.AMI.Reader")]
    public class RemoveReader : IGXRequest<RemoveReaderResponse>
    {
        /// <summary>
        /// Reader Ids to remove.
        /// </summary>
        [DataMember]
        [Description("Reader Ids to remove.")]
        public UInt64[] Ids
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Remove reased response.
    /// </summary>
    [DataContract]
    [Description("Remove reased response.")]
    public class RemoveReaderResponse
    {
    }

    /// <summary>
    /// Get devices that are mapped for given readers.
    /// </summary>
    [DataContract]
    [Description("Get devices that are mapped for given readers.")]
    [Example("/api/Reader/ReaderDevices", "{Ids: {1}}", "www.gurux.fi/Gurux.DLMS.AMI.Reader")]
    public class ReaderDevices : IGXRequest<ReaderDeviceResponse>
    {
        /// <summary>
        /// List of Readers IDs.
        /// </summary>
        [DataMember]
        [Description("List of Readers IDs.")]
        public UInt64[] Ids
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Returns devices that are mapped for given readers.
    /// </summary>
    [DataContract]
    [Description("Returns devices that are mapped for given readers.")]
    public class ReaderDeviceResponse
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
    /// Add device to readers or remove device from readers.
    /// </summary>
    [DataContract]
    [Description("Add device to readers or remove device from readers.")]
    [Example("/api/Reader/AddReaderToDevices", "{Readers: {1}, Devices: {1}}", "www.gurux.fi/Gurux.DLMS.AMI.Reader")]
    public class ReaderDevicesUpdate : IGXRequest<ReaderDevicesUpdateResponse>
    {
        /// <summary>
        /// List of readers IDs to add or remove.
        /// </summary>
        [DataMember]
        [Description("List of readers IDs to add or remove.")]
        public UInt64[] Readers
        {
            get;
            set;
        }

        /// <summary>
        /// List of device IDs to add or remove.
        /// </summary>
        [DataMember]
        [Description("List of device IDs to add or remove.")]
        public UInt64[] Devices
        {
            get;
            set;
        }
    }

    /// <summary>
    /// ReaderDevicesUpdate response.
    /// </summary>
    [DataContract]
    [Description("ReaderDevicesUpdate response.")]
    public class ReaderDevicesUpdateResponse
    {
    }
}
