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
using System.Data.Common;
using System.Data.SqlClient;
using Gurux.DLMS.Enums;
using Gurux.Service.Orm;
using Gurux.DLMS.AMI.Messages.DB;
using Gurux.DLMS.AMI.Messages.Rest;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Oracle.ManagedDataAccess.Client;
using System.Data.SQLite;
using Gurux.Net;
using Gurux.DLMS.AMI.Internal;
using Gurux.DLMS.AMI.Notify;
using Microsoft.Extensions.Hosting;
using Gurux.DLMS.AMI.Reader;

namespace Gurux.DLMS.AMI
{
    public class Startup
    {
        static readonly System.Net.Http.HttpClient httpClient = new System.Net.Http.HttpClient();

        //Listener wait incoming connections from the meters.
        GXNet listener;

        //Notify wait push, events or notifies from the meters.
        GXNet notify;

        public static string ServerAddress
        {
            get;
            set;
        }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            int port = configuration.GetSection("Listener").Get<ListenerOptions>().Port;
            if (port != 0)
            {
                listener = new GXNet(NetworkType.Tcp, port);
                listener.OnClientConnected += GXListener.OnClientConnected;
                Console.WriteLine("Listening port:" + listener.Port);
                listener.Open();
            }

            NotifyOptions n = configuration.GetSection("Notify").Get<NotifyOptions>();
            port = n.Port;
            if (port != 0)
            {
                notify = new GXNet(NetworkType.Tcp, port);
                GXNotifyListener.ExpirationTime = n.ExpirationTime;
                notify.OnReceived += GXNotifyListener.OnNotifyReceived;
                Console.WriteLine("Notifing port:" + notify.Port);
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
                notify.Open();
            }
            ServerAddress = configuration.GetSection("Client").Get<ClientOptions>().Address;
            Console.WriteLine("RestAddress: " + ServerAddress);
        }

        public IConfiguration Configuration { get; }

        private async void AddSchedule()
        {
            GXSchedule m = new GXSchedule();
            m.Name = "Minutely";
            GXDateTime dt = new GXDateTime(DateTime.Now.Date);
            dt.Skip = DateTimeSkips.Year | DateTimeSkips.Month | DateTimeSkips.Day | DateTimeSkips.Hour | DateTimeSkips.Minute;
            m.Start = dt.ToFormatString();
            UpdateSchedule us = new UpdateSchedule();
            us.Schedules.Add(m);
            GXSchedule h = new GXSchedule();
            h.Name = "Hourly";
            dt.Skip = DateTimeSkips.Year | DateTimeSkips.Month | DateTimeSkips.Day | DateTimeSkips.Hour;
            h.Start = dt.ToFormatString();
            us.Schedules.Add(h);
            GXSchedule d = new GXSchedule();
            d.Name = "Daily";
            dt.Skip = DateTimeSkips.Year | DateTimeSkips.Month | DateTimeSkips.Day;
            d.Start = dt.ToFormatString();
            us.Schedules.Add(d);
            using (System.Net.Http.HttpResponseMessage response = await httpClient.PostAsJsonAsync(Startup.ServerAddress + "/api/schedule/UpdateSchedule", us))
            {
                Helpers.CheckStatus(response);
                UpdateScheduleResponse r = await response.Content.ReadAsAsync<UpdateScheduleResponse>();
            }
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
#if !NETCOREAPP2_0 && !NETCOREAPP2_1
            services.AddControllers();
#endif//!NETCOREAPP2_0 && !NETCOREAPP2_1

            if (!Configuration.GetSection("Scheduler").Get<SchedulerOptions>().Disabled)
            {
                services.AddHostedService<TimedHostedService>();
            }

            string settings = Configuration.GetSection("Database").Get<DatabaseOptions>().Settings;
            string type = Configuration.GetSection("Database").Get<DatabaseOptions>().Type;
            Console.WriteLine("Database type: " + type);
            Console.WriteLine("Connecting: " + settings);
            DbConnection connection;
            if (string.IsNullOrEmpty(type))
            {
                //Gurux.DLMS.AMI DB is defined elsewhere.
                connection = null;
            }
            if (string.Compare(type, "Oracle", true) == 0)
            {
                connection = new OracleConnection(settings);
            }
            else if (string.Compare(type, "MSSQL", true) == 0)
            {
                connection = new SqlConnection(settings);
            }
            else if (string.Compare(type, "MySQL", true) == 0)
            {
                connection = new MySql.Data.MySqlClient.MySqlConnection(settings);
            }
            else if (string.Compare(type, "SQLite", true) == 0)
            {
                connection = new SQLiteConnection(settings);
            }
            else
            {
                throw new Exception("Invalid connection type. " + type);
            }
            if (connection != null)
            {
                connection.Open();
                GXHost h = new GXHost()
                {
                    Connection = new GXDbConnection(connection, null)
                };
                if (!h.Connection.TableExist<GXDevice>())
                {
                    Console.WriteLine("Creating tables.");
                    h.Connection.CreateTable<GXSystemError>(false, false);
                    h.Connection.CreateTable<GXDeviceTemplate>(false, false);
                    h.Connection.CreateTable<GXDevice>(false, false);
                    h.Connection.CreateTable<GXObjectTemplate>(false, false);
                    h.Connection.CreateTable<GXAttributeTemplate>(false, false);
                    h.Connection.CreateTable<GXObject>(false, false);
                    h.Connection.CreateTable<GXAttribute>(false, false);
                    h.Connection.CreateTable<GXValue>(false, false);
                    h.Connection.CreateTable<GXTask>(false, false);
                    h.Connection.CreateTable<GXError>(false, false);
                    h.Connection.CreateTable<GXSchedule>(false, false);
                    h.Connection.CreateTable<GXScheduleToAttribute>(false, false);
                    h.Connection.CreateTable<GXSchedulerInfo>(false, false);
                    h.Connection.CreateTable<GXReaderInfo>(false, false);
                    h.Connection.CreateTable<GXDeviceToReader>(false, false);
                    try
                    {
                        AddSchedule();
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine("Failed to add schedules: " + ex.Message);
                    }
                }
                else
                {
                    h.Connection.UpdateTable<GXSystemError>();
                    h.Connection.UpdateTable<GXError>();
                    h.Connection.UpdateTable<GXReaderInfo>();
                    h.Connection.UpdateTable<GXObjectTemplate>();
                    h.Connection.UpdateTable<GXAttributeTemplate>();
                    h.Connection.UpdateTable<GXDeviceTemplate>();
                    h.Connection.UpdateTable<GXObject>();
                    h.Connection.UpdateTable<GXAttribute>();
                    h.Connection.UpdateTable<GXDevice>();
                }
                h.Connection.Insert(GXInsertArgs.Insert(new GXSystemError()
                {
                    Generation = DateTime.Now,
                    Error = "Service started: " + ServerAddress
                })); ;
                Console.WriteLine("Service started: " + ServerAddress);
                services.AddScoped<GXHost>(q =>
                {
                    return h;
                });
            }
            services.Configure<ReaderOptions>(Configuration.GetSection("Reader"));
            if (!Configuration.GetSection("Scheduler").Get<SchedulerOptions>().Disabled)
            {
                services.AddHostedService<TimedHostedService>();
            }
            if (Configuration.GetSection("Reader").Get<ReaderOptions>().Threads != 0)
            {
                services.AddHostedService<ReaderService>();
            }
#if NETCOREAPP2_0 || NETCOREAPP2_1
            services.AddMvc().SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Version_2_1);
#endif //NETCOREAPP2_0 || NETCOREAPP2_1
        }

#if NETCOREAPP2_0 || NETCOREAPP2_1
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, Microsoft.AspNetCore.Hosting.IHostingEnvironment env)
        {
            //Add exception handler.
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }
            app.UseMvc();
        }
#else
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
#endif //NETCOREAPP2_0 || NETCOREAPP2_1
    }
}
