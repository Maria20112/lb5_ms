using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AuthService.Db
{
    public class UserCredentials
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("email")]
        public string Email { get; set; } = string.Empty;

        [BsonElement("pass")]
        public string Pass { get; set; } = string.Empty;

        [BsonElement("version")]
        public int Version { get; set; } = 0;
    }
}