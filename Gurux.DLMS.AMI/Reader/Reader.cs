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
using System.Net.Http;
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

        internal static void Read(ILogger _logger, HttpClient client, GXDLMSReader reader, GXTask task, IGXMedia net, GXDLMSObject obj)
        {
            AddValue v;
            HttpResponseMessage response;
            if (_logger != null)
            {
                _logger.LogInformation("Reading: " + obj.ToString());
            }
            Console.WriteLine("Reading: " + obj.ToString());
            object val;
            if (obj.ObjectType == ObjectType.ProfileGeneric && task.Index == 2 && task.Object.Attributes[GetBufferIndex(task.Object)].Read != DateTime.MinValue)
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
                                    if (dt < latest)
                                    {
                                        continue;
                                    }
                                    if (dt > latest)
                                    {
                                        latest = dt;
                                    }
                                }
                            }
                        }
                        if (_logger != null)
                        {
                            _logger.LogInformation("Read: " + task.Data);
                        }
                        list.Add(new GXValue()
                        {
                            Read = dt,
                            Value = task.Data,
                            AttributeId = attributeId
                        });
                    }
                    v = new AddValue() { Items = list.ToArray() };
                    response = client.PostAsJsonAsync(Startup.ServerAddress + "/api/value/AddValue", v).Result;
                    Helpers.CheckStatus(response);
                    ListDevicesResponse r = response.Content.ReadAsAsync<ListDevicesResponse>().Result;
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
