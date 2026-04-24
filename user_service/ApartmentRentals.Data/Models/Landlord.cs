namespace ApartmentRentals.Data.Models
{
    public class Landlord:User
    {
        public float Rating { get; set; }
        public List<string> OwnedSpaceIds { get; set; } = new();
    }
}
