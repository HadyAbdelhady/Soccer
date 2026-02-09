namespace Business.DTOs.Teams
{
    public class UpdateTeamRequest
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
    }
}
