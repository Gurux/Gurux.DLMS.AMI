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

using Gurux.DLMS.Enums;
using System;

namespace Gurux.DLMS.AMI.Notify
{
    public class GXNotifyClient
    {
        public GXNotifyClient()
        {
            Notify = new GXReplyData();
            Reply = new GXByteBuffer();
            Client = new GXDLMSClient(true, -1, -1, Authentication.None, null, InterfaceType.WRAPPER);
            DataReceived = DateTime.MinValue;
        }

        /// <summary>
        /// Client used to parse received data.
        /// </summary>
        public GXDLMSClient Client
        {
            get;
            set;
        }

        /// <summary>
        /// Received data is saved to reply buffer because whole message is not always received in one packet.
        /// </summary>
        public GXByteBuffer Reply
        {
            get;
            set;
        }

        /// <summary>
        /// Received data. This is used if GBT is used and data is received on several data blocks.
        /// </summary>
        public GXReplyData Notify
        {
            get;
            set;
        }

        /// <summary>
        /// Time when last data was received.
        /// </summary>
        public DateTime DataReceived
        {
            get;
            set;
        }

    }
}
