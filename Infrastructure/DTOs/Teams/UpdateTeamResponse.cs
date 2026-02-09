namespace Business.DTOs.Teams
{
    public class UpdateTeamResponse
    {
        public string Message { get; set; } = null!;
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
    }
}
