using ApartmentRentals.Data.DTOs;
using ApartmentRentals.Data.Models;
using ApartmentRentals.WebAPI.Services.Repositories;

namespace ApartmentRentals.Services {
    public class SpaceService : IService
    {
        private readonly IRepository<Space> _spaceService;

        public SpaceService(IRepository<Space> spaceService)
        {
            _spaceService = spaceService;
        }

        public async Task UpdateRegistrationAsync(ReceivedObjectDTO receivedObject) {
            var objId = receivedObject.ObjectId;
            var foundObj = await _spaceService.GetByIdAsync(objId);
            if (foundObj != null) {
                foundObj.RegStatus = RegistrationStatus.REGISTRATED;
                foundObj.AuthorId = receivedObject.AuthorId;

                await _spaceService.UpdateAsync(objId, foundObj);
            }
        }
    }
}