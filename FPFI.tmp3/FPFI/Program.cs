using System;
using System.Collections.Generic;
using FPFI.Data;
using FPFI.Services;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FPFI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = BuildWebHost(args);
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<ApplicationDbContext>();
                    UserInitializer.Initialize(context);
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "There has been an error while seeding the database.");
                }
            }
            host.Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();

        public class CustomConfigProvider : ConfigurationProvider
        {
            public CustomConfigProvider() { }

            public override void Load()
            {
                Data = new Dictionary<string,string>{ { "", Encryption.DecryptString("bla", "bla") } };
            }
        }

        public class CustomConfigurationSource : IConfigurationSource
        {
            public CustomConfigurationSource() { }


            public IConfigurationProvider Build(IConfigurationBuilder builder)
            {
                return new CustomConfigProvider();
            }
        }

        //public static class CustomConfigurationExtensions
        //{
        //    public static IConfigurationBuilder AddCustomConfiguration(this IConfigurationBuilder builder)
        //    {
        //        return builder.Add(new CustomConfigurationSource());
        //    }
        //}
    }
}
