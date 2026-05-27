using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
namespace AuthService.Db
{
    public class MongoDbRepository : IRepository<UserCredentials>
    {
        private readonly IMongoCollection<UserCredentials> collection;

        public MongoDbRepository(IMongoClient client, IOptions<MongoDbContext> settings)
        {
            var database = client.GetDatabase(settings.Value.DatabaseName);
            collection = database.GetCollection<UserCredentials>(settings.Value.AuthCollectionName);
        }

        public async Task<UserCredentials?> GetByIdAsync(string id) =>
            await collection.Find(x => x.Id == id).FirstOrDefaultAsync();
        public async Task<UserCredentials?> GetByEmailAsync(string email) =>
            await collection.Find(x => x.Email == email).FirstOrDefaultAsync();

        public async Task CreateAsync(UserCredentials newObject) =>
            await collection.InsertOneAsync(newObject);

        public async Task<bool> ExistsAsync(string email)
        {
            var count = await collection.CountDocumentsAsync(u => u.Email == email);
            return count > 0;
        }

        public async Task<bool> ChangePassAsync(string email, string pass)
        {
            var filter = Builders<UserCredentials>.Filter.Eq(x => x.Email, email);
            var update = Builders<UserCredentials>.Update
                .Set(x => x.Pass, pass)
                .Inc(x => x.Version, 1);

            var result = await collection.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> DeleteByIdAsync(string id)
        {
            var result = await collection.DeleteOneAsync(x => x.Id == id);
            return result.DeletedCount > 0;
        }
    }
}