namespace ApartmentRentals.WebAPI.Services.Repositories
{
    public interface IRepository<T>
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> GetFilteredByPropertyAsync(string propertyName, string value);
        Task<T?> GetByIdAsync(string id);
        Task<bool> DeleteByIdAsync(string id);
        Task CreateAsync(T entity);
        Task<bool> UpdateAsync(string id, T entity);
        Task<bool> DeleteAllAsync();
    }
}
