using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.StaticFiles;
using System.Text;

namespace FycnLog
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDirectoryBrowser();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
           
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            var dir = new DirectoryBrowserOptions();
            dir.FileProvider = new PhysicalFileProvider("/log/");
            app.UseDirectoryBrowser(dir);
            var staticfile = new StaticFileOptions();
            staticfile.FileProvider = new PhysicalFileProvider("/log/");//指定目录 这里指定C盘,也可以是其它目录
            staticfile.ServeUnknownFileTypes = true;
            staticfile.DefaultContentType = "application/x-msdownload"; //设置默认  MIME
            
            var provider = new FileExtensionContentTypeProvider();
            provider.Mappings.Add(".log", "text/plain");//手动设置对应MIME
            staticfile.ContentTypeProvider = provider;
            app.UseStaticFiles(staticfile);
        }
    }
}
