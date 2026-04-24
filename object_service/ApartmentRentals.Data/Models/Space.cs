using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ApartmentRentals.Data.Models
{
    public enum SpaceType { Apartment, Room, BedPlace, House, PartOfHouse}
    public enum HouseType { Brick, Wooden, Monolithic, Panel, Block, Stalinka }
    public enum RentalTerm { LongTerm, SeveralMonths, ShortTerm }

    public class SpacePhoto
    {
        public string Url { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsMain { get; set; }
    }

    public class Area
    {
        public double Total { get; set; }
        public double Kitchen { get; set; }
        public double Living { get; set; }
    }

    public class Space : IModel
    {
        [BsonElement("category")]
        public SpaceType Category { get; set; }
        [BsonElement("title")]
        public string Title { get; set; } = string.Empty;
        [BsonElement("address")]
        public string Address { get; set; } = string.Empty;

        [BsonElement("roomCount")]
        public int RoomCount { get; set; }

        [BsonElement("price")]
        public decimal Price { get; set; }
        [BsonElement("term")]
        public RentalTerm Term { get; set; }

        public Area Area { get; set; } = new();
        public int Floor { get; set; }
        public int TotalFloors { get; set; }

        public int BuildYear { get; set; }
        public HouseType Material { get; set; }

        public bool AllowsChildren { get; set; }
        public bool AllowsPets { get; set; }

        public List<SpacePhoto> Photos { get; set; } = new();

        [BsonElement("LandlordId")]
        public string LandlordId { get; set; } = "0";
        public DateTime PublishedAt { get; set; } = DateTime.Now;
    }
}
