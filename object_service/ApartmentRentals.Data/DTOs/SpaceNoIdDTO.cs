using ApartmentRentals.Data.Models;

namespace ApartmentRentals.Data.DTOs
{
    public class SpaceNoIdDTO
    {
        // Основная инфо
        public SpaceType Category { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;

        // Комнатность и цена
        public int RoomCount { get; set; }
        public decimal Price { get; set; }
        public RentalTerm Term { get; set; }

        // Площадь и этаж
        public Area Area { get; set; } = new();
        public int Floor { get; set; }
        public int TotalFloors { get; set; }

        // Характеристики дома
        public int BuildYear { get; set; }
        public HouseType Material { get; set; }

        // Условия проживания
        public bool AllowsChildren { get; set; }
        public bool AllowsPets { get; set; }

        // фото
        public List<SpacePhoto> Photos { get; set; } = new();

        // Связи
        public int LandlordId { get; set; } = 0;
    }
}
