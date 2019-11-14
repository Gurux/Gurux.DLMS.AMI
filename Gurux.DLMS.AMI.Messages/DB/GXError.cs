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
    [DataContract]
    [Description("Device errors.")]
    public class GXError : IUnique<UInt64>
    {
        /// <summary>
        /// Error Id.
        /// </summary>
        [DataMember]
        [AutoIncrement]
        [Description("Error Id.")]
        public UInt64 Id
        {
            get;
            set;
        }

        /// <summary>
        /// Device Id.
        /// </summary>
        [DataMember]
        [Description("Device Id.")]
        [ForeignKey(typeof(GXDevice), OnDelete = ForeignKeyDelete.Cascade)]
        public UInt64 DeviceId
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
        /// Error string.
        /// </summary>
        [DataMember]
        [Description("Error string.")]
        public string Error
        {
            get;
            set;
        }
    }
}
