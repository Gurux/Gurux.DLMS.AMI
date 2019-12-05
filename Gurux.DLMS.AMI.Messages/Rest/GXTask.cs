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
    /// Add new task (Read, write or action) to the device.
    /// </summary>
    [DataContract]
    [Description("Add new task (Read, write or action) to the device.")]
    [Example("/api/Task/AddTask", null, "www.gurux.fi/Gurux.DLMS.AMI.Task")]

    public class AddTask : IGXRequest<AddTaskResponse>
    {
        /// <summary>
        /// Tasks to execute.
        /// </summary>
        [Description("Tasks to execute.")]
        [DataMember]
        public GXTask[] Actions
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Add task response.
    /// </summary>
    [DataContract]
    [Description("Add task response.")]
    public class AddTaskResponse
    {

    }

    /// <summary>
    /// Get list from the tasks.
    /// </summary>
    [DataContract]
    [Description("Get list from the tasks.")]
    [Example("/api/Task/ListTasks", null, "www.gurux.fi/Gurux.DLMS.AMI.Task")]
    public class ListTasks : IGXRequest<ListTasksResponse>
    {
        /// <summary>
        /// Targets.
        /// </summary>
        [DataMember]
        [Description("Targets")]
        public TargetType Targets
        {
            get;
            set;
        }

        [Description("Get tasks by device ID.")]
        [DataMember]
        public UInt64 DeviceId
        {
            get;
            set;
        }

        [Description("Get tasks by object ID.")]
        [DataMember]
        public UInt64 ObjectId
        {
            get;
            set;
        }

        [Description("Get tasks by attribute ID.")]
        [DataMember]
        public UInt64 AttributeId
        {
            get;
            set;
        }

        /// <summary>
        /// Maximum task count to return.
        /// </summary>
        [DataMember]
        [Description("Maximum task count to return..")]
        public UInt32 Count
        {
            get;
            set;
        }
    }

    /// <summary>
    /// List tasks response.
    /// </summary>
    [Description("List tasks response.")]
    [DataContract]
    public class ListTasksResponse
    {
        /// <summary>
        /// List of tasks.
        /// </summary>
        [DataMember]
        [Description("List of tasks.")]
        public GXTask[] Tasks
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Reserved for internal use. Get next executed task. Readers use this.
    /// </summary>
    [DataContract]
    [Description("Reserved for internal use. Get next executed task. Readers use this.")]
    [Example("/api/Task/GetNextTask", null, "www.gurux.fi/Gurux.DLMS.AMI.Task")]

    public class GetNextTask : IGXRequest<GetNextTaskResponse>
    {
        /// <summary>
        /// Device ID.
        /// </summary>
        /// <remarks>
        /// If device ID is given, all tasks for that device are retreaved.
        /// </remarks>
        [DataMember]
        [Description("Device ID. If device ID is given, all tasks for that device are retreaved.")]
        public UInt64 DeviceId
        {
            get;
            set;
        }

        /// <summary>
        /// Is listener or normal read asking next task.
        /// </summary>
        [DataMember]
        [Description("Is listener asking next task.")]
        public bool Listener
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Get next task response.
    /// </summary>
    [DataContract]
    [Description("Get next task response.")]
    public class GetNextTaskResponse
    {
        /// <summary>
        /// Executed tasks. Null if there are no operations to execute.
        /// </summary>
        [DataMember]
        [Description("Executed tasks. Null if there are no operations to execute.")]
        public GXTask[] Tasks
        {
            get;
            set;
        }
    }


    /// <summary>
    /// Reserved for internal use. Mark task to executed. Readers use this.
    /// </summary>
    [DataContract]
    [Description("Reserved for internal use. Mark task to executed. Readers use this.")]
    [Example("/api/Task/TaskReady", null, "www.gurux.fi/Gurux.DLMS.AMI.Task")]
    public class TaskReady : IGXRequest<TaskReadyResponse>
    {
        /// <summary>
        /// Executed tasks.
        /// </summary>
        [DataMember]
        [Description("Executed tasks.")]
        public GXTask[] Tasks
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Reserved for internal use. Task ready reply. Readers use this.
    /// </summary>
    [DataContract]
    [Description("Reserved for internal use. Task ready reply. Readers use this.")]
    public class TaskReadyResponse
    {

    }



    /// <summary>
    /// Delete tasks.
    /// </summary>
    [DataContract]
    [Description("Delete tasks.")]
    [Example("/api/Task/DeleteTask", "{\"Ids\": {1}}", "www.gurux.fi/Gurux.DLMS.AMI.Task")]
    public class DeleteTask : IGXRequest<DeleteTaskResponse>
    {
        /// <summary>
        /// Removed Tasks IDs.
        /// </summary>
        [DataMember]
        [Description("Removed Tasks IDs.")]
        public UInt64[] Ids
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Delete tasks response.
    /// </summary>
    [Description("Delete tasks response.")]
    [DataContract]
    public class DeleteTaskResponse
    {
    }
}
