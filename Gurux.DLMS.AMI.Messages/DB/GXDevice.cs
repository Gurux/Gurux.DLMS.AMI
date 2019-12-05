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
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Gurux.DLMS.AMI.Messages.DB
{
    /// <summary>
    /// Device settings.
    /// </summary>
    [Description("Device settings.")]
    public class GXDevice : GXDLMSMeter, IUnique<UInt64>
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDevice() : base()
        {
        }

        /// <summary>
        /// Device Id.
        /// </summary>
        [AutoIncrement]
        [Description("Device unique ID.")]
        public UInt64 Id
        {
            get;
            set;
        }


        /// <summary>
        /// DC Id.
        /// </summary>
        /// <remarks>
        /// This is reserved for later use.
        /// </remarks>
        [Description("DC ID.")]
        public UInt64 Dc
        {
            get;
            set;
        }


        /// <summary>
        /// Template Id.
        /// </summary>
        [Description("Template ID.")]
        [ForeignKey(typeof(GXDeviceTemplate))]
        public UInt64 TemplateId
        {
            get;
            set;
        }

        /// <summary>
        /// Device type.
        /// </summary>
        [Description("Device type.")]
        public string Type
        {
            get;
            set;
        }

        /// <summary>
        /// Device system title.
        /// </summary>
        [DataMember, StringLength(16)]
        [Description("Client system title.")]
        public string ClientSystemTitle
        {
            get
            {
                return base.SystemTitle;
            }
            set
            {
                SystemTitle = value;
            }
        }

        /// <summary>
        /// Device system title.
        /// </summary>
        [StringLength(16)]
        [Description("Device system title.")]
        public string DeviceSystemTitle
        {
            get
            {
                return ServerSystemTitle;
            }
            set
            {
                ServerSystemTitle = value;
            }
        }

        /// <summary>
        /// Block cipher key.
        /// </summary>
        [StringLength(32)]
        [Description("Block cipher key.")]
        override public string BlockCipherKey
        {
            get;
            set;
        }

        /// <summary>
        /// Authentication key.
        /// </summary>
        [StringLength(32)]
        [Description("Authentication key.")]
        override public string AuthenticationKey
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
        /// When devic is updated last time.
        /// </summary>
        [Description("When device is updated last time.")]
        public DateTime Updated
        {
            get;
            set;
        }

        /// <summary>
        /// When meter is detected last time.
        /// </summary>
        [Description("When meter is detected last time.")]
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

        [ForeignKey(typeof(GXObject))]
        public new List<GXObject> Objects
        {
            get;
            set;
        }

        /// <summary>
        /// Is device using dynamic IP address.
        /// </summary>
        public bool Dynamic
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
