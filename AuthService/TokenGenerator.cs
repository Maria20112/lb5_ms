using AuthService.Db;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Options;

namespace AuthService
{
    public class TokenGenerator
    {
        public int Expires { get; } = 300;
        public int RefreshExpires { get; } = 600;

        private IOptions<AuthOption> authOption;
        private IRepository<UserCredentials> repository;

        public TokenGenerator(IOptions<AuthOption> authOption, IRepository<UserCredentials> repository)
        {
            this.authOption = authOption;
            this.repository = repository;
        }

        public async Task<(string access, string refresh)> GenerateTokensAsync(string userId)
        {
            var user = await repository.GetByIdAsync(userId);
            if (user == null) throw new ArgumentException("User not exist");

            return (GenerateAccessToken(user.Id, user.Email), GenerateRefreshToken(user.Id, user.Version));
        }

        private string GenerateAccessToken(string userId, string email)
        {
            var claims = new List<Claim> { new Claim(ClaimTypes.Email, email), new Claim(ClaimTypes.NameIdentifier, userId) };
            // создаем JWT-токен
            var jwt = new JwtSecurityToken(
                    issuer: authOption.Value.ISSUER,
                    audience: authOption.Value.AUDIENCE,
                    claims: claims,
                    expires: DateTime.UtcNow.Add(TimeSpan.FromSeconds(Expires)),
                    signingCredentials: new SigningCredentials(AuthOption.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }

        private string GenerateRefreshToken(string userId, int currentVersion)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Version, currentVersion.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: authOption.Value.ISSUER,
                audience: authOption.Value.AUDIENCE,
                claims: claims,
                expires: DateTime.UtcNow.Add(TimeSpan.FromSeconds(RefreshExpires)),
                signingCredentials: new SigningCredentials(AuthOption.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<(string access, string refresh)> RefreshTokenAsync(string expiredAccessToken, string refreshToken)
        {
            var userId = GetUserIdFromToken(expiredAccessToken);
            if (userId == null) throw new BadHttpRequestException("", 400);

            var refreshClaims = DecodeRefreshToken(refreshToken);

            if (userId != refreshClaims.userId) throw new UnauthorizedAccessException();
            if (!await repository.ExistsAsync(userId)) throw new UnauthorizedAccessException();
            var user = await repository.GetByIdAsync(userId);
            if (refreshClaims.version != user.Version)
                throw new UnauthorizedAccessException();

            return (GenerateAccessToken(user.Id, user.Email), GenerateRefreshToken(user.Id, user.Version));
        }

        private string? GetUserIdFromToken(string token) => new JwtSecurityTokenHandler().ReadJwtToken(token).
            Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

        private (string userId, int version) DecodeRefreshToken(string token)
        {
            try
            {
                var claims = new JwtSecurityTokenHandler().ReadJwtToken(token).Claims;
                return (claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value,
                int.Parse(claims.FirstOrDefault(x => x.Type == ClaimTypes.Version).Value));
            }
            catch (Exception ex)
            {
                throw new BadHttpRequestException("", 400);
            }
        }
    }
}
