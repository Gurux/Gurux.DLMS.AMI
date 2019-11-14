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
using Gurux.DLMS.AMI.Messages.Enums;
using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Gurux.DLMS.AMI.Messages.DB
{
    [Description("DC request.")]
    public class GXTask : IUnique<UInt64>
    {
        /// <summary>
        /// Request Id.
        /// </summary>
        [DataMember]
        [Gurux.Service.Orm.AutoIncrement]
        [Description("Request ID.")]
        public UInt64 Id
        {
            get;
            set;
        }

        /// <summary>
        /// COSEM object.
        /// </summary>
        [DataMember]
        [ForeignKey(typeof(GXObject), OnDelete = ForeignKeyDelete.Cascade)]
        [Description("Object.")]
        public GXObject Object
        {
            get;
            set;
        }

        /// <summary>
        /// Task type.
        /// </summary>
        [DataMember]
        [Description("Task type.")]
        public TaskType TaskType
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
        /// Start execution time.
        /// </summary>
        [DataMember]
        [Description("Start execution time.")]
        public DateTime Start
        {
            get;
            set;
        }

        /// <summary>
        /// Time when task is executed.
        /// </summary>
        [DataMember]
        [Description("Time when task is executed.")]
        public DateTime End
        {
            get;
            set;
        }


        /// <summary>
        /// Task result.
        /// </summary>
        [DataMember]
        [Description("Task result.")]
        public string Result
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
        /// Data to write.
        /// </summary>
        [DataMember]
        [Description("Data to write.")]
        public string Data
        {
            get;
            set;
        }
    }

}
