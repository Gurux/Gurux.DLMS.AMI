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

namespace Gurux.DLMS.AMI.Messages.Rest
{
    public class ExampleAttribute : Attribute
    {
        /// <summary>
        /// Example url.
        /// </summary>
        public string Url
        {
            get;
            private set;
        }
        //Example body.
        public string Body
        {
            get;
            private set;
        }

        //Help url.
        public string Help
        {
            get;
            private set;
        }

        /// <summary>
        /// Constrctor.
        /// </summary>
        /// <param name="method">Example url.</param>
        /// <param name="body">Example body.</param>
        /// <param name="help">Help url.</param>
        public ExampleAttribute(string url, string body, string help)
        {
            Url = url;
            Body = body;
            Help = help;
        }
    }
}
