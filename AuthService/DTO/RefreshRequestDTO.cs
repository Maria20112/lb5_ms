namespace AuthService.DTO
{
    public class RefreshRequestDTO
    {
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }
}
