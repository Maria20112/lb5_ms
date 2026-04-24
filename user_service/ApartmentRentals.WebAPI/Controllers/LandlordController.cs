using ApartmentRentals.Data.DTOs;
using ApartmentRentals.Data.Models;
using ApartmentRentals.WebAPI.Services.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class LandlordController : ControllerBase
{
    private readonly IRepository<Landlord> _landlordService;
    private readonly IMapper _mapper;

    public LandlordController(IRepository<Landlord> landlordService, IMapper mapper)
    {
        _landlordService = landlordService;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<List<Landlord>>> GetAllAsync() => (await _landlordService.GetAllAsync()).ToList();

    [HttpGet("filter")]
    public async Task<IActionResult> GetFilteredByPropertyAsync([FromQuery] string propertyName, [FromQuery] string value)
    {
        try
        {
            return Ok(await _landlordService.GetFilteredByPropertyAsync(propertyName, value));
        }
        catch (PropertyFilterException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Landlord>> GetAsync(string id)
    {
        var landlord = await _landlordService.GetByIdAsync(id);

        if (landlord == null) return NotFound();

        return Ok(landlord);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync(LandlordNoIdDTO dto)
    {
        var landlord = _mapper.Map<Landlord>(dto);

        await _landlordService.CreateAsync(landlord);

        return CreatedAtAction(nameof(GetAsync), new { id = landlord.Id }, landlord);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync(string id, LandlordNoIdDTO dto) => 
        await _landlordService.UpdateAsync(id, _mapper.Map<Landlord>(dto)) ? NoContent() : NotFound();

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id) => await _landlordService.DeleteByIdAsync(id) ? NoContent() : NotFound();

    [HttpDelete]
    public async Task<IActionResult> DeleteAll()
    {
        await _landlordService.DeleteAllAsync();
        return Ok();
    }
}
