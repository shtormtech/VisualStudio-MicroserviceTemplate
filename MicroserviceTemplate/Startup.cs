using System;
using System.Collections.Generic;
using System.IO;

using MicroserviceTemplate.Config;

using MicroServiceTemplate.Interfaces;
using MicroServiceTemplate.Services;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
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
        public SwaggerConfig SwaggerConfig => Configuration.GetSection(BaseSectionConfig).Get<BaseConfiguration>().SwaggerConfig;
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

            services.AddControllers().AddNewtonsoftJson();
            services.AddHealthChecks();
            services.AddOptions();
            services.Configure<BaseConfiguration>(Configuration.GetSection(BaseSectionConfig));

            // Add swagger
            if (SwaggerConfig.IsEnabled)
            {
                services
                .AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v0", new OpenApiInfo
                    {
                        Version = "v0",
                        Title = "Swagger $safeprojectname$",
                        Description = "Swagger $safeprojectname$ (ASP.NET Core 3.1)"
                    });
                    c.CustomSchemaIds(type => type.FullName);
                    var xmlPath = $"{AppContext.BaseDirectory}{Path.DirectorySeparatorChar}{_hostingEnv.ApplicationName}.xml";
                    if (File.Exists(xmlPath))
                    {
                        c.IncludeXmlComments(xmlPath);
                    }
                });
                services.AddSwaggerGenNewtonsoftSupport();
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

            if (SwaggerConfig.IsEnabled)
            {
                app.UseSwagger(c =>
                {
                    c.RouteTemplate = "swagger/{documentName}/swagger.json";
                    c.PreSerializeFilters.Add((swaggerDoc, httpReq) =>
                    {
                        //Список серверов нужен для подстановки верного префикса при тестировании API через интерфейс swagger,
                        //при расположении сервиса за ingress контроллером
                        //TODO: схемы HTTP и HTTPS принудительно добавлены, так как пока нет возможности узнать верную схему при работе через балансировщик.
                        //нужно найти вариант самостоятелного определения верной схемы
                        //p.s. httpReq.Scheme не работает при изменении схемы после балансировщика
                        swaggerDoc.Servers = new List<OpenApiServer> {
                            new OpenApiServer { Url = $"http://{httpReq.Host.Value}/{SwaggerConfig.EndpointPrefix.Trim('/')}" },
                            new OpenApiServer { Url = $"https://{httpReq.Host.Value}/{SwaggerConfig.EndpointPrefix.Trim('/')}" }
                        };
                    });
                });
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint($"{SwaggerConfig.EndpointPrefix.TrimEnd('/')}/swagger/v0/swagger.json", "$safeprojectname$ API V0");
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
