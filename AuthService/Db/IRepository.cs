namespace AuthService.Db
{
    public interface IRepository<T>
    {
        Task<T?> GetByIdAsync(string id);
        Task<T?> GetByEmailAsync(string email);
        Task CreateAsync(T entity);
        Task<bool> ExistsAsync(string email);
    }
}
