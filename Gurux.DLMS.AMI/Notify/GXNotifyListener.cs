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
using Gurux.DLMS.AMI.Internal;
using System;
using System.Collections.Generic;

namespace Gurux.DLMS.AMI.Notify
{
    public class GXNotifyListener
    {
        static public int ExpirationTime = 0;
        /// <summary>
        /// Each client has own message queue.
        /// </summary>
        static Dictionary<string, GXNotifyClient> notifyMessages = new Dictionary<string, GXNotifyClient>();

        /// <summary>
        /// Handle received notify message.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal static void OnNotifyReceived(object sender, ReceiveEventArgs e)
        {
            GXNotifyClient reply;
            lock (notifyMessages)
            {
                if (notifyMessages.ContainsKey(e.SenderInfo))
                {
                    reply = notifyMessages[e.SenderInfo];
                }
                else
                {
                    reply = new GXNotifyClient();
                    notifyMessages.Add(e.SenderInfo, reply);
                }
            }
            DateTime now = DateTime.Now;
            //If received data is expired.
            if (ExpirationTime != 0 && (now - reply.DataReceived).TotalSeconds > ExpirationTime)
            {
                reply.Reply.Clear();
            }
            reply.DataReceived = now;
            reply.Reply.Set((byte[])e.Data);
            GXReplyData data = new GXReplyData();
            reply.Client.GetData(reply.Reply, data, reply.Notify);
            // If all data is received.
            if (reply.Notify.IsComplete && !reply.Notify.IsMoreData)
            {
                try
                {
                }
                catch (Exception ex)
                {
                    //TODO: Save error to the database.
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    reply.Notify.Clear();
                    reply.Reply.Clear();
                }
            }
        }

    }
}
