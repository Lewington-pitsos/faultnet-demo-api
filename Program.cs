using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace faultnet_demo_api {
    public class Program{
        public static void Main(string[] args) {
            CreateHostBuilder(args).Build().Run();
        }
        public static IHostBuilder CreateHostBuilder(string[] args) => 
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) => {
                    HostConfig.CertPath = context.Configuration["CertPath"];
                    HostConfig.CertPassword = context.Configuration["CertPassword"];
                })
                .ConfigureWebHostDefaults(webBuilder => 
                {   
                    webBuilder.ConfigureKestrel((opt => {
                        opt.ListenAnyIP(5000);
                        opt.ListenAnyIP(5001, listOpt => {
                            listOpt.UseHttps(HostConfig.CertPath, HostConfig.CertPassword);
                        });
                    });
                    webBuilder.UseStartup<Startup>();
                });
    }

    public static class HostConfig
    {
        public static string CertPath {get; set;}
        public static string CertPassword {get; set;}
    }
}
