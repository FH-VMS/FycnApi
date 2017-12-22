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
                     .UseKestrel(options =>
                     {
                         options.ApplicationSchedulingMode = SchedulingMode.ThreadPool;
                         options.AllowSynchronousIO = true;
                         options.Limits.KeepAliveTimeout = new TimeSpan(0, 0, 5);
                     })
                    .UseContentRoot(Directory.GetCurrentDirectory())
                    .UseUrls("http://localhost:5000")
                    .UseIISIntegration()
                    .UseStartup<Startup>()
                    .UseApplicationInsights()
                    .Build();
    }
}
