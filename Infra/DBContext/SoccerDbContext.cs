using Data.Entities;
using Infra.Interceptors;
using Microsoft.EntityFrameworkCore;

namespace Infra.DBContext
{
    public class SoccerDbContext(DbContextOptions<SoccerDbContext> options) : DbContext(options)
    {
        private readonly SoftDeleteInterceptor _softDeleteInterceptor = new();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.AddInterceptors(_softDeleteInterceptor);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Team>()
                .HasQueryFilter(x => !x.IsDeleted)
                .HasIndex(t => t.Username)
                .IsUnique();

            modelBuilder.Entity<Team>()
                .HasQueryFilter(x => !x.IsDeleted)
                .HasMany(t => t.Tournaments)
                .WithMany(t => t.Teams)
                .UsingEntity(j => j
                    .ToTable("TeamTournament")
                    .HasKey("TeamsId", "TournamentsId"));

            modelBuilder.Entity<Match>()
                .HasQueryFilter(x => !x.IsDeleted)
                .HasOne(m => m.HomeTeam)
                .WithMany(t => t.HomeMatches)
                .HasForeignKey(m => m.HomeTeamId)
                .OnDelete(DeleteBehavior.Restrict); 

            modelBuilder.Entity<Match>()
                .HasQueryFilter(x => !x.IsDeleted)
                .HasOne(m => m.AwayTeam)
                .WithMany(t => t.AwayMatches)
                .HasForeignKey(m => m.AwayTeamId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MatchCard>()
                .HasQueryFilter(x => !x.IsDeleted)
                .Property(c => c.CardType)
                .HasConversion<string>();

            modelBuilder.Entity<MatchCard>()
                .HasOne(c => c.Team)
                .WithMany()
                .HasForeignKey(c => c.TeamId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<MatchGoal>()
                .HasQueryFilter(x => !x.IsDeleted)
                .Property(g => g.GoalType)
                .HasConversion<string>();

            modelBuilder.Entity<MatchGoal>()
                .HasOne(g => g.Team)
                .WithMany()
                .HasForeignKey(g => g.TeamId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Player>()
                .HasQueryFilter(x => !x.IsDeleted)
                .Property(p => p.Position)
                .HasConversion<string>();

            modelBuilder.Entity<Match>()
                .Property(m => m.Status)
                .HasConversion<string>();

            modelBuilder.Entity<Group>()
                .HasQueryFilter(x => !x.IsDeleted)
                .HasMany(g => g.Matches)
                .WithOne(m => m.Group);

            modelBuilder.Entity<Group>()
                .HasMany(g => g.Teams)
                .WithOne(t => t.Group)
                .HasForeignKey(t => t.GroupId);

            modelBuilder.Entity<Tournament>()
                .HasQueryFilter(x => !x.IsDeleted)
                .HasMany(t => t.Groups)
                .WithOne(g => g.Tournament);

            modelBuilder.Entity<Tournament>()
                .Property(t => t.Type)
                .HasConversion<string>();

            modelBuilder.Entity<Tournament>()
                .Property(t => t.Legs)
                .HasConversion<string>();

            modelBuilder.Entity<MatchLineup>()
                .HasQueryFilter(x => !x.IsDeleted)
                .HasOne(l => l.Match)
                .WithMany(m => m.Lineups)
                .HasForeignKey(l => l.MatchId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MatchLineup>()
                .HasOne(l => l.Team)
                .WithMany()
                .HasForeignKey(l => l.TeamId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<MatchLineup>()
                .HasOne(l => l.Player)
                .WithMany()
                .HasForeignKey(l => l.PlayerId)
                .OnDelete(DeleteBehavior.NoAction);
        }

        public DbSet<Team> Teams => Set<Team>();
        public DbSet<Player> Players => Set<Player>();
        public DbSet<Match> Matches => Set<Match>();
        public DbSet<Tournament> Tournaments => Set<Tournament>();
        public DbSet<MatchCard> MatchCards => Set<MatchCard>();
        public DbSet<MatchGoal> MatchGoals => Set<MatchGoal>();
        public DbSet<Group> Groups => Set<Group>();
        public DbSet<MatchLineup> MatchLineups => Set<MatchLineup>();


    }
}
