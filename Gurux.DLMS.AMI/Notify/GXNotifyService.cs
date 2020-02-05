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
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Gurux.DLMS.AMI.Internal;
using Gurux.Net;
using Microsoft.Extensions.Options;
using Gurux.Common;

namespace Gurux.DLMS.AMI.Notify
{
    internal class GXNotifyService : IHostedService, IDisposable
    {
        bool _useLogicalNameReferencing;
        string _systemTitle;
        string _blockCipherKey;
        int _interfaceType;
        private readonly ILogger _logger;
        //Notify wait push, events or notifies from the meters.
        GXNet notify;

        static public int ExpirationTime = 0;
        /// <summary>
        /// Each client has own message queue.
        /// </summary>
        static Dictionary<string, GXNotifyClient> notifyMessages = new Dictionary<string, GXNotifyClient>();

        public GXNotifyService(ILogger<GXNotifyService> logger, IOptions<NotifyOptions> optionsAccessor)
        {
            _useLogicalNameReferencing = optionsAccessor.Value.UseLogicalNameReferencing;
            _systemTitle = optionsAccessor.Value.SystemTitle;
            _blockCipherKey = optionsAccessor.Value.BlockCipherKey;
            _interfaceType = optionsAccessor.Value.Interface;
            _logger = logger;
            notify = new GXNet((NetworkType)optionsAccessor.Value.NetworkType, optionsAccessor.Value.Port);
            ExpirationTime = optionsAccessor.Value.ExpirationTime;
            notify.OnReceived += OnNotifyReceived;
            _logger.LogInformation("Listening notifications in port: " + notify.Port);
            notify.Open();
            /*
          if (!string.IsNullOrEmpty(n.Parser))
          {
              string[] tmp = n.Parser.Split(";");
              //GXNotifyListener.Parser = new Gurux.DLMS.AMI.NotifyParser.GXNotifyParser();
              string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), tmp[0]);
              Assembly asm = Assembly.LoadFile(path);
              foreach (Type type in asm.GetTypes())
              {
                  //if (!type.IsAbstract && type.IsClass && typeof(IGXNotifyParser).IsAssignableFrom(type))
                  {
                      GXNotifyListener.Parser = Activator.CreateInstance(type) as IGXNotifyParser;
                      break;
                  }
              }
              //GXNotifyListener.Parser = asm.CreateInstance(tmp[1]) as IGXNotifyParser;
          }
          */
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Notify service is starting.");
            return Task.CompletedTask;
        }

        /// <summary>
        /// Handle received notify message.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal void OnNotifyReceived(object sender, ReceiveEventArgs e)
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
                    reply = new GXNotifyClient(_useLogicalNameReferencing, _interfaceType, _systemTitle, _blockCipherKey);
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
                    _logger.LogError(ex.Message);
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

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogWarning("Notify service is stopping.");
            if (notify != null)
            {
                notify.OnReceived -= OnNotifyReceived;
                notify.Close();
            }
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            if (notify != null)
            {
                notify.Dispose();
                notify = null;
            }
        }
    }
}
