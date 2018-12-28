namespace RoomTemp.Data.Repositories
{
    public class LocationRepository : BaseRepository<Sensor, int>
    {
        public LocationRepository(TemperatureContext temperatureContext) : base(temperatureContext)
        {
        }
    }
}