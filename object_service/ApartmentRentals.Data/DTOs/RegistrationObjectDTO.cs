namespace ApartmentRentals.Data.DTOs
{
    public class RegistrationObjectDTO{
        public ObjectType Type { get; set; }
        public string Id { get; set; } = "";
        public string AuthorId { get; set; } = "";
    }
}