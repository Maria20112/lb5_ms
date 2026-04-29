using ApartmentRentals.Data.DTOs;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using kafka.KafkaProducer;
using System.Runtime.InteropServices.ObjectiveC;

namespace ApartmentRentals.UsersService
{
    public class UserService
    {
        private readonly UserRepository<UserDTO> _userRepository;
        private readonly IMapper _mapper;
        private readonly KafkaProducer _producer;
        private readonly KafkaProducerSettings _producerSettings;
        public UserService(UserRepository<UserDTO> userRepository, KafkaProducer producer, IOptions<KafkaProducerSettings> producerSettings,
            IMapper mapper) {
            _userRepository = userRepository;
            _mapper = mapper;
            _producer = producer;
            _producerSettings = producerSettings.Value;
        }

        public async Task registerObject(RegistrationObjectDTO reg) {
            var userId = reg.AuthorId;
            var user = await _userRepository.GetByIdAsync(userId);

            if (user != null) {
                user.registered_objects++;
                await _userRepository.UpdateAsync(userId, user);

                await _producer.SendAsync(JsonSerializer.Serialize(new ResponseDTO
                {
                    UserId = userId,
                    Type = reg.Type,
                    ObjectId = reg.Id
                }));
            }
        }
    }
}