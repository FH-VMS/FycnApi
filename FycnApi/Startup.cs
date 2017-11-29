using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Cors.Internal;
using Newtonsoft.Json.Serialization;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using Fycn.Utility;
using Microsoft.Extensions.Logging;

namespace FycnApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            /*
            var builder = new ConfigurationBuilder()
       .SetBasePath(env.ContentRootPath)
       .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
       */
            
             Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            //services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            //services.Configure<WebConfig>(Configuration.GetSection("AppConfiguration"));
            services.AddCors();
            services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin",
                    builder => builder
                    //.WithOrigins("http://localhost:9090")
                    .AllowAnyOrigin()
                    .WithMethods("GET", "POST", "PUT", "DELETE")
                    .AllowAnyHeader()
                    .AllowCredentials()
                    .SetPreflightMaxAge(TimeSpan.FromSeconds(2520))
                    );
            });
            services.Configure<MvcOptions>(options =>
            {
                options.Filters.Add(new CorsAuthorizationFilterFactory("AllowSpecificOrigin"));
            });

            services.AddMvc()
                .AddJsonOptions(options => { options.SerializerSettings.ContractResolver = new DefaultContractResolver(); })
                .AddWebApiConventions();
            
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseDeveloperExceptionPage();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            app.UseMvc(routes =>
            {
                routes.MapRoute("default", "api/{controller=Machine}/{action=GetPayResult}/{id?}");
            });

            
            app.UseStaticFiles();
            app.UseCors("AllowSpecificOrigin");
            app.UseStaticHttpContext();

            //app.UseMvcWithDefaultRoute();
        }

        private void OnStarted()
        {
            // Perform post-startup activities here
        }

        private void OnStopping()
        {
            // Perform on-stopping activities here
        }

        private void OnStopped()
        {
            // Perform post-stopped activities here
        }
    }
}
