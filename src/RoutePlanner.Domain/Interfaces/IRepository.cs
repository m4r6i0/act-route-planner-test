namespace RoutePlanner.Domain.Interfaces
{
    public interface IRepository<T>
    {
        Task AddAsync(T entity);
        Task<List<T>> GetAllAsync();
    }
}
