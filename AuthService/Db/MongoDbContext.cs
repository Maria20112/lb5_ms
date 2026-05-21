namespace AuthService.Db
{
    public class MongoDbContext
    {
        public string ConnectionString { get; set; } = null!;
        public string DatabaseName { get; set; } = null!;
        public string AuthCollectionName { get; set; } = null!;

    }
}
