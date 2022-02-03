using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Test.MockedServices;
using WebApplication;
using WebApplication.DB;
using WebApplication.MessageSender.Models;
using WebApplication.Settings.Models;

namespace Test
{
    public class TestHost : IDisposable
    {
        private IHost _host;

        internal HttpClient Client { get; private set; }

        public AppSettings AppSettings { get; private set; }

        private TestHost()
        {
        }

        public static async Task<TestHost> InitTestHost()
        {
            var host = new TestHost();

            host.InitializeApp();
            await host.InitDb();

            return host;
        }

        public IHostBuilder CreateHostBuilder()
        {
            const string configDir = "config";

            var config = new ConfigurationBuilder()
                .AddJsonFile($"{configDir}/appsettings.json")
                .AddEnvironmentVariables()
                .Build();

            AppSettings = new AppSettings();

            config.Bind(nameof(AppSettings), AppSettings);

            return Host.CreateDefaultBuilder()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                        .UseUrls(config["Urls"])
                        .UseConfiguration(config)
                        .UseTestServer()
                        .ConfigureTestServices(services =>
                        {
                            services.AddScoped<ISenderApi, SmsSenderApiMock>();
                            services.AddOptions();
                            services.AddEntityFrameworkInMemoryDatabase();
                        })
                        .UseStartup<Startup>()
                        .ConfigureLogging(builder => builder.ClearProviders());
                });
        }

        private void InitializeApp()
        {
            _host = CreateHostBuilder().Build();

            _host.Start();

            Client = _host.GetTestClient();
        }

        public async Task<SqliteDbRepository> GetDbContext()
        {
            var context = new SqliteDbRepository(CreateNewContextOptions());
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();
            return context;
        }

        private async Task InitDb()
        {
            await using var context = await GetDbContext();

            await context.CreateTables();
        }

        private static DbContextOptions<SqliteDbRepository> CreateNewContextOptions()
        {
            using var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();
            var options = new DbContextOptionsBuilder<SqliteDbRepository>()
                .UseSqlite(connection)
                .Options;
            connection.Close();
            return options;
        }

        public void Dispose()
        {
            _host?.Dispose();
            Client?.Dispose();
        }
    }
}