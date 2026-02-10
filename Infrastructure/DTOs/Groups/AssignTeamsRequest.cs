namespace Business.DTOs.Groups
{
    public class AssignTeamsRequest
    {
        public Guid GroupId { get; set; }
        public List<Guid> TeamIds { get; set; } = [];
    }

    public class AssignTeamsResponse
    {
        public string Message { get; set; } = null!;
        public int AssignedCount { get; set; }
    }
}
