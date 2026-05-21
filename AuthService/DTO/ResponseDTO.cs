namespace AuthService.DTO
{
    public class ResponseDTO
    {
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public int Expires { get; set; } = 0;
    }
}
