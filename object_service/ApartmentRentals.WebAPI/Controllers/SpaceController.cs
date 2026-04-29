using ApartmentRentals.Data.DTOs;
using ApartmentRentals.Data.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using ApartmentRentals.WebAPI.Services.Repositories;
using ApartmentRentals.kafka.KafkaProducer;
using System.Text.Json;

namespace ApartmentRentals.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpaceController : ControllerBase
    {

        private readonly IRepository<Space> _spaceService;
        private readonly IMapper _mapper;

        private readonly KafkaProducer _producer;

        public SpaceController(KafkaProducer producer, IRepository<Space> spaceService, IMapper mapper)
        {
            _producer = producer;
            _spaceService = spaceService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync([FromQuery] bool full = false)
        {
            var spaces = await _spaceService.GetAllAsync();

            if (full)
            {
                return Ok(spaces.ToList());
            }

            var shortRes = _mapper.Map<IEnumerable<SpaceListDTO>>(spaces);
            return Ok(shortRes.ToList());
        }

        [HttpGet("filter")]
        public async Task<IActionResult> GetFilteredByPropertyAsync([FromQuery] string propertyName, [FromQuery] string value)
        {
            try
            {
                var results = await _spaceService.GetFilteredByPropertyAsync(propertyName, value);
                return Ok(results);
            }
            catch (PropertyFilterException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Space>> GetAsync(string id)
        {
            var space = await _spaceService.GetByIdAsync(id);

            if (space == null) return NotFound();

            return Ok(space);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(SpaceNoIdDTO s)
        {
            var space = _mapper.Map<Space>(s);

            await _spaceService.CreateAsync(space);
           SendRegRequest(space);

            return CreatedAtAction(nameof(GetAsync), new { id = space.Id }, space);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(string id, SpaceNoIdDTO s) => 
            await _spaceService.UpdateAsync(id, _mapper.Map<Space>(s)) ? NoContent() : NotFound();


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id) => await _spaceService.DeleteByIdAsync(id) ? NoContent() : NotFound();

        [HttpDelete]
        public async Task<IActionResult> DeleteAll()
        {
            await _spaceService.DeleteAllAsync();
            return Ok();
        }

    private void SendRegRequest(Space space) {
        RegistrationObjectDTO reg = new RegistrationObjectDTO()
        {
            Type = ObjectType.Space,
            Id = space.Id,
            AuthorId = space.AuthorId
        };

        string message = JsonSerializer.Serialize(reg);

        _producer.SendAsync(message);
    }
}
}
