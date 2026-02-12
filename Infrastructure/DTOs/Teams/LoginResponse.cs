namespace Business.DTOs.Teams
{
    public class LoginResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string Role { get; set; } = null!;
        public string AccessToken { get; set; } = null!;
        public string? RefreshToken { get; set; }
        public string Message { get; set; } = null!;
    }
}
