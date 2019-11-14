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
using Gurux.DLMS.AMI.Messages.Rest;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text;
using Gurux.DLMS.AMI.Internal;
using System.Collections.Generic;
using System.Diagnostics;

namespace DBService.Controllers
{
    [Microsoft.AspNetCore.Mvc.Route("api/[controller]")]
    [ApiController]
    public class InfoController : ControllerBase
    {
        private readonly string sqlSettings;
        SortedDictionary<string, GXRestMethodInfo> MessageMap = new SortedDictionary<string, GXRestMethodInfo>();

        public InfoController(GXHost value)
        {
            sqlSettings = value.Connection.Builder.Settings.Type.ToString();
            Helpers.UpdateRestMessageTypes(MessageMap);
        }

        [HttpGet]
        public ContentResult Get()
        {
            //Get version info
            System.Reflection.Assembly asm = typeof(InfoController).Assembly;
            FileVersionInfo info = FileVersionInfo.GetVersionInfo(asm.Location);
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<!DOCTYPE html >");
            sb.AppendLine("<html>");
            sb.AppendLine("<style type=\"text/css\">");
            sb.AppendLine(".tooltip {");
            sb.AppendLine("position: relative;");
            sb.AppendLine("display: inline-block;");
            sb.AppendLine("border-bottom: 1px dotted black;");
            sb.AppendLine("}");
            sb.AppendLine(".tooltip .tooltiptext {");
            sb.AppendLine("visibility: hidden;");
            sb.AppendLine("width: 600px;");
            sb.AppendLine("background-color: Gray;");
            sb.AppendLine("color: #fff;");
            sb.AppendLine("text-align: left;");
            sb.AppendLine("border-radius: 6px;");
            sb.AppendLine("padding: 5px 0;");
            //Position the tooltip
            sb.AppendLine("position: absolute;");
            sb.AppendLine("z-index: 1;");
            sb.AppendLine("}");
            sb.AppendLine(".tooltip:hover .tooltiptext {");
            sb.AppendLine("visibility: visible;");
            sb.AppendLine("}");
            sb.AppendLine("ul.nav li{");
            sb.AppendLine("position:relative;padding-left:30px;padding-top:9px;border-top:1px solid #ccc;margin-left:0;margin-bottom:10px; background: url(https://www.gurux.fi/themes/seven/images/list-item.png) bottom left no-repeat;}");
            sb.AppendLine("</style>");
            sb.AppendLine("<body>");
            sb.Append("<table width=\"100%\">");
            sb.Append("<tr>");
            sb.Append("<td><center><h1>Gurux.DLMS.AMI supported operations:</h1></center></td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<td><center>Using " + sqlSettings + " database.</center></td>");
            sb.Append("<td><right>Version: " + info.FileVersion + ".</right></td>");
            sb.Append("</tr>");
            sb.Append("</table>");
            sb.Append("<ul class=\"nav\">");
            foreach (var it in MessageMap)
            {
                StringBuilder sb2 = new StringBuilder();
                if (it.Value.Parameters.Count != 0)
                {
                    sb2.Append("<p/>");
                    sb2.Append("{");
                    sb2.Append("<br/>");
                    foreach (GXRestParameterInfo p in it.Value.Parameters)
                    {
                        sb2.Append(p.Name);
                        sb2.Append(": ");
                        sb2.Append(p.Description);
                        sb2.Append(",<br/>");
                    }
                    sb2.Append("}");
                }
                if (!string.IsNullOrEmpty(it.Value.Url))
                {
                    sb2.Append("<p/>");
                    sb2.Append("REST Example:");
                    sb2.Append("<hr/>");

                    sb2.Append("<p/>");
                    sb2.Append("URL:");
                    sb2.Append("<br/>");
                    sb2.Append(this.Request.Scheme + "://" + this.Request.Host + it.Value.Url);
                    sb2.Append("<p/>");
                    sb2.Append("Body:");
                    sb2.Append("<br/>");
                    if (!string.IsNullOrEmpty(it.Value.Body))
                    {
                        sb2.Append(it.Value.Body.Replace("\r", "<br/>").Replace("\t", "&ensp;"));
                    }
                    else
                    {
                        sb2.Append("{<br/>}");
                    }
                }
                sb.Append("<li>");
                sb.Append("<a target =\"_blank\"" + ">" + "</a>");
                sb.Append("<div class=\"tooltip\">" + it.Key);
                sb.Append("<span class=\"tooltiptext\">");
                sb.Append(sb2.ToString());
                sb.Append("</span></div>");
                sb.Append("<div class=\"description\">");
                sb.Append(it.Value.Description);
                sb.Append("</div>");
                sb.Append("</li>");
                sb.Append("<hr></hr>");
                //< div class="description">
                sb.Append("<p/>");
            }
            sb.Append("</ul>");
            sb.Append("</body></http>");
            return new ContentResult
            {
                ContentType = "text/html",
                StatusCode = (int)HttpStatusCode.OK,
                Content = sb.ToString()
            };
    }
}
}
