using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using MicroServiceTemplate.Interfaces;
using MicroServiceTemplate.Services;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace MicroServiceTemplate
{
    public class Startup
    {
        const string BaseSectionConfig = "BaseConfiguration";
        public bool SwaggerIsEnabled => Configuration.GetSection(BaseSectionConfig).Get<BaseConfiguration>().SwaggerIsEnabled;
        private readonly IWebHostEnvironment _hostingEnv;
        public IConfiguration Configuration { get; }
        public Startup(IWebHostEnvironment env)
        {
            _hostingEnv = env;
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: false)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IWeatherForecastService, WeatherForecastService>();

            services.AddControllers();
            services.AddHealthChecks();
            services.AddOptions();
            services.Configure<BaseConfiguration>(Configuration.GetSection(BaseSectionConfig));

            // Add swagger
            if (SwaggerIsEnabled)
            {
                services
                .AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v0", new OpenApiInfo
                    {
                        Version = "v0",
                        Title = "Swagger MicroServiceTemplate",
                        Description = "Swagger MicroServiceTemplate (ASP.NET Core 3.1)"
                    });
                    c.CustomSchemaIds(type => type.FullName);
                    var xmlPath = $"{AppContext.BaseDirectory}{Path.DirectorySeparatorChar}{_hostingEnv.ApplicationName}.xml";
                    if (File.Exists(xmlPath))
                    {
                        c.IncludeXmlComments(xmlPath);
                    }
                });
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                //TODO: Enable production exception handling (https://docs.microsoft.com/en-us/aspnet/core/fundamentals/error-handling)
                app.UseExceptionHandler("/Error");
            }


            app.UseRouting();

            app.UseAuthorization();

            if (SwaggerIsEnabled)
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("../swagger/v0/swagger.json", "MicroServiceTemplate API V0");
                    c.RoutePrefix = string.Empty;
                });
            }

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseHealthChecks("/health");
        }
    }
}
