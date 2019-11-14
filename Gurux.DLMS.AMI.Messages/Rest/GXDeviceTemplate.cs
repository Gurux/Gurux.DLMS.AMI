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
    /// Update device template information. Device template is added if ID is zero.
    /// </summary>
    [DataContract]
    [Description("Add or Update device template information. Device template is added if ID is zero.")]
    [Example("/api/Template/UpdateDeviceTemplate", null, "www.gurux.fi/Gurux.DLMS.AMI.Template")]
    public class UpdateDeviceTemplate : IGXRequest<UpdateDeviceTemplateResponse>
    {
        /// <summary>
        /// Inserted or updated device templates.
        /// </summary>
        [DataMember]
        [Description("Inserted or updated device templates.")]
        public GXDeviceTemplate Device
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Device template response.
    /// </summary>
    [DataContract]
    [Description("Insert or update device template response.")]
    public class UpdateDeviceTemplateResponse
    {
        /// <summary>
        /// New device template ID.
        /// </summary>
        [DataMember]
        [Description("new device template ID.")]
        public UInt64 DeviceId
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Get list from available device templates.
    /// </summary>
    [DataContract]
    [Description("Get list from available device templates.")]
    [Example("/api/Template/ListDeviceTemplates", null, "www.gurux.fi/Gurux.DLMS.AMI.DeviceTemplate")]
    public class ListDeviceTemplates : IGXRequest<ListDeviceTemplatesResponse>
    {
        /// <summary>
        /// List of device template IDs to retreave. Null if all devices templates are retreaved.
        /// </summary>
        [DataMember]
        [Description("List of device template IDs to retreave. Null if all devices templates are retreaved.")]
        public UInt64[] Ids
        {
            get;
            set;
        }

        /// <summary>
        /// Device template name.
        /// </summary>
        [DataMember]
        [Description("Device template name.")]
        public string Name
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

    }

    /// <summary>
    /// Available devices templates response.
    /// </summary>
    [DataContract]
    [Description("Available devices templates response.")]
    public class ListDeviceTemplatesResponse
    {
        /// <summary>
        /// List of device templates.
        /// </summary>
        [DataMember]
        [Description("List of device templates.")]
        public GXDeviceTemplate[] Devices
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Delete device template.
    [Description("Delete device template.")]
    /// </summary>
    [DataContract]
    [Example("/api/Template/DeviceTemplateDelete", "{\"Ids\": {1}}", "www.gurux.fi/Gurux.DLMS.AMI.Template")]

    public class DeviceTemplateDelete : IGXRequest<DeviceTemplateDeleteResponse>
    {
        /// <summary>
        /// Removed device IDs.
        /// </summary>
        [Description("Removed device IDs.")]
        [DataMember]
        public UInt64[] Ids
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Delete device template response.
    /// </summary>
    [DataContract]
    [Description("Delete device template response.")]
    public class DeviceTemplateDeleteResponse
    {
    }

    /// <summary>
    /// Remove object template.
    /// </summary>
    [Description("Remove object template.")]
    [DataContract]
    [Example("/api/Template/ObjectTemplateDelete", "{\"Ids\": {1}}", "www.gurux.fi/Gurux.DLMS.AMI.Template")]
    public class ObjectTemplateDelete : IGXRequest<ObjectTemplateDeleteResponse>
    {
        /// <summary>
        /// Removed object template IDs.
        /// </summary>
        [DataMember]
        [Description("Removed object template IDs.")]
        public UInt64[] Ids
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Remove object template response.
    /// </summary>
    [Description("Remove object template response.")]
    [DataContract]
    public class ObjectTemplateDeleteResponse
    {
    }


    /// <summary>
    /// Remove attribute template.
    /// </summary>
    [Description("Remove attribute template.")]
    [DataContract]
    [Example("/api/Template/AttributeTemplateDelete", "{\"Ids\": {1}}", "www.gurux.fi/Gurux.DLMS.AMI.Template")]
    public class AttributeTemplateDelete : IGXRequest<AttributeTemplateDeleteResponse>
    {
        /// <summary>
        /// Removed attribute template IDs.
        /// </summary>
        [Description("Removed attribute template IDs.")]
        [DataMember]
        public UInt64[] Ids
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Delete attribute template response.
    /// </summary>
    [Description("Delete attribute template response.")]
    [DataContract]
    public class AttributeTemplateDeleteResponse
    {
    }
}
