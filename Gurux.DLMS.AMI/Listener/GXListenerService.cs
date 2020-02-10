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
using System.Threading;
using System.Threading.Tasks;
using Gurux.DLMS.AMI.Internal;
using Gurux.Net;
using Microsoft.Extensions.Options;

namespace Gurux.DLMS.AMI.Notify
{
    internal class GXListenerService : IHostedService, IDisposable
    {
        private readonly ILogger _logger;
        //Listener wait incoming connections from the meters.
        GXNet listener;


        public GXListenerService(ILogger<GXListenerService> logger, IOptions<ListenerOptions> optionsAccessor)
        {
            _logger = logger;
            int port = optionsAccessor.Value.Port;
            GXListener._logger = logger;
            listener = new GXNet((NetworkType)optionsAccessor.Value.NetworkType, port);
            if (listener.Protocol == NetworkType.Tcp)
            {
                listener.OnClientConnected += GXListener.OnClientConnected;
            }
            else
            {
                listener.OnReceived += GXListener.OnOnReceived;
            }
            _logger.LogInformation("Listening incoming connections in port:" + listener.Port);
            listener.Open();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Listener service is starting.");
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogWarning("Listener service is stopping.");
            if (listener != null)
            {
                if (listener.Protocol == NetworkType.Tcp)
                {
                    listener.OnClientConnected -= GXListener.OnClientConnected;
                }
                else
                {
                    listener.OnReceived -= GXListener.OnOnReceived;
                }
                listener.Close();
            }
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            if (listener != null)
            {
                listener.Dispose();
                listener = null;
            }
        }
    }
}
