using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Server.Kestrel.Transport.Abstractions.Internal;
using System.Security.Cryptography.X509Certificates;
using System.Net;

namespace FycnApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                     .UseKestrel(options => options.Listen(IPAddress.Any, 5000, listenOptions =>
                     {
                         //listenOptions.UseHttps(new X509Certificate2("你的.pfx", "pfx文件的密码"));
                         //options.Limits.MaxConcurrentConnections = 100;
                         //options.Limits.MaxConcurrentUpgradedConnections = 100;
                         //options.Limits.MaxRequestBodySize = 10 * 1024;
                         options.ApplicationSchedulingMode = SchedulingMode.ThreadPool;
                         options.AllowSynchronousIO = true;
                         options.Limits.KeepAliveTimeout = new TimeSpan(0, 0, 15);
                     }))
                    .UseContentRoot(Directory.GetCurrentDirectory())
                    .UseUrls("http://localhost:5000")
                    .UseIISIntegration()
                    .UseStartup<Startup>()
                    .UseApplicationInsights()
                    .Build();
    }
}
