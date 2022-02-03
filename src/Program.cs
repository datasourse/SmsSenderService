using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebApplication.DB.Models;
using WebApplication.Extensions;

namespace WebApplication
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            var scopeFactory = host.Services.GetRequiredService<IServiceScopeFactory>();
            var db = scopeFactory.GetScopedService<IDbRepository>();
            await InitDb(db);
            
            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            const string configDir = "config";

            var configBuilder = new ConfigurationBuilder()
                .AddJsonFile($"{configDir}/appsettings.json");

            var config = configBuilder
                .AddJsonFile($"{configDir}/appsettings.{environmentName}.json")
                .AddEnvironmentVariables()
                .Build();

            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                        .UseUrls(config["Urls"])
                        .UseConfiguration(config)
                        .UseStartup<Startup>();
                });
        }

        private static async Task InitDb(IDbRepository db)
        {
            await db.CreateTables();
        }
    }
}