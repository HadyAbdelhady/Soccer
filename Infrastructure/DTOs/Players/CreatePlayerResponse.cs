namespace Business.DTOs.Players
{
    public class CreatePlayerResponse
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = null!;
        public string Message { get; set; } = null!;
    }
}
