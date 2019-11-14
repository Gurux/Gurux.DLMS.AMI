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

namespace Gurux.DLMS.AMI.Messages.DB
{
    /// <summary>
    /// Reader info.
    /// </summary>
    [Description("Reader info.")]
    public class GXSchedulerInfo : IUnique<UInt64>
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public GXSchedulerInfo()
        {
        }

        /// <summary>
        /// Scheduler info Id.
        /// </summary>
        [AutoIncrement]
        [Description("Scheduler info unique ID.")]
        public UInt64 Id
        {
            get;
            set;
        }

        /// <summary>
        /// Name of the scheduler.
        /// </summary>
        [Description("Name of the scheduler.")]
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Scheduler identifier.
        /// </summary>
        [Description("Scheduler identifier.")]
        public Guid Guid
        {
            get;
            set;
        }

        /// <summary>
        /// Generation time.
        /// </summary>
        [Description("Generation time.")]
        public DateTime Generation
        {
            get;
            set;
        }

        /// <summary>
        /// When reader is detected last time.
        /// </summary>
        [Description("When reader is detected last time.")]
        public DateTime Detected
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
    }
}
