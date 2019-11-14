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
using Gurux.DLMS.AMI.Messages.Enums;
using System.Collections.Generic;

namespace Gurux.DLMS.AMI.Messages.Rest
{
    /// <summary>
    /// Get list from schedules.
    /// </summary>
    [DataContract]
    [Description("Get list from schedules.")]
    [Example("/api/Schedule/ListSchedules", null, "www.gurux.fi/Gurux.DLMS.AMI.Schedule")]
    public class ListSchedules : IGXRequest<ListSchedulesResponse>
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
        /// Get schedules using device ID.
        /// </summary>
        [Description("Get schedules using device ID.")]

        [DataMember]
        public UInt64 DeviceId
        {
            get;
            set;
        }

        /// <summary>
        /// Get schedules using object ID.
        /// </summary>
        [Description("Get schedules using object ID.")]
        [DataMember]
        public UInt64 ObjectId
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Schedule items reply.
    /// </summary>
    [DataContract]
    [Description("Schedule items reply.")]
    public class ListSchedulesResponse
    {
        /// <summary>
        /// List of schedule items.
        /// </summary>
        [Description("List of schedule items.")]
        [DataMember]
        public GXSchedule[] Schedules
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Update schedules.
    /// </summary>
    [DataContract]
    [Description("Update schedules.")]
    [Example("/api/Schedule/UpdateSchedule", null, "www.gurux.fi/Gurux.DLMS.AMI.Schedule")]
    public class UpdateSchedule : IGXRequest<UpdateScheduleResponse>
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public UpdateSchedule()
        {
            Schedules = new List<GXSchedule>();
        }

        /// <summary>
        /// Schedules to update.
        /// </summary>
        [DataMember]
        [Description("Schedules to update.")]
        public List<GXSchedule> Schedules
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Update schedules reply.
    /// </summary>
    [Description("Update schedules reply.")]
    [DataContract]
    public class UpdateScheduleResponse
    {
        /// <summary>
        /// New Schedule ID.
        /// </summary>
        [DataMember]
        [Description("new Schedule IDs.")]
        public UInt64[] ScheduleIds
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Delete schedules.
    /// </summary>
    [DataContract]
    [Description("Delete schedules.")]
    [Example("/api/Schedule/DeleteSchedule", "{\"ScheduleIds\": {1}}", "www.gurux.fi/Gurux.DLMS.AMI.Schedule")]
    public class DeleteSchedule : IGXRequest<DeleteScheduleResponse>
    {
        /// <summary>
        /// Removed schedule IDs.
        /// </summary>
        [DataMember]
        [Description("Removed schedule IDs.")]
        public UInt64[] ScheduleIds
        {
            get;
            set;
        }

        /// <summary>
        /// Removed attribute ID.
        /// </summary>
        [DataMember]
        [Description("Removed attribute ID.")]
        public UInt64[] AttributeIds
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Reply from Delete schedule.
    /// </summary>
    [DataContract]
    [Description("Reply from Delete schedule.")]
    public class DeleteScheduleResponse
    {
    }

    /// <summary>
    /// Add schedule target.
    /// </summary>
    [DataContract]
    [Description("Add schedule target.")]
    [Example("/api/Schedule/AddScheduleTarget", null, "www.gurux.fi/Gurux.DLMS.AMI.Schedule")]
    public class AddScheduleTarget : IGXRequest<AddScheduleTargetResponse>
    {
        /// <summary>
        /// Schedules Ids.
        /// </summary>
        [DataMember]
        [Description("Schedules Ids.")]
        public UInt64[] Schedules
        {
            get;
            set;
        }

        /// <summary>
        /// List of Device IDs to add to the schedule.
        /// </summary>
        [Description("List of Device IDs to add to the schedule.")]
        [DataMember]
        public UInt64 DeviceId
        {
            get;
            set;
        }

        /// <summary>
        /// List of objects to add to the schedule.
        /// </summary>
        [DataMember]
        [Description("List of objects to add to the schedule.")]
        public GXObject[] Objects
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Update schedule targets response.
    /// </summary>
    [DataContract]
    [Description("Update schedule targets response.")]
    public class AddScheduleTargetResponse
    {

    }

    /// <summary>
    /// Delete schedule target.
    /// </summary>
    [DataContract]
    [Description("Delete schedule target.")]
    [Example("/api/Schedule/DeleteScheduleTarget", null, "www.gurux.fi/Gurux.DLMS.AMI.Schedule")]
    public class DeleteScheduleTarget : IGXRequest<DeleteScheduleTargetResponse>
    {
        /// <summary>
        /// Schedules Ids where targets are removed.
        /// </summary>
        [DataMember]
        [Description("Schedules Ids where targets are removed.")]
        public UInt64[] Schedules
        {
            get;
            set;
        }

        /// <summary>
        /// List of attribute IDs to remove from the schedule.
        /// </summary>
        [DataMember]
        [Description("List of attribute IDs to remove from the schedule.")]
        public UInt64[] Attributes
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Delete schedule reply.
    /// </summary>
    [DataContract]
    [Description("Delete schedule reply.")]
    public class DeleteScheduleTargetResponse
    {
    }

    /// <summary>
    /// Update schedule execution time.
    /// </summary>
    [DataContract]
    [Description("Update schedule execution time.")]
    [Example("/api/Schedule/UpdateScheduleExecutionTime", null, "www.gurux.fi/Gurux.DLMS.AMI.Schedule")]
    public class UpdateScheduleExecutionTime : IGXRequest<UpdateScheduleExecutionTimeResponse>
    {
        /// <summary>
        /// Schedules.
        /// </summary>
        [DataMember]
        [Description("Schedule ID.")]
        public UInt64 Id
        {
            get;
            set;
        }
        /// <summary>
        /// Schedule execution time.
        /// </summary>
        [DataMember]
        [Description("Schedule execution time.")]
        public DateTime Time
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Update schedules response.
    /// </summary>
    [DataContract]
    [Description("Update schedules response.")]
    public class UpdateScheduleExecutionTimeResponse
    {

    }
}
