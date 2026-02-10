namespace Business.DTOs.Players
{
    public class UpdatePlayerResponse
    {
        public string Message { get; set; } = null!;
        public Guid Id { get; set; }
        public string FullName { get; set; } = null!;
    }
}
