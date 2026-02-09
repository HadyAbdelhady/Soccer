namespace Business.DTOs.Teams
{
    public class DeleteTeamResponse
    {
        public Guid Id { get; set; }
        public bool IsDeleted { get; set; }
        public string Message { get; set; } = null!;
    }
}
