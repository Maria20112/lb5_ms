namespace ApartmentRentals.Data.DTOs
{
    public class TenantNoIdDTO : UserNoIdDTO
    {
        public bool HasPets { get; set; }
        public bool HasChildren { get; set; }
    }
}
