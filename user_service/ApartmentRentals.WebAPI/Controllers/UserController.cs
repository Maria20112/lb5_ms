using ApartmentRentals.Data.DTOs;
using ApartmentRentals.Data.Models;
using ApartmentRentals.WebAPI.Services.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly UserRepository<UserDTO> _userService;
    private readonly IMapper _mapper;

    public UserController(UserRepository<UserDTO> landlordService, IMapper mapper)
    {
        _userService = landlordService;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<List<UserDTO>>> GetAllAsync() => (await _userService.GetAllAsync()).ToList();

    [HttpGet("filter")]
    public async Task<IActionResult> GetFilteredByPropertyAsync([FromQuery] string propertyName, [FromQuery] string value)
    {
        try
        {
            return Ok(await _userService.GetFilteredByPropertyAsync(propertyName, value));
        }
        catch (PropertyFilterException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserDTO>> GetAsync(string id)
    {
        var current_user = await _userService.GetByIdAsync(id);

        if (current_user == null) return NotFound();

        return Ok(current_user);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync(UserDTO dto)
    {
        var current_user = _mapper.Map<UserDTO>(dto);

        await _userService.CreateAsync(current_user);

        return CreatedAtAction(nameof(GetAsync), new { id = current_user.Id }, current_user);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync(string id, UserDTO dto) => 
        await _userService.UpdateAsync(id, _mapper.Map<UserDTO>(dto)) ? NoContent() : NotFound();

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id) => await _userService.DeleteByIdAsync(id) ? NoContent() : NotFound();

    [HttpDelete]
    public async Task<IActionResult> DeleteAll()
    {
        await _userService.DeleteAllAsync();
        return Ok();
    }
}
