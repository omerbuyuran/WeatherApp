namespace WeatherApp.Interfaces
{
    public interface IUnitOfWork
    {
        Task CompleteAsync();
    }
}
