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
using System.Collections.Generic;

namespace Gurux.DLMS.AMI.Messages.DB
{
    [Description("Schedule.")]
    public class GXSchedule : IUnique<UInt64>
    {
        /// <summary>
        /// Schedule Id.
        /// </summary>
        [DataMember]
        [AutoIncrement]
        [Description("Schedule ID.")]
        public UInt64 Id
        {
            get;
            set;
        }

        /// <summary>
        /// Objects.
        /// </summary>
        [DataMember]
        [ForeignKey(typeof(GXAttribute), typeof(GXScheduleToAttribute))]
        [Description("Objects.")]
        public List<GXObject> Objects
        {
            get;
            set;
        }

        /// <summary>
        /// Name.
        /// </summary>
        [DataMember]
        [Description("Schedule name.")]
        public string Name
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
        /// Schedule start time.
        /// </summary>
        [DataMember]
        [Description("Schedule start time.")]
        public string Start
        {
            get;
            set;
        }

        /// <summary>
        /// Last execution time
        /// </summary>
        [DataMember]
        [Description("Last execution time.")]
        public DateTime ExecutionTime
        {
            get;
            set;
        }

        /// <summary>
        /// Remove time.
        /// </summary>
        [Description("Remove time.")]
        public DateTime Removed
        {
            get;
            set;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
