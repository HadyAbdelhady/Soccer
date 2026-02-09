namespace Business.DTOs.Groups
{
    public class DeleteGroupResponse
    {
        public Guid Id { get; set; }
        public bool IsDeleted { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
