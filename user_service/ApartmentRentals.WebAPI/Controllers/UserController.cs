using ApartmentRentals.Data.DTOs;
using ApartmentRentals.Data.Models;
using ApartmentRentals.WebAPI.Services.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly UserRepository<UserDTO> _landlordService;
    private readonly IMapper _mapper;

    public UserController(UserRepository<UserDTO> landlordService, IMapper mapper)
    {
        _landlordService = landlordService;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<List<UserDTO>>> GetAllAsync() => (await _landlordService.GetAllAsync()).ToList();

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
    public async Task<ActionResult<UserDTO>> GetAsync(string id)
    {
        var landlord = await _landlordService.GetByIdAsync(id);

        if (landlord == null) return NotFound();

        return Ok(landlord);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync(UserDTO dto)
    {
        var landlord = _mapper.Map<UserDTO>(dto);

        await _landlordService.CreateAsync(landlord);

        return CreatedAtAction(nameof(GetAsync), new { id = landlord.Id }, landlord);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync(string id, UserDTO dto) => 
        await _landlordService.UpdateAsync(id, _mapper.Map<UserDTO>(dto)) ? NoContent() : NotFound();

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id) => await _landlordService.DeleteByIdAsync(id) ? NoContent() : NotFound();

    [HttpDelete]
    public async Task<IActionResult> DeleteAll()
    {
        await _landlordService.DeleteAllAsync();
        return Ok();
    }
}
