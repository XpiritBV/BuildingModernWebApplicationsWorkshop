using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RetroGamingWebAPI.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NSwag.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using NJsonSchema.Generation;
using NSwag.Generation.Processors;

namespace RetroGamingWebAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostEnvironment hostEnvironment)
        {
            Configuration = configuration;
            HostEnvironment = hostEnvironment;
        }

        public IConfiguration Configuration { get; }
        public IHostEnvironment HostEnvironment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<RetroGamingContext>(options => {
                options.UseInMemoryDatabase(databaseName: "RetroGaming");
            });

            services.AddTransient<IMailService, MailService>();

            services.AddOpenApiDocument(document =>
            {
                document.OperationProcessors.Add(new ApiVersionProcessor() { IncludedVersions = new string[] { "1.0" } });
                document.DocumentName = "v1";
                document.PostProcess = d => d.Info.Title = "Retro Gaming Web API v1.0 OpenAPI";
            });
            services.AddOpenApiDocument(document =>
            {
                document.OperationProcessors.Add(new ApiVersionProcessor() { IncludedVersions = new string[] { "2.0" } });
                document.DocumentName = "v2";
                document.PostProcess = d => d.Info.Title = "Retro Gaming Web API v2.0 OpenAPI";
            });

            ConfigureVersioning(services);
            ConfigureSecurity(services);

            services
                .AddControllers(options => {
                    options.RespectBrowserAcceptHeader = true;
                    options.ReturnHttpNotAcceptable = true;
                    options.FormatterMappings.SetMediaTypeMappingForFormat("xml", new MediaTypeHeaderValue("application/xml"));
                    options.FormatterMappings.SetMediaTypeMappingForFormat("json", new MediaTypeHeaderValue("application/json"));
                })
                .AddNewtonsoftJson(setup => {
                    setup.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                })
                .AddXmlSerializerFormatters();
        }

        private void ConfigureVersioning(IServiceCollection services)
        {
            services.AddApiVersioning(options =>
            {
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(2, 0);
                options.ReportApiVersions = true;
                options.ApiVersionReader = new UrlSegmentApiVersionReader();
            });
        }

        private void ConfigureSecurity(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                   builder => builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader()
                );
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,
            RetroGamingContext context)
        {
            app.UseCors("CorsPolicy");
            app.UseOpenApi(config =>
            {
                config.DocumentName = "v1";
                config.Path = "/openapi/v1.json";
            });
            app.UseOpenApi(config =>
            {
                config.DocumentName = "v2";
                config.Path = "/openapi/v2.json";
            });

            if (env.IsDevelopment())
            {
                DbInitializer.Initialize(context).Wait();
                app.UseStatusCodePages();
                app.UseDeveloperExceptionPage();
                app.UseSwaggerUi3(config =>
                {
                    config.SwaggerRoutes.Add(new SwaggerUi3Route("v1", "/openapi/v1.json"));
                    config.SwaggerRoutes.Add(new SwaggerUi3Route("v2", "/openapi/v2.json"));
                    
                    config.Path = "/openapi";
                    config.DocumentPath = "/openapi/v2.json";
                });
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
