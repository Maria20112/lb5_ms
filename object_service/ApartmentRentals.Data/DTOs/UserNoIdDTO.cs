using ApartmentRentals.Data.Models;

namespace ApartmentRentals.Data.DTOs
{
    public class UserNoIdDTO
    {
        public string Surname { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;

        public string? MiddleName { get; set; } = null;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string AuthorId {get; set;} = string.Empty;
        public RegistrationStatus RegStatus {get; init;}
    }

    public enum ObjectType
    {
        Landlord,
        Tenant,
        Space,
        RentalContract
    }
}
