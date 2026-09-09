using ApartmentRentals.WebAPI.Services.Repositories;
using ApartmentRentals.Data.DTOs;
using MongoDB.Driver;

public class UserRepository<T> : IRepository<T> where T : UserDTO
{
    private readonly IMongoCollection<T> _users;

    public UserRepository(IMongoDatabase db) {
        _users = db.GetCollection<T>("users");
    }
    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _users.Find(_ => true).ToListAsync();
    }

    public async Task<T?> GetByIdAsync(string id)
    {
        var filter = Builders<T>.Filter.Eq(u => u.Id, id);
        return await _users.Find(filter).FirstOrDefaultAsync();
    }

        public async Task CreateAsync(T entity)
        {
            await _users.InsertOneAsync(entity);
        }

        public async Task<bool> UpdateAsync(string id, T entity)
        {
            var filter = Builders<T>.Filter.Eq(u => u.Id, id);
            var update = Builders<T>.Update
                .Set(u => u.Name, entity.Name)
                .Set(u => u.registered_objects, entity.registered_objects);

            var result = await _users.UpdateOneAsync(filter, update);
            return true;
        }

        public async Task<bool> DeleteByIdAsync(string id)
        {
            var filter = Builders<T>.Filter.Eq(u => u.Id, id);
            var result = await _users.DeleteOneAsync(filter);

            return result.DeletedCount > 0;
        }

    public async Task<bool> DeleteAllAsync() => false;

    public async Task<IEnumerable<T>> GetFilteredByPropertyAsync(string propertyName, string value)
    {
        return await _users.Find(_ => true).ToListAsync();
    }
    }
    