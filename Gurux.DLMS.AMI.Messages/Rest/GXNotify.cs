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
    ///Wait task, device, object or error changes.
    /// </summary>
    [DataContract]
    [Description("Wait task, device, object or error changes.")]
    public class WaitChange : IGXRequest<WaitChangeResponse>
    {
        /// <summary>
        /// Requests.
        /// </summary>
        [DataMember]
        [Description("Last notify time")]
        public DateTime Time
        {
            get;
            set;
        }

        /// <summary>
        /// Changed types.
        /// </summary>
        [DataMember]
        [Description("Changed types.")]
        public TargetType Change
        {
            get;
            set;
        }

        /// <summary>
        /// How long new event is waited in seconds.
        /// </summary>
        [DataMember]
        [Description("How long new event is waited in seconds.")]
        public int WaitTime
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Wait change response.
    /// </summary>
    [DataContract]
    public class WaitChangeResponse
    {
        /// <summary>
        /// Time when last change was made.
        /// </summary>
        [DataMember]
        [Description("Last notify time")]
        public DateTime Time
        {
            get;
            set;
        }

        /// <summary>
        /// Changed types.
        /// </summary>
        [DataMember]
        [Description("Changed types.")]
        public TargetType Change
        {
            get;
            set;
        }
    }
}
