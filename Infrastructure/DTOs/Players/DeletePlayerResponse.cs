namespace Business.DTOs.Players
{
    public class DeletePlayerResponse
    {
        public Guid Id { get; set; }
        public bool IsDeleted { get; set; }
        public string Message { get; set; } = null!;
    }
}
