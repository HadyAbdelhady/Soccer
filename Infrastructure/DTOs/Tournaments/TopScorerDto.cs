namespace Business.DTOs.Tournaments
{
    public class TopScorerDto
    {
        public Guid PlayerId { get; set; }
        public string PlayerName { get; set; } = null!;
        public string TeamName { get; set; } = null!;
        public int Goals { get; set; }
    }
}
