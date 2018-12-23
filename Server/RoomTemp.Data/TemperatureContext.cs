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
        public DbSet<TempReading> TempReading { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TempReading>().HasOne(t => t.Sensor).WithMany(s => s.TempReadings)
                .HasForeignKey(t => t.SensorId);
            modelBuilder.Entity<TempReading>().HasOne(t => t.Device).WithMany(d => d.TempReadings)
                .HasForeignKey(t => t.DeviceId);
        }
    }
}