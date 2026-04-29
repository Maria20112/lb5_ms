namespace ApartmentRentals.Data.DTOs
{
    public class ReceivedObjectDTO{
        public ObjectType Type { get; set; }
        public string ObjectId { get; set; } = "";
        public string AuthorId { get; set; } = "";
    }
}