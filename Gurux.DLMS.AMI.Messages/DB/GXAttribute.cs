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
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Gurux.DLMS.AMI.Messages.DB
{
    [Description("COSEM object attribute.")]
    public class GXAttribute : IUnique<UInt64>
    {
        /// <summary>
        /// Attribute Id.
        /// </summary>
        [DataMember]
        [AutoIncrement]
        [Description("Attribute Id.")]
        public UInt64 Id
        {
            get;
            set;
        }

        /// <summary>
        /// Object Id.
        /// </summary>
        [DataMember]
        [ForeignKey(typeof(GXObject), OnDelete = ForeignKeyDelete.Cascade)]
        [Description("Object Id.")]
        public UInt64 ObjectId
        {
            get;
            set;
        }

        /// <summary>
        /// Template Id.
        /// </summary>
        [DataMember]
        [ForeignKey(typeof(GXAttributeTemplate))]
        [Description("Template Id.")]
        public UInt64 TemplateId
        {
            get;
            set;
        }

        /// <summary>
        /// Attribute index.
        /// </summary>
        [DataMember]
        [Description("Attribute index.")]
        public int Index
        {
            get;
            set;
        }

        /// <summary>
        /// Object attribute name.
        /// </summary>
        [DataMember]
        [Description("Object attribute name.")]
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Latest read value.
        /// </summary>
        [DataMember]
        [Description("Latest read value")]
        public string Value
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
        /// When value is last updated.
        /// </summary>
        [DataMember]
        [Description("When value is last updated.")]
        public DateTime Updated
        {
            get;
            set;
        }

        /// <summary>
        /// When value is read last time.
        /// </summary>
        /// <remarks>
        /// Profile generic uses this when buffer is read.
        /// </remarks>
        [DataMember]
        [Description("When value is read last time.")]
        public DateTime Read
        {
            get;
            set;
        }

        /// <summary>
        /// ID of last executed task.
        /// </summary>
        [DataMember]
        [Description("ID of last executed task.")]
        public UInt64 TaskId
        {
            get;
            set;
        }

        /// <summary>
        /// Expiration time tells how often value needs to read from the meter. It's not used if it is Zero and read only once if 0xFFFFFFFF.
        /// </summary>
        [DataMember]
        [Description("Expiration time in seconds.")]
        public UInt32 ExpirationTime
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
        /// Access level.
        /// </summary>
        [DataMember]
        [Description("Access level.")]
        public int AccessLevel
        {
            get;
            set;
        }

        /// <summary>
        /// Last exception. Successful read nulls this.
        /// </summary>
        [DataMember]
        [Description("Last exception.")]
        public string Exception
        {
            get;
            set;
        }

        /// <summary>
        /// Data type.
        /// </summary>
        [DataMember]
        [Description("Data type.")]
        public int DataType
        {
            get;
            set;
        }

        /// <summary>
        /// UI Data type.
        /// </summary>
        [DataMember]
        [Description("UI Data type.")]
        public int UIDataType
        {
            get;
            set;
        }

        /// <summary>
        /// Extra info is used to save future extra info.
        /// </summary>
        [DataMember]
        [Description("Extra info is used to save future extra info.")]
        public UInt32 ExtraInfo
        {
            get;
            set;
        }
    }
}
