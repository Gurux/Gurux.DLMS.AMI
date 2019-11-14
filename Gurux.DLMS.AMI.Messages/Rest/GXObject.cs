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
    /// Add COSEM object.
    /// </summary>
    [DataContract]
    [Description("Add COSEM object.")]
    [Example("/api/ObjectAddObject", "{\r\t\"Items\" : {\"DeviceId\" : 1, \"TemplateId\" : 1}\r}", "www.gurux.fi/Gurux.DLMS.AMI.Object")]
    public class AddObject : IGXRequest<AddObjectResponse>
    {
        /// <summary>
        /// Added COSEM objects.
        /// </summary>
        [Description("Added COSEM objects.")]
        [DataMember]
        public GXObject[] Items
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Add COSEM object Reply.
    /// </summary>
    [DataContract]
    [Description("Add COSEM object Reply.")]
    public class AddObjectResponse
    {
        /// <summary>
        /// Object Ids.
        /// </summary>
        [Description("Object Ids.")]
        [DataMember]
        public UInt64[] Ids
        {
            get;
            set;
        }
    }


    /// <summary>
    /// Get object.
    /// </summary>
    [DataContract]
    [Description("List objects.")]
    [Example("/api/Object/ListObjects", "{\r\tDeviceId:1\r}", "www.gurux.fi/Gurux.DLMS.AMI.Object")]
    public class ListObjects : IGXRequest<ListObjectsResponse>
    {
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
        /// Device ID.
        /// </summary>
        [DataMember]
        [Description("Device ID.")]
        public UInt64 DeviceId
        {
            get;
            set;
        }

        /// <summary>
        /// Objects to list.
        /// </summary>
        [DataMember]
        [Description("Objects to list.")]
        public GXObject[] Objects
        {
            get;
            set;
        }

        /// <summary>
        /// Profile Generic start time. This is used only with Profile Generic objects.
        /// </summary>
        [DataMember]
        [Description("Profile Generic start time. This is used only with Profile Generic objects.")]
        public DateTime Start
        {
            get;
            set;
        }

        /// <summary>
        /// Profile Generic end time. This is used only with Profile Generic objects.
        /// </summary>
        [DataMember]
        [Description("Profile Generic end time. This is used only with Profile Generic objects.")]
        public DateTime End
        {
            get;
            set;
        }

        /// <summary>
        /// Profile Generic Index. This is used only with Profile Generic objects.
        /// </summary>
        [DataMember]
        [Description("Profile Generic Index. This is used only with Profile Generic objects.")]
        public UInt64 Index
        {
            get;
            set;
        }

        /// <summary>
        /// Profile Generic Count. This is used only with Profile Generic objects.
        /// </summary>
        [DataMember]
        [Description("Profile Generic Count. This is used only with Profile Generic objects.")]
        public UInt64 Count
        {
            get;
            set;
        }
    }

    /// <summary>
    /// List COSEM object response.
    /// </summary>
    [DataContract]
    [Description("List COSEM object response.")]
    public class ListObjectsResponse
    {

        /// <summary>
        /// List of COSEM objects.
        /// </summary>
        [DataMember]
        [Description("List of COSEM objects.")]
        public GXObject[] Items
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Delete COSEM objects.
    /// </summary>
    [Description("Delete COSEM objects.")]
    [DataContract]
    [Example("/api/Object/ObjectDelete", "{\r\t\"Ids\": {1}\r}", "www.gurux.fi/Gurux.DLMS.AMI.Object")]
    public class ObjectDelete : IGXRequest<ObjectDeleteResponse>
    {
        /// <summary>
        /// Removed COSEM objects IDs.
        /// </summary>
        [Description("Removed COSEM object IDs.")]
        [DataMember]
        public UInt64[] Ids
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Delete COSEM object response.
    /// </summary>
    [DataContract]
    [Description("Delete COSEM object response.")]
    public class ObjectDeleteResponse
    {
    }
}
