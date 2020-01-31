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
namespace Gurux.DLMS.AMI.Internal
{
    /// <summary>
    /// Notify settings.
    /// </summary>
    public class NotifyOptions
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public NotifyOptions()
        {
            NetworkType = 1;
        }

        /// <summary>
        /// Is Notifier disabled.
        /// </summary>
        public bool Disabled { get; set; }
        /// <summary>
        /// Network type. UDP = 0 and TCP = 1.
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
        /// System title of the server.
        /// </summary>
        public string SystemTitle { get; set; }

        /// <summary>
        /// Block cipher key key.
        /// </summary>
        public string BlockCipherKey { get; set; }

        /// <summary>
        /// Notify port. Server waits notify, event or push messages to this port.
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Expiration time in seconds.
        /// </summary>
        /// <remarks>
        /// Sometimes only part of the push message is received. Received data is clear after expiration time.
        /// </remarks>
        public int ExpirationTime { get; set; }

        /// <summary>
        /// Nofify parser.
        /// </summary>
        public string Parser { get; set; }
    }
}
