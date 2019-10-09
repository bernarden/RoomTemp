using System;
using System.Linq;
using GraphQL;
using GraphQL.Types;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RoomTemp.Data;
using RoomTemp.Data.Repositories;
using RoomTemp.Domain;
using RoomTemp.Models.GraphQL.Sensor;

namespace RoomTemp
{
    public class Startup
    {
        public const string LocalTestsEnvironment = "LocalTests";

        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            Env = env;
        }

        public static IConfiguration Configuration { get; private set; }
        private IHostingEnvironment Env { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMemoryCache();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            ConfigureDbContext(services);
            
            services.AddTransient<SensorQuery>();
            services.AddTransient<SensorMutation>();
            services.AddTransient<IDocumentExecuter, DocumentExecuter>();
            services.AddTransient<ISchema>(sp => new Schema
                { Query = sp.GetService<SensorQuery>(), Mutation = sp.GetService<SensorMutation>() });
            services.AddTransient<SensorRepository, SensorRepository>();
            services.AddTransient<ICachingService, CachingService>();
            services.AddTransient<DeviceRepository, DeviceRepository>();

            // In production, the React files will be served from this directory
            services.AddSpaStaticFiles(configuration => { configuration.RootPath = "ClientApp/build"; });
        }

        private void ConfigureDbContext(IServiceCollection services)
        {
            if (Env.IsEnvironment(LocalTestsEnvironment))
            {
                services.AddDbContext<TemperatureContext>(options =>
                    options.UseInMemoryDatabase(databaseName: LocalTestsEnvironment));
                return;
            }

            var connectionStrings = Configuration.GetSection("ConnectionStrings");
            var database = connectionStrings.GetValue<string>("Database");
            var connectionString = connectionStrings.GetValue<string>(database);
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                const string message = "Database connection string is missing. Terminating execution";
                Console.WriteLine(message);
                throw new Exception(message);
            }

            switch (database)
            {
                case "SqlServer":
                    services.AddDbContext<TemperatureContext>(o => o.UseSqlite(connectionString));
                    break;
                case "Sqlite":
                    services.AddDbContext<TemperatureContext>(o => o.UseSqlServer(connectionString));
                    break;
                default:
                    var message = $"Could not configure Db Context. Database '{database}' is not supported.";
                    Console.WriteLine(message);
                    throw new Exception(message);
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            if (Env.IsDevelopment() || Env.IsEnvironment(LocalTestsEnvironment))
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            UpdateDatabase(app);
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    "default",
                    "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

                if (Env.IsDevelopment()) spa.UseReactDevelopmentServer("start");
            });
        }

        private void UpdateDatabase(IApplicationBuilder app)
        {
            if (Env.IsEnvironment(LocalTestsEnvironment)) return;
            try
            {
                using (var serviceScope = app.ApplicationServices
                    .GetRequiredService<IServiceScopeFactory>()
                    .CreateScope())
                {
                    using (var context = serviceScope.ServiceProvider.GetService<TemperatureContext>())
                    {
                        context.Database.Migrate();

                        if (context.Device.Any()) return;
                        context.Device.Add(new Device { Name = "Raspberry Pi 3", Key = Guid.NewGuid() });
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
           
        }
    }
}