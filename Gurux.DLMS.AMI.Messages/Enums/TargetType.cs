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

using System;

namespace Gurux.DLMS.AMI.Messages.Enums
{
    /// <summary>
    /// Enumerated changed items.
    /// </summary>
    [Flags]
    public enum TargetType : int
    {
        /// <summary>
        /// Nothing is changed.
        /// </summary>
        None = 0x0,
        /// <summary>
        /// Devices are changed.
        /// </summary>
        Device = 0x2,
        /// <summary>
        /// Objects are changed.
        /// </summary>
        Object = 0x4,
        /// <summary>
        /// Attributes are changed.
        /// </summary>
        Attribute = 0x8,
        /// <summary>
        /// Values are changed.
        /// </summary>
        Value = 0x10,
        /// <summary>
        /// Tasks are changed.
        /// </summary>
        Tasks = 0x20,
        /// <summary>
        /// Errors are changed.
        /// </summary>
        Error = 0x40,
        /// <summary>
        /// System errors are changed.
        /// </summary>
        SystemError = 0x80,
        /// <summary>
        /// Schedules are changed.
        /// </summary>
        Schedule = 0x100,
        /// <summary>
        /// Readers are changed.
        /// </summary>
        Readers = 0x200,
        /// <summary>
        /// Device templates are changed.
        /// </summary>
        DeviceTemplate = 0x400,
        /// <summary>
        /// Object templates are changed.
        /// </summary>
        ObjectTemplate = 0x800,
        /// <summary>
        /// Attribute templates are changed.
        /// </summary>
        AttributeTemplate = 0x1000
    }
}
