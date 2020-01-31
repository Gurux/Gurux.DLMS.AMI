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

using Gurux.DLMS.AMI.Messages.DB;
using Gurux.DLMS.AMI.Messages.Enums;
using Gurux.DLMS.AMI.Messages.Rest;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Gurux.DLMS.AMI.Internal;

namespace Gurux.DLMS.AMI.Scheduler
{
    internal class GXSchedulerService : IHostedService, IDisposable
    {
        private readonly ILogger _logger;
        private Timer _timer;

        public GXSchedulerService(ILogger<GXSchedulerService> logger)
        {
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Background Service is starting.");
            TimeSpan start = DateTime.Now.AddSeconds(60 - DateTime.Now.Second) - DateTime.Now;
            _timer = new Timer(DoWork, null, start, TimeSpan.FromMinutes(1));
            return Task.CompletedTask;
        }

        private async void DoWork(object state)
        {
            _logger.LogWarning("Timed Background Service is working.");
            try
            {
                DateTime now = DateTime.Now;
                ListSchedulesResponse list = null;
                using (System.Net.Http.HttpResponseMessage response = await Helpers.client.PostAsJsonAsync(Startup.ServerAddress + "/api/schedule/ListSchedules", new ListSchedules()))
                {
                    Helpers.CheckStatus(response);
                    list = await response.Content.ReadAsAsync<ListSchedulesResponse>();
                }
                List<GXTask> tasks = new List<GXTask>();
                foreach (GXSchedule it in list.Schedules)
                {
                    GXDateTime dt = new GXDateTime(it.Start);
                    if (it.ExecutionTime == DateTime.MinValue)
                    {
                        it.ExecutionTime = now;
                    }
                    if (Equals(dt, now))
                    {
                        _logger.LogTrace("+");
                        foreach (GXObject obj in it.Objects)
                        {
                            foreach (GXAttribute a in obj.Attributes)
                            {
                                GXTask t = new GXTask()
                                {
                                    Object = obj,
                                    TaskType = TaskType.Read,
                                    Index = a.Index
                                };
                                tasks.Add(t);
                            }
                        }
                        it.ExecutionTime = now;
                        UpdateScheduleExecutionTime us = new UpdateScheduleExecutionTime();
                        us.Id = it.Id;
                        us.Time = now;
                        using (System.Net.Http.HttpResponseMessage response = await Helpers.client.PostAsJsonAsync(Startup.ServerAddress + "/api/schedule/UpdateScheduleExecutionTime", us))
                        {
                            Helpers.CheckStatus(response);
                            UpdateScheduleExecutionTime r = await response.Content.ReadAsAsync<UpdateScheduleExecutionTime>();
                        }
                    }
                    else if (now.Minute == 0)
                    {
                        Console.WriteLine(dt.ToFormatString());
                        Console.WriteLine(now.ToString());
                    }

                }
                if (tasks.Count != 0)
                {
                    AddTask at = new AddTask();
                    at.Actions = tasks.ToArray();
                    using (System.Net.Http.HttpResponseMessage response = await Helpers.client.PostAsJsonAsync(Startup.ServerAddress + "/api/task/AddTask", at))
                    {
                        Helpers.CheckStatus(response);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
            }
        }
        bool Equals(GXDateTime t, DateTime t2)
        {
            if ((t.Skip & Gurux.DLMS.Enums.DateTimeSkips.Minute) == 0 &&
                t.Value.Minute != t2.Minute)
            {
                return false;
            }
            if ((t.Skip & Gurux.DLMS.Enums.DateTimeSkips.Hour) == 0 &&
                t.Value.Hour != t2.Hour)
            {
                return false;
            }
            if ((t.Skip & Gurux.DLMS.Enums.DateTimeSkips.Day) == 0 &&
                t.Value.Day != t2.Day)
            {
                return false;
            }
            if ((t.Skip & Gurux.DLMS.Enums.DateTimeSkips.Month) == 0 &&
                t.Value.Month != t2.Month)
            {
                return false;
            }
            if ((t.Skip & Gurux.DLMS.Enums.DateTimeSkips.Year) == 0 &&
                t.Value.Month != t2.Year)
            {
                return false;
            }
            return true;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogWarning("Timed Background Service is stopping.");
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
