using ApartmentRentals. Data.DTOs;
using ApartmentRentals.Data.Models;
using AutoMapper;

namespace ApartmentRentals.Models
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<UserNoIdDTO, User>();
        }
    }
}
