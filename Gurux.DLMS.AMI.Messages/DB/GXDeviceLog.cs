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
    /// <summary>
    /// Device log is used to save send and receive meter data to the DB.
    /// </summary>
    [Description("Device Log.")]
    public class GXDeviceLog : IUnique<UInt64>
    {
        /// <summary>
        /// Request Id.
        /// </summary>
        [DataMember]
        [AutoIncrement]
        [Description("Request ID.")]
        public UInt64 Id
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
        /// Trace type.
        /// </summary>
        [DataMember]
        [Description("Trace type")]
        public byte Type
        {
            get;
            set;
        }

        /// <summary>
        /// Log data.
        /// </summary>
        [DataMember]
        [Description("Log data.")]
        public string Data
        {
            get;
            set;
        }
    }

}
