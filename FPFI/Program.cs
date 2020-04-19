using System;
using System.IO;
using System.Net;
using FPFI.Data;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;

namespace FPFI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateWebHostBuilder(args).Build();
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                //try
                {
                    var context = services.GetRequiredService<ApplicationDbContext>();
                    UserInitializer.Initialize(context);
                    DataInitializer.Initialize(context);
                }
                //catch (Exception ex)
                //{
                //    var logger = services.GetRequiredService<ILogger<Program>>();
                //    logger.LogError(ex, "There has been an error while seeding the database.");
                //}
            }
            host.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseKestrel(options =>
                {
                    string os = Environment.OSVersion.Platform.ToString();

                    options.Limits.MaxConcurrentConnections = 100;
                    options.Limits.MaxConcurrentUpgradedConnections = 100;
                    //options.Limits.MaxRequestBodySize = 20_000_000;
                    options.Limits.MinRequestBodyDataRate =
                        new MinDataRate(bytesPerSecond: 100, gracePeriod: TimeSpan.FromSeconds(10));
                    options.Limits.MinResponseDataRate =
                        new MinDataRate(bytesPerSecond: 100, gracePeriod: TimeSpan.FromSeconds(10));
                    options.Listen(IPAddress.Parse("127.0.0.8"), 5080);
                    options.Listen(IPAddress.Parse("127.0.0.8"), 5081, listenOptions =>
                    {
                        listenOptions.UseHttps(os == "Win32NT" ?
                            Path.Combine(
                                Directory.GetCurrentDirectory(),
                                "sslforfree/fpfi.pfx") : "/etc/ssl/certs/fpfi.pfx", "89ioIOkl");
                    });
                })
                .UseStartup<Startup>();
    }
}
