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

namespace Gurux.DLMS.AMI.Messages.Rest
{

    /// <summary>
    /// Add new system error.
    /// </summary>
    [DataContract]
    [Description("Add new system error.")]
    [Example("/api/SystemError/AddSystemError", "{\r\tError:\r\t{\r\t\tError: \"Hello world!\"\r\t}\r}", "www.gurux.fi/Gurux.DLMS.AMI.SystemError")]
    public class AddSystemError : IGXRequest<AddSystemErrorResponse>
    {
        /// <summary>
        /// Occurred error.
        /// </summary>
        [DataMember]
        [Description("Occurred error.")]
        public GXSystemError Error
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Add system error response.
    /// </summary>
    [DataContract]
    [Description("Add system error response.")]
    public class AddSystemErrorResponse
    {
    }

    /// <summary>
    /// Get system errors.
    /// </summary>
    [DataContract]
    [Description("Get system errors.")]
    [Example("/api/SystemError/ListSystemErrors", null, "www.gurux.fi/Gurux.DLMS.AMI.SystemError")]
    public class ListSystemErrors : IGXRequest<ListSystemErrorsResponse>
    {
    }

    /// <summary>
    /// Get system errors response.
    /// </summary>
    [Description("Get system errors response.")]
    [DataContract]
    public class ListSystemErrorsResponse
    {
        /// <summary>
        /// System errors.
        /// </summary>
        [DataMember]
        [Description("System errors.")]
        public GXSystemError[] Errors
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Clear system errors.
    /// </summary>
    [DataContract]
    [Description("Clear system errors.")]
    [ExampleAttribute("/api/SystemError/ClearSystemErrors", "{\r}", "www.gurux.fi/Gurux.DLMS.AMI.SystemError")]
    public class ClearSystemError : IGXRequest<ClearSystemErrorResponse>
    {
    }

    /// <summary>
    /// Clear system errors response.
    /// </summary>
    [Description("Clear system errors response.")]
    [DataContract]
    public class ClearSystemErrorResponse
    {
    }

}
