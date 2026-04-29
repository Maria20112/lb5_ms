using ApartmentRentals.Data.DTOs;
using ApartmentRentals.Data.Models;
using ApartmentRentals.WebAPI.Services.Repositories;

namespace ApartmentRentals.Services {
    public class RentalContractService : IService
    {
        private readonly IRepository<RentalContract> _rentalContractService;

        public RentalContractService(IRepository<RentalContract> rentalContractService)
        {
            _rentalContractService = rentalContractService;
        }

        public async Task UpdateRegistrationAsync(ReceivedObjectDTO receivedObject) {
            var objId = receivedObject.ObjectId;
            var foundObj = await _rentalContractService.GetByIdAsync(objId);
            if (foundObj != null) {
                foundObj.RegStatus = RegistrationStatus.REGISTRATED;
                foundObj.AuthorId = receivedObject.AuthorId;

                await _rentalContractService.UpdateAsync(objId, foundObj);
            }
        }
    }
}