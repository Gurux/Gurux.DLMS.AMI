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

namespace Gurux.DLMS.AMI.Messages.Rest
{
    /// <summary>
    /// Get historical value from the DB.
    /// </summary>
    [DataContract]
    [Description("Get historical value from the DB.")]
    [Example("/api/Value/ListValues", null, "www.gurux.fi/Gurux.DLMS.AMI.Value")]
    public class ListValues : IGXRequest<ListValuesResponse>
    {
        /// <summary>
        /// List of value IDs.
        /// </summary>
        [DataMember]
        [Description("List of value IDs.")]
        public UInt64[] Ids
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Get values reply.
    /// </summary>
    [DataContract]
    [Description("Get values reply.")]
    public class ListValuesResponse
    {
        /// <summary>
        /// List of value items.
        /// </summary>
        [Description("List of value items.")]
        [DataMember]
        public GXValue[] Items
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Add attribute value.
    /// </summary>
    [DataContract]
    [Description("Add attribute value.")]
    [Example("/api/Value/AddValue", null, "www.gurux.fi/Gurux.DLMS.AMI.Value")]
    public class AddValue : IGXRequest<AddValueResponse>
    {
        /// <summary>
        /// Values to add.
        /// </summary>
        [DataMember]
        [Description("Values to add")]
        public GXValue[] Items
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Value add response.
    /// </summary>
    [Description("Value add response.")]
    [DataContract]
    public class AddValueResponse
    {
    }
}
