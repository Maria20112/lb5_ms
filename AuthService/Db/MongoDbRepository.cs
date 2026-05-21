using Microsoft.Extensions.Options;
using MongoDB.Driver;
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
    }
}