using AuthService.Db;
using AuthService.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private TokenGenerator tokenGenerator;
        private IRepository<UserCredentials> repository;

        public AuthController(IRepository<UserCredentials> repository, TokenGenerator tokenGenerator)
        {
            this.repository = repository;
            this.tokenGenerator = tokenGenerator;
        }

        [HttpPost("login")]
        public async Task<ActionResult<ResponseDTO>> LoginAsync(RequestDTO requestDTO)
        {
            try
            {
                var user = await repository.GetByEmailAsync(requestDTO.Email);
                if (user == null) return NotFound();

                if (user.Pass != requestDTO.Pass) return Unauthorized();

                (var access, var refresh) = await tokenGenerator.GenerateTokensAsync(user.Id);

                return Ok(new ResponseDTO
                {
                    Token = access,
                    RefreshToken = refresh,
                    Expires = tokenGenerator.Expires
                });
            }
            catch (ArgumentException ex)
            {
                return StatusCode(500);
            }
        }

        [Authorize]
        [HttpPost("refresh")]
        public async Task<ActionResult<ResponseDTO>> RefreshAsync(RefreshRequestDTO dto)
        {
            try
            {
                var resp = await tokenGenerator.RefreshTokenAsync(dto.Token, dto.RefreshToken);

                return Ok(new ResponseDTO
                {
                    Token = resp.access,
                    RefreshToken = resp.refresh,
                    Expires = tokenGenerator.Expires
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized();
            }
            catch (BadHttpRequestException ex)
            {
                return BadRequest();
            }
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult<ResponseDTO>> CreateAsync(RequestDTO dto)
        {
            if (await repository.ExistsAsync(dto.Email)) return BadRequest();

            var user = new UserCredentials
            {
                Email = dto.Email,
                Pass = dto.Pass
            };

            await repository.CreateAsync(user);

            return Created();
        }
    }
}
