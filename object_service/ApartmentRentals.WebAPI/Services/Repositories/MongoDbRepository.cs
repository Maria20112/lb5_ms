using ApartmentRentals.Data.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Reflection;

namespace ApartmentRentals.WebAPI.Services.Repositories
{
    public class MongoDbRepository<T> : IRepository<T> where T : IModel
    {
        private readonly IMongoCollection<T> collection;
        const long SAMPLE_NUM = 1000;

        public MongoDbRepository(IMongoClient client, IOptions<MongoDbContext> settings)
        {
            var database = client.GetDatabase(settings.Value.DatabaseName);
            collection = database.GetCollection<T>(typeof(T).Name);
        }

        public async Task<IEnumerable<T>> GetAllAsync() =>
            await collection.Aggregate().Sample(SAMPLE_NUM).ToListAsync();

        public async Task<IEnumerable<T>> GetFilteredByPropertyAsync(string propertyName, string value)
        {
            var property = typeof(T).GetProperty(propertyName,
                BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

            if (property == null)
                throw new PropertyFilterException($"Поле '{propertyName}' не найдено");

            try
            {
                object convertedValue;
                Type targetType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;

                if (targetType.IsEnum)
                {
                    convertedValue = Enum.Parse(targetType, value, true);
                }
                else
                {
                    convertedValue = Convert.ChangeType(value, targetType);
                }

                return await collection.Find(Builders<T>.Filter.Eq(property.Name, convertedValue)).ToListAsync();
            }
            catch (Exception)
            {
                throw new PropertyFilterException($"Неверный тип значения '{value}' для поля '{propertyName}'");
            }
        }

        public async Task<T?> GetByIdAsync(string id) =>
            await collection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(T newObject) =>
            await collection.InsertOneAsync(newObject);

        public async Task<bool> UpdateAsync(string id, T updatedObject)
        {
            updatedObject.Id = id;
            var result = await collection.ReplaceOneAsync(x => x.Id == id, updatedObject);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> DeleteByIdAsync(string id)
        {
            var result = await collection.DeleteOneAsync(x => x.Id == id);
            return result.DeletedCount > 0;
        }

        public async Task<bool> DeleteAllAsync()
        {
            var result = await collection.DeleteManyAsync(_ => true);
            return result.DeletedCount > 0;
        }
    }
}
