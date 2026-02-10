namespace Business.DTOs.Teams
{
    public class CreateTeamResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Message { get; set; } = string.Empty;
    }
}
