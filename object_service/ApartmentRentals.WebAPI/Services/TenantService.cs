using ApartmentRentals.Data.DTOs;
using ApartmentRentals.Data.Models;
using ApartmentRentals.WebAPI.Services.Repositories;

namespace ApartmentRentals.Services {
    public class TenantService : IService
    {
        private readonly IRepository<Tenant> _tenantService;

        public TenantService(IRepository<Tenant> tenantService)
        {
            _tenantService = tenantService;
        }

        public async Task UpdateRegistrationAsync(ReceivedObjectDTO receivedObject) {
            var objId = receivedObject.ObjectId;
            var foundObj = await _tenantService.GetByIdAsync(objId);
            if (foundObj != null) {
                foundObj.RegStatus = RegistrationStatus.REGISTRATED;
                foundObj.AuthorId = receivedObject.AuthorId;

                await _tenantService.UpdateAsync(objId, foundObj);
            }
        }
    }
}