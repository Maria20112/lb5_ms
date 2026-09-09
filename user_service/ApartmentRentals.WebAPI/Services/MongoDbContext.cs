namespace ApartmentRentals.WebAPI.Services
{
    public class MongoDbContext
    {
        public string ConnectionString { get; set; } = null!;
        public string DatabaseName { get; set; } = null!;
        public string UserCollectionName { get; set;} = null!;

    }
}
