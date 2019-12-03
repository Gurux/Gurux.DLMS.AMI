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
using Gurux.Service.Orm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Gurux.DLMS.AMI.Messages.DB
{
    [Description("COSEM object template.")]
    public class GXObjectTemplate : IUnique<UInt64>
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public GXObjectTemplate()
        {
            Attributes = new List<GXAttributeTemplate>();
        }

        /// <summary>
        /// Object template Id.
        /// </summary>
        [DataMember]
        [AutoIncrement]
        [Description("Object template Id.")]
        public UInt64 Id
        {
            get;
            set;
        }

        /// <summary>
        /// Device template Id.
        /// </summary>
        [DataMember]
        [ForeignKey(typeof(GXDeviceTemplate))]
        [Description("Device template Id.")]
        public UInt64 DeviceTemplateId
        {
            get;
            set;
        }

        /// <summary>
        /// Object type.
        /// </summary>
        [DataMember]
        [Description("Object type.")]
        public int ObjectType
        {
            get;
            set;
        }


        /// <summary>
        /// Object version.
        /// </summary>
        [DataMember]
        [Description("Object version.")]
        public int Version
        {
            get;
            set;
        }


        /// <summary>
        /// Logical name of the object.
        /// </summary>
        [DataMember]
        [Description("Name of the object.")]
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Logical name of the object.
        /// </summary>
        [DataMember]
        [Description("Logical name of the object.")]
        public string LogicalName
        {
            get;
            set;
        }

        /// <summary>
        /// Short name of the object.
        /// </summary>
        [DataMember]
        [Description("Short name of the object.")]
        public UInt16 ShortName
        {
            get;
            set;
        }

        /// <summary>
        /// Generation time.
        /// </summary>
        [DataMember]
        [Description("Generation time.")]
        public DateTime Generation
        {
            get;
            set;
        }

        /// <summary>
        /// When object is last updated.
        /// </summary>
        [DataMember]
        [Description("When object is last updated.")]
        public DateTime Updated
        {
            get;
            set;
        }

        /// <summary>
        /// Remove time.
        /// </summary>
        [DataMember]
        [Description("Remove time.")]
        public DateTime Removed
        {
            get;
            set;
        }

        /// <summary>
        /// Expiration time tells how often value needs to read from the meter. If it's DateTime.Min it will read every read. If it's DateTime.Max it's read only once.
        /// </summary>
        [DataMember]
        [Description("Expiration time.")]
        public DateTime ExpirationTime
        {
            get;
            set;
        }

        /// <summary>
        /// Attribute templates.
        /// </summary>
        [DataMember]
        [Description("Attribute templates")]
        [ForeignKey(typeof(GXAttributeTemplate))]
        public List<GXAttributeTemplate> Attributes
        {
            get;
            set;
        }
    }
}
