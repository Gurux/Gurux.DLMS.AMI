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
using Gurux.DLMS.Enums;
using Gurux.DLMS.Objects;
using Gurux.DLMS.AMI.Messages.DB;
using Gurux.DLMS.AMI.Messages.Rest;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using Gurux.DLMS.AMI.Internal;

namespace Gurux.DLMS.AMI.Reader
{
    class Reader
    {
        private static int GetAttributeIndex(GXObject obj, int index)
        {
            int index2 = 0;
            foreach (GXAttribute it in obj.Attributes)
            {
                if (it.Index == index)
                {
                    return index2;
                }
                ++index2;
            }
            return -1;
        }

        private static int GetBufferIndex(GXObject obj)
        {
            return GetAttributeIndex(obj, 2);
        }

        /// <summary>
        /// Get capture period of the profile generic.
        /// </summary>
        private static int GetCapturePeriod(GXObject obj)
        {
            int index = GetAttributeIndex(obj, 4);
            if (index == -1)
            {
                return -1;
            }
            return Convert.ToInt32(obj.Attributes[index].Value);
        }

        /// <summary>
        /// Get last read time.
        /// </summary>
        private static DateTime GetReadTime(GXObject obj)
        {
            int index = GetAttributeIndex(obj, 4);
            if (index == -1)
            {
                return DateTime.MinValue;
            }
            return obj.Attributes[index].Read;
        }

        /// <summary>
        /// Is profile generic object sorted using date time.
        /// </summary>
        private static bool IsSortedByDateTime(GXObject obj)
        {
            int index = GetAttributeIndex(obj, 3);
            if (index == -1)
            {
                return false;
            }
            try
            {
                GXArray tmp = GXDLMSTranslator.XmlToValue(obj.Attributes[index].Value) as GXArray;
                if (tmp != null)
                {
                    //If interface type is clock.
                    return (UInt16)((GXStructure)tmp[0])[0] == 8;
                }
            }
            catch(Exception)
            {
                //It's OK if this fails.
            }
            return false;
        }

        internal static void Read(ILogger _logger, System.Net.Http.HttpClient client, GXDLMSReader reader, GXTask task, IGXMedia net, GXDLMSObject obj)
        {
            AddValue v;
            System.Net.Http.HttpResponseMessage response;
            if (_logger != null)
            {
                _logger.LogInformation("Reading: " + obj.ToString());
            }
            Console.WriteLine("Reading: " + obj.ToString());
            object val;
            if (obj.ObjectType == ObjectType.ProfileGeneric && task.Index == 2 && GetReadTime(task.Object) != DateTime.MinValue && IsSortedByDateTime(task.Object))
            {
                //Read profile generic using range.
                DateTime now = DateTime.Now;
                now = now.AddSeconds(-now.Second);
                now = now.AddMinutes(-now.Minute);
                now = now.AddHours(1);
                val = reader.ReadRowsByRange((GXDLMSProfileGeneric)obj, task.Object.Attributes[GetBufferIndex(task.Object)].Read, now);
            }
            else
            {
                val = reader.Read(obj, task.Index);
            }
            if (val is Enum)
            {
                //Enum values are saved as interger.
                val = Convert.ToString(Convert.ToInt32(val));
            }
            else if (val is byte[])
            {
                DataType dt = (DataType)task.Object.Attributes[GetBufferIndex(task.Object)].UIDataType;
                if (dt == DataType.String)
                {
                    val = ASCIIEncoding.ASCII.GetString((byte[])val);
                }
                else if (dt == DataType.StringUTF8)
                {
                    val = ASCIIEncoding.UTF8.GetString((byte[])val);
                }
                else
                {
                    val = GXCommon.ToHex((byte[])val);
                }
            }
            else if (val is GXDateTime)
            {
                val = ((GXDateTime)val).ToFormatString();
            }
            if (obj.ObjectType == ObjectType.ProfileGeneric && task.Index == 2)
            {
                //Make own value from each row.
                if (val != null)
                {
                    UInt64 attributeId = task.Object.Attributes[GetBufferIndex(task.Object)].Id;
                    List<GXValue> list = new List<GXValue>();
                    DateTime latest = task.Object.Attributes[GetBufferIndex(task.Object)].Read;
                    DateTime first = latest;
                    Boolean read = false;
                    int period = -1;
                    foreach (GXStructure row in (GXArray)val)
                    {
                        DateTime dt = DateTime.MinValue;
                        task.Data = GXDLMSTranslator.ValueToXml(row);
                        for (int pos = 0; pos != row.Count; ++pos)
                        {
                            if (row[pos] is byte[])
                            {
                                row[pos] = GXDLMSClient.ChangeType((byte[])row[pos], DataType.DateTime);
                                if (pos == 0)
                                {
                                    dt = ((GXDateTime)row[pos]).Value.LocalDateTime;
                                    //If we have already read this row.
                                    if (dt <= first)
                                    {
                                        read = true;
                                        break;
                                    }
                                    if (dt > latest)
                                    {
                                        latest = dt;
                                    }
                                }
                            }
                            //Some meters are returning null as date time to save bytes...
                            if (pos == 0 && row[pos] == null)
                            {
                                if (period == -1)
                                {
                                    period = GetCapturePeriod(task.Object);
                                    if (period == -1)
                                    {
                                        throw new Exception("Invalid capture period.");
                                    }
                                }
                                row[pos] = latest.AddMinutes(period);
                            }
                        }
                        if (_logger != null)
                        {
                            _logger.LogInformation("Read: " + task.Data);
                        }
                        if (!read)
                        {
                            list.Add(new GXValue()
                            {
                                Read = dt,
                                Value = task.Data,
                                AttributeId = attributeId
                            });
                        }
                    }
                    if (list.Count != 0)
                    {
                        v = new AddValue() { Items = list.ToArray() };
                        response = client.PostAsJsonAsync(Startup.ServerAddress + "/api/value/AddValue", v).Result;
                        Helpers.CheckStatus(response);
                        ListDevicesResponse r = response.Content.ReadAsAsync<ListDevicesResponse>().Result;
                    }
                }
            }
            else
            {
                if (!(val is string))
                {
                    val = Convert.ToString(val);
                }
                task.Data = (string)val;
                if (_logger != null)
                {
                    _logger.LogInformation("Read: " + (string)val);
                }
                int index = GetAttributeIndex(task.Object, task.Index);
                if (index != -1)
                {
                    UInt64 attributeId = task.Object.Attributes[index].Id;
                    v = new AddValue()
                    {
                        Items = new GXValue[] {new GXValue(){
                            AttributeId = attributeId,
                            Read = DateTime.Now,
                            Value = (string)val}
                        }
                    };
                    response = client.PostAsJsonAsync(Startup.ServerAddress + "/api/value/AddValue", v).Result;
                    Helpers.CheckStatus(response);
                    AddValueResponse r = response.Content.ReadAsAsync<AddValueResponse>().Result;
                }
                else
                {
                    if (_logger != null)
                    {
                        _logger.LogInformation("Invalid task index: " + task.Index);
                    }
                }
            }
        }
    }
}
