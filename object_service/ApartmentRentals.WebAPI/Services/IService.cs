using ApartmentRentals.Data.DTOs;

namespace ApartmentRentals.Services {
    public interface IService {
        public Task UpdateRegistrationAsync(ReceivedObjectDTO receivedObject);
    }
}