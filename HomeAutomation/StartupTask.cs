﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using Windows.ApplicationModel.Background;
using Devkoes.Restup.WebServer;
using Devkoes.Restup.WebServer.File;
using Devkoes.Restup.WebServer.Http;
using Devkoes.Restup.WebServer.Rest;
using Windows.System.Threading;

// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace HomeAutomation
{
    public sealed class StartupTask : IBackgroundTask
    {
        internal static BackgroundTaskDeferral Deferral = null;

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            // 
            // TODO: Insert code to perform background work
            //
            // If you start any asynchronous methods here, prevent the task
            // from closing prematurely by using BackgroundTaskDeferral as
            // described in http://aka.ms/backgroundtaskdeferral
            //

            Deferral = taskInstance.GetDeferral();

            await ThreadPool.RunAsync(async workItem => {

                HttpServer httpServer = new HttpServer(80);
                try
                {
                    // initialize webserver
                    var restRouteHandler = new RestRouteHandler();

                    restRouteHandler.RegisterController<Controller.Home.Home>();
                    restRouteHandler.RegisterController<Controller.Web.Web>();
                    restRouteHandler.RegisterController<Controller.PhilipsHUE.Main>();

                    httpServer.RegisterRoute("api", restRouteHandler);
                    httpServer.RegisterRoute(new StaticFileRouteHandler(@"DemoStaticFiles\Web"));

                    await httpServer.StartServerAsync();
                }
                catch (Exception ex)
                {
                    Log.e(ex);
                    httpServer.StopServer();
                    Deferral.Complete();
                }

            }, WorkItemPriority.High);
        }
    }
}
