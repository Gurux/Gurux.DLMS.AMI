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
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Xml;
using System.Xml.Serialization;

namespace Gurux.DLMS.AMI.UI
{
    internal class Password
    {
        //Don't change this.
        public static string Key = "Gurux Ltd.";
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
            if (string.IsNullOrEmpty(content))
            {
                throw new HttpException((int) response.StatusCode, HttpWorkerRequest.GetStatusDescription((int)response.StatusCode));
            }
            throw new SimpleHttpResponseException(response.StatusCode, content);
        }

        public static string[] GetServerAddress()
        {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "GXDLMSDirector");
            path = Path.Combine(path, "gurux.dlms.ami.settings");
            if (File.Exists(path) && new FileInfo(path).Length != 0)
            {
                using (XmlReader reader = XmlReader.Create(path))
                {
                    XmlSerializer x = new XmlSerializer(typeof(GuruxAmiSettings));
                    GuruxAmiSettings addresses = (GuruxAmiSettings)x.Deserialize(reader);
                    return addresses.Servers;
                }
            }
            return new string[0];
        }
    }
}
