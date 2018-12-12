using System.Threading.Tasks;

namespace RoomTemp.Data.Repositories
{
    public interface ISensorRepository
    {
        Task<Sensor> Get(int id);
    }
}
