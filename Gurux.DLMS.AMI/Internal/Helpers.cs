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
using Gurux.DLMS.AMI.Messages.Rest;
using Gurux.Service.Rest;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading.Tasks;

namespace Gurux.DLMS.AMI.Internal
{
    public static class HttpClientExtensions
    {
        public static Task<HttpResponseMessage> PostAsJsonAsync<T>(
            this HttpClient httpClient, string url, T data)
        {
            var dataAsString = JsonConvert.SerializeObject(data);
            var content = new StringContent(dataAsString);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            return httpClient.PostAsync(url, content);
        }

        public static async Task<T> ReadAsAsync<T>(this HttpContent content)
        {
            var dataAsString = await content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(dataAsString);
        }
    }

    class Helpers
    {
        public class SimpleHttpResponseException : Exception
        {
            public HttpStatusCode StatusCode { get; private set; }

            public SimpleHttpResponseException(HttpStatusCode statusCode, string content) : base(content)
            {
                StatusCode = statusCode;
            }
        }

        public static void CheckStatus(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                return;
            }
            var content = response.Content.ReadAsStringAsync().Result;
            if (response.Content != null)
                response.Content.Dispose();

            throw new SimpleHttpResponseException(response.StatusCode, content);
        }

        public static bool IsNumber(object value)
        {
            return value is sbyte
                    || value is byte
                    || value is short
                    || value is ushort
                    || value is int
                    || value is uint
                    || value is long
                    || value is ulong
                    || value is float
                    || value is double
                    || value is decimal;
        }

        /// <summary>
        /// Find available Rest messages.
        /// </summary>
        public static void UpdateRestMessageTypes(SortedDictionary<string, GXRestMethodInfo> messageMap)
        {
            AuthenticateAttribute[] auths;
            Type tp;
            ParameterInfo[] parameters;
            foreach (Type type in typeof(Helpers).Assembly.GetExportedTypes())
            {
                if (!type.IsAbstract && type.IsClass && typeof(ControllerBase).IsAssignableFrom(type))
                {
                    foreach (MethodInfo method in type.GetMethods())
                    {
                        parameters = method.GetParameters();
                        if (parameters.Length == 1 &&
                            (method.Name == "Post" || method.Name == "Get" || method.Name == "Put" || method.Name == "Delete"))
                        {
                            tp = parameters[0].ParameterType;
                            string name = tp.Name;
                            Microsoft.AspNetCore.Mvc.RouteAttribute[] ra = (Microsoft.AspNetCore.Mvc.RouteAttribute[])tp.GetCustomAttributes(typeof(Microsoft.AspNetCore.Mvc.RouteAttribute), true);
                            if (ra.Length == 1)
                            {
                                name = ra[0].Name;
                            }
                            foreach (var it in tp.GetInterfaces())
                            {
                                if (it.IsGenericType && it.GetGenericTypeDefinition() == typeof(IGXRequest<>))
                                {
                                    GXRestMethodInfo r = new GXRestMethodInfo();
                                    messageMap.Add(name, r);
                                    auths = (AuthenticateAttribute[])method.GetCustomAttributes(typeof(AuthenticateAttribute), true);
                                    if (method.Name == "Post")
                                    {
                                        DescriptionAttribute[] desc = (DescriptionAttribute[])tp.GetCustomAttributes(typeof(DescriptionAttribute), true);
                                        if (desc != null && desc.Length != 0)
                                        {
                                            r.Description = desc[0].Description;
                                        }
                                        ExampleAttribute[] e = (ExampleAttribute[])tp.GetCustomAttributes(typeof(ExampleAttribute), true);
                                        if (e != null && e.Length != 0)
                                        {
                                            r.Url = e[0].Url;
                                            r.Body = e[0].Body;
                                        }
                                        foreach (var p in tp.GetProperties())
                                        {
                                            GXRestParameterInfo pi = new GXRestParameterInfo();
                                            pi.Name = p.Name;
                                            pi.Type = p.PropertyType.Name;
                                            desc = (DescriptionAttribute[])p.GetCustomAttributes(typeof(DescriptionAttribute), true);
                                            if (desc != null && desc.Length != 0)
                                            {
                                                pi.Description = desc[0].Description;
                                            }
                                            r.Parameters.Add(pi);
                                        }
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
