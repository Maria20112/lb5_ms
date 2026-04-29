using ApartmentRentals.Data.DTOs;
using ApartmentRentals.Data.Models;
using ApartmentRentals.WebAPI.Services.Repositories;

namespace ApartmentRentals.Services {
    public class LandlordService : IService
    {
        private readonly IRepository<Landlord> _landlordService;

        public LandlordService(IRepository<Landlord> landlordService)
        {
            _landlordService = landlordService;
        }

        public async Task UpdateRegistrationAsync(ReceivedObjectDTO receivedObject) {
            var objId = receivedObject.ObjectId;
            var foundObj = await _landlordService.GetByIdAsync(objId);
            if (foundObj != null) {
                foundObj.RegStatus = RegistrationStatus.REGISTRATED;
                foundObj.AuthorId = receivedObject.AuthorId;

                await _landlordService.UpdateAsync(objId, foundObj);
            }
        }
    }
}