﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace docker_helloWorld
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>

            // kubectl create secret generic helloworld-appsettings --from-file=./appsettings.docker.json
            WebHost.CreateDefaultBuilder(args)
              .ConfigureAppConfiguration((buildercontext, config) =>
              {
                  config.AddJsonFile("settings/appsettings.docker.json", optional: true);
              })
                .UseStartup<Startup>();
    }
}
