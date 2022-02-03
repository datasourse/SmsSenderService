using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using WebApplication.Controllers.Helpers;
using WebApplication.DB;
using WebApplication.DB.Models;
using WebApplication.MessageSender;
using WebApplication.MessageSender.Models;
using WebApplication.Settings.Models;
using WebApplication.Validator;
using WebApplication.Validator.Models;
using WebApplication.Validator.Transliterator;

namespace WebApplication
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers()
                .AddMvcOptions(options => { options.Filters.Add(typeof(CustomExceptionFilter)); });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "SmsSenderService", Version = "v1"});
            });

            services.AddOptions();
            services.Configure<SmsSettings>(Configuration.GetSection(nameof(SmsSettings)));
            services.Configure<AppSettings>(Configuration.GetSection(nameof(AppSettings)));

            services.AddDbContext<IDbRepository, SqliteDbRepository>();

            services.AddScoped<IValidator, SmsValidator>();
            services.AddHttpClient<ISenderApi, SmsSenderApi>();
            services.AddScoped<IMessageSender, SmsSenderService>();
            services.AddScoped<TransliterationService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "SmsSenderService v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}