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
using System.Diagnostics;

namespace Gurux.DLMS.AMI.Internal
{
    /// <summary>
    /// Listener settings. Listener is used with dynamic TCP/IP connections
    /// where the meter makes the connection to the server.
    /// </summary>
    public class ListenerOptions
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public ListenerOptions()
        {
            NetworkType = 1;
        }

        /// <summary>
        /// Is Listener disabled.
        /// </summary>
        public bool Disabled { get; set; }
        /// <summary>
        /// Listener port. Server waits meters to connect to this port..
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Network type. UDP = 1 and TCP = 2.
        /// </summary>
        public int NetworkType { get; set; }

        /// <summary>
        /// Interface type.
        /// </summary>
        public int Interface { get; set; }


        /// <summary>
        /// Use logical name referencing.
        /// </summary>
        public bool UseLogicalNameReferencing { get; set; }

        /// <summary>
        /// Client address.
        /// </summary>
        public int ClientAddress { get; set; }

        /// <summary>
        /// Server address.
        /// </summary>
        public int ServerAddress { get; set; }

        /// <summary>
        /// Authentication level.
        /// </summary>
        public int Authentication { get; set; }

        /// <summary>
        /// Password.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Security.
        /// </summary>
        public int Security { get; set; }

        /// <summary>
        /// Server address.
        /// </summary>
        public string InvocationCounter { get; set; }

        /// <summary>
        /// Default device template ID.
        /// </summary>
        /// <remarks>
        /// New meter with this device template is created when a new meter is making connection to the Gurux.DLMS.AMI server.
        /// New devices are not created automatically if value is zero.
        /// </remarks>
        public UInt64 DefaultDeviceTemplate { get; set; }

        /// <summary>
        /// Used trace level.
        /// </summary>
        public TraceLevel TraceLevel
        {
            get;
            set;
        }
    }
}
