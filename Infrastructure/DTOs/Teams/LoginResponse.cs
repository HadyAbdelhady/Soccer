namespace Business.DTOs.Teams
{
    public class LoginResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string Message { get; set; } = null!;
    }
}
