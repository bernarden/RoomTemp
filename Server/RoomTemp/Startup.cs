using System;
using System.Linq;
using GraphQL;
using GraphQL.Types;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.EntityFrameworkCore;
using RoomTemp.Data;
using RoomTemp.Data.Repositories;
using RoomTemp.Domain;
using RoomTemp.Models.GraphQL.Sensor;

namespace RoomTemp
{
    public class Startup
    {
        public const string LocalTestsEnvironment = "LocalTests";

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            Env = env;
        }

        public static IConfiguration Configuration { get; private set; }
        private IWebHostEnvironment Env { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMemoryCache();

            services.AddMvc(option => option.EnableEndpointRouting = false);
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

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
            services.AddApplicationInsightsTelemetry();
        }

        private void ConfigureDbContext(IServiceCollection services)
        {
            if (Env.IsEnvironment(LocalTestsEnvironment))
            {
                services.AddDbContext<TemperatureContext>(options =>
                    options.UseInMemoryDatabase(databaseName: LocalTestsEnvironment));
                return;
            }

            var database = Configuration.GetConnectionString("Database");
            var connectionString = Configuration.GetConnectionString(database);
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                const string message = "Database connection string is missing. Terminating execution";
                Console.WriteLine(message);
                throw new Exception(message);
            }

            switch (database)
            {
                case "SqlServer":
                    services.AddDbContext<TemperatureContext>(o => o.UseSqlServer(connectionString));
                    break;
                case "Sqlite":
                    services.AddDbContext<TemperatureContext>(o => o.UseSqlite(connectionString));
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
                using var serviceScope = app.ApplicationServices
                    .GetRequiredService<IServiceScopeFactory>()
                    .CreateScope();
                using var context = serviceScope.ServiceProvider.GetService<TemperatureContext>();

#if DEBUG
                if (!context.Database.CanConnect())
                {
                    // Generate database with fake data for development. ~ 65MB db.
                    context.Database.Migrate();
                    context.Device.Add(new Device { Name = "Raspberry Pi", Key = Guid.NewGuid(), Id = 1 });
                    context.Location.Add(new Location { Name = "Home", Id = 3 });
                    context.Sensor.Add(new Sensor { Name = "TSYS01", Id = 1 });

                    int tempShift = 15;
                    var numberOfTempReadingsPerDay = 24 * 60 * 60 / 15;
                    for (int numberDaysBack = -40; numberDaysBack < 41; numberDaysBack++)
                    {
                        double tempVariancePerDay = new Random().NextDouble() * 20;
                        var takenAt = DateTime.UtcNow.Date.AddDays(numberDaysBack);
                        for (int i = 0; i < numberOfTempReadingsPerDay; i++)
                        {
                            var temperature =
                                tempVariancePerDay *
                                Math.Sin(2 * Math.PI * i / numberOfTempReadingsPerDay) +
                                tempShift;
                            var tempReading = new TempReading
                            {
                                DeviceId = 1,
                                LocationId = 3,
                                SensorId = 1,
                                TakenAt = takenAt,
                                Temperature = (decimal) Math.Round(temperature, 2)
                            };
                            context.TempReading.Add(tempReading);
                            takenAt = takenAt.AddSeconds(15);
                        }
                    }

                    context.SaveChanges();
                    return;
                }
#endif

                context.Database.Migrate();

                if (context.Device.Any()) return;
                context.Device.Add(new Device { Name = "Raspberry Pi 3", Key = Guid.NewGuid() });
                context.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}