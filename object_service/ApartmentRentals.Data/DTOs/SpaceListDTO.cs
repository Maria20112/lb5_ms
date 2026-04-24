using ApartmentRentals.Data.Models;

namespace ApartmentRentals.Data.DTOs
{
    public class SpaceListDTO
    {
        public string Id { get; set; } = "";
        public string Title { get; set; } = string.Empty;
        public SpaceType Category { get; set; }
        public decimal Price { get; set; }
        public string Address { get; set; } = string.Empty;
        public string MainPhotoUrl { get; set; } = string.Empty; // Остается только главное фото для списка
    }
}
