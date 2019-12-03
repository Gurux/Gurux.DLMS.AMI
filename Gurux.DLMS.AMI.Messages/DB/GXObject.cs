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
    [Description("COSEM object.")]
    public class GXObject : IUnique<UInt64>
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public GXObject()
        {
            Attributes = new List<GXAttribute>();
        }


        /// <summary>
        /// Object Id.
        /// </summary>
        [DataMember]
        [AutoIncrement]
        [Description("Object Id.")]
        public UInt64 Id
        {
            get;
            set;
        }

        /// <summary>
        /// Device Id.
        /// </summary>
        [DataMember]
        [ForeignKey(typeof(GXDevice), OnDelete = ForeignKeyDelete.Cascade)]
        [Description("Device Id.")]
        public UInt64 DeviceId
        {
            get;
            set;
        }


        /// <summary>
        /// Template Id.
        /// </summary>
        [DataMember]
        [ForeignKey(typeof(GXObjectTemplate))]
        [Description("Template Id.")]
        public UInt64 TemplateId
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
        /// Name of the object.
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
        /// Attributes.
        /// </summary>
        [DataMember]
        [Description("Attributes")]
        [ForeignKey(typeof(GXAttribute))]
        public List<GXAttribute> Attributes
        {
            get;
            set;
        }
    }
}
