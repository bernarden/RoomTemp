using System;
using Microsoft.EntityFrameworkCore;

namespace RoomTemp.Data
{
    public class TemperatureContext : DbContext
    {
        public TemperatureContext(DbContextOptions<TemperatureContext> options) : base(options)
        {
        }

        public DbSet<Device> Device { get; set; }
        public DbSet<Sensor> Sensor { get; set; }
        public DbSet<Location> Location { get; set; }

        public DbSet<TempReading> TempReading { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TempReading>(e =>
            {
                e.HasOne(t => t.Sensor)
                    .WithMany(s => s.TempReadings)
                    .HasForeignKey(t => t.SensorId);

                e.HasOne(t => t.Device)
                    .WithMany(d => d.TempReadings)
                    .HasForeignKey(t => t.DeviceId);

                e.HasIndex(t => t.TakenAt);
                e.Property(t => t.TakenAt)
                    .HasConversion(v => v, v => DateTime.SpecifyKind(v, DateTimeKind.Utc));
            });
        }
    }
}