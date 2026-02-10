using Infra.enums;

namespace Business.DTOs.Players
{
    public class UpdatePlayerRequest
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = null!;
        public string NickName { get; set; } = null!;
        public PlayerPosition Position { get; set; }
        public int JerseyNumber { get; set; }
        public bool IsCaptain { get; set; }
        public Guid TeamId { get; set; }
    }
}
