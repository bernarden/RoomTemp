namespace RoomTemp.Data
{
    public interface IEntity<TKey>
    {
        TKey Id { get; set; }
    }
}