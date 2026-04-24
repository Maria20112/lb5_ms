namespace ApartmentRentals.Data.DTOs
{
    public class LandlordNoIdDTO : UserNoIdDTO
    {
        public float Rating { get; set; }
        public List<int> OwnedSpaceIds { get; set; } = new();
    }
}
