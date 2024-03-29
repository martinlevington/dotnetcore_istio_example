﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using docker_helloWorld.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace docker_helloWorld
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddConsole();
                loggingBuilder.AddDebug();
            });

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });


            // Registers required services for health checks
            services.AddHealthChecks()
            .AddCheck<ReadyHealthCheck>("Ready", failureStatus: null, tags: new[] { "ready", });


            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
           

            var logger = loggerFactory.CreateLogger<Startup>();

            logger.LogInformation($"Host: {Environment.MachineName}");
            logger.LogInformation($"EnvironmentName: {env.EnvironmentName}");
            logger.LogInformation($"PingService:Url: {Configuration["PingService:Url"]}");
            logger.LogInformation($"HelloWorld:Url: {Configuration["HelloWorld:Url"]}");

            // The readiness check uses all registered checks with the 'ready' tag.
            app.UseHealthChecks("/meta/health/ready", new HealthCheckOptions()
            {
                Predicate = (check) => check.Tags.Contains("ready"),
            });

            // The liveness filters out all checks and just returns success
            app.UseHealthChecks("/meta/health/live", new HealthCheckOptions()
            {
                // Exclude all checks, just return a 200.
                Predicate = (check) => false,
            });


            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
