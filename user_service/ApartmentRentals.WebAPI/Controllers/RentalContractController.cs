using ApartmentRentals.Data.DTOs;
using ApartmentRentals.Data.Models;
using ApartmentRentals.WebAPI.Services.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class RentalContractController : ControllerBase
{
    private readonly IRepository<RentalContract> _rentalService;
    private readonly IMapper _mapper;

    public RentalContractController(IRepository<RentalContract> rentalService, IMapper mapper)
    {
        _rentalService = rentalService;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<List<RentalContract>>> GetAllAsync() => (await _rentalService.GetAllAsync()).ToList();

    [HttpGet("{id}")]
    public async Task<ActionResult<RentalContract>> GetAsync(string id)
    {
        var rentalContract = await _rentalService.GetByIdAsync(id);

        if (rentalContract == null) return NotFound();

        return Ok(rentalContract);
    }

    [HttpGet("filter")]
    public async Task<IActionResult> GetFilteredByPropertyAsync([FromQuery] string propertyName, [FromQuery] string value)
    {
        try
        {
            return Ok(await _rentalService.GetFilteredByPropertyAsync(propertyName, value));
        }
        catch (PropertyFilterException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync(RentalContractNoIdDTO dto)
    {
        var rentalContract = _mapper.Map<RentalContract>(dto);

        await _rentalService.CreateAsync(rentalContract);

        return CreatedAtAction(nameof(GetAsync), new { id = rentalContract.Id }, rentalContract);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync(string id, RentalContractNoIdDTO dto) => 
        await _rentalService.UpdateAsync(id, _mapper.Map<RentalContract>(dto)) ? NoContent() : NotFound();

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id) => await _rentalService.DeleteByIdAsync(id) ? NoContent() : NotFound();

    [HttpDelete]
    public async Task<IActionResult> DeleteAll()
    {
        await _rentalService.DeleteAllAsync();
        return Ok();
    }
}
