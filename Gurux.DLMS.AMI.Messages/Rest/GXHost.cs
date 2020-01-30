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
using Gurux.DLMS.AMI.Messages.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Gurux.DLMS.AMI.Messages.Rest
{
    public class GXHost
    {
        Dictionary<TargetType, DateTime> changes = new Dictionary<TargetType, DateTime>();
        List<KeyValuePair<TargetType, AutoResetEvent>> waitList = new List<KeyValuePair<TargetType, AutoResetEvent>>();

        public void SetChange(TargetType type, DateTime time)
        {
            lock(changes)
            {
                changes[type] = time;
                foreach (KeyValuePair<TargetType, AutoResetEvent> it in waitList)
                {
                    if (it.Key == TargetType.None || (it.Key & type) != 0)
                    {
                        it.Value.Set();
                    }
                }
            }
        }

        public TargetType GetChange(TargetType type, DateTime time, out DateTime when)
        {
            when = DateTime.MinValue;
            TargetType ret = TargetType.None;
            lock (changes)
            {
                foreach (var it in changes)
                {
                    if ((type == TargetType.None || (type & it.Key) != 0))
                    {
                        if ((it.Value.ToUniversalTime() - time.ToUniversalTime()).Seconds > 0)
                        {
                            ret |= it.Key;
                            if (when == DateTime.MinValue || (it.Value.ToUniversalTime() - when.ToUniversalTime()).Seconds > 0)
                            {
                                when = it.Value;
                            }
                        }
                    }
                }
            }
            return ret;
        }

        /// <summary>
        /// Wait until state has change.
        /// </summary>
        /// <param name="type">Change type.</param>
        /// <param name="h">Event handle.</param>
        /// <param name="waitTime">Wait time in seconds.</param>
        /// <returns>True if event has occurred.</returns>
        public bool WaitChange(TargetType type, AutoResetEvent h, int waitTime)
        {
            lock (changes)
            {
                waitList.Add(new KeyValuePair<TargetType, AutoResetEvent>(type, h));
            }
            if (waitTime == 0)
            {
               return h.WaitOne();
            }
            return h.WaitOne(waitTime * 1000);
        }

        /// <summary>
        /// Wait until state has change.
        /// </summary>
        /// <param name="type">Change type.</param>
        /// <param name="h">Event handle.</param>
        /// <param name="closing">Closing event handle.</param>
        /// <param name="waitTime">Wait time in seconds.</param>
        /// <returns>True if event has occurred.</returns>
        public bool WaitChange(TargetType type, AutoResetEvent h, WaitHandle closing, int waitTime)
        {
            lock (changes)
            {
                waitList.Add(new KeyValuePair<TargetType, AutoResetEvent>(type, h));
            }
            if (waitTime == 0)
            {
                return WaitHandle.WaitAny(new WaitHandle[] { h, closing }) == 0;
            }
            return WaitHandle.WaitAny(new WaitHandle[] { h, closing }, waitTime) == 0;
        }

        public void CancelChange(AutoResetEvent h)
        {
            lock (changes)
            {
                foreach(KeyValuePair<TargetType, AutoResetEvent>  it in waitList)
                {
                    if (it.Value == h)
                    {
                        waitList.Remove(it);
                        break;
                    }
                }
            }
        }


        public GXDbConnection Connection
        {
            get;
            set;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (Connection == null)
            {
                sb.AppendLine("Invalid DB connection.");
            }
            else
            {
                sb.AppendLine("DB connected to: " + Connection.Connection.ConnectionString);
                sb.AppendLine("DB connection state: " + Connection.Connection.State);
            }
            return sb.ToString();
        }
    }
}
