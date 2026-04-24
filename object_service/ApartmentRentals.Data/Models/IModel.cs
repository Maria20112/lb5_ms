using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ApartmentRentals.Data.Models
{
    public abstract class IModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
    }
}
