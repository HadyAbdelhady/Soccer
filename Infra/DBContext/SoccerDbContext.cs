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
            // 🔑 UNIQUE INDEX for team logins
            modelBuilder.Entity<Team>()
                .HasIndex(t => t.Username)
                .IsUnique();

            // ↔️ MANY-TO-MANY: Teams ↔ Tournaments (generates TeamTournament join table)
            modelBuilder.Entity<Team>()
                .HasMany(t => t.Tournaments)
                .WithMany(t => t.Teams)
                .UsingEntity(j => j
                    .ToTable("TeamTournament") // Explicit table name
                    .HasKey("TeamsId", "TournamentsId")); // EF Core 6+ naming

            // 🔄 FIX AMBIGUITY: Two FKs to Team in Match
            modelBuilder.Entity<Match>()
                .HasOne(m => m.HomeTeam)
                .WithMany(t => t.HomeMatches)
                .HasForeignKey(m => m.HomeTeamId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent accidental cascade deletes

            modelBuilder.Entity<Match>()
                .HasOne(m => m.AwayTeam)
                .WithMany(t => t.AwayMatches)
                .HasForeignKey(m => m.AwayTeamId)
                .OnDelete(DeleteBehavior.Restrict);

            // 🗂️ ENUM STORAGE (Store as string for readability)
            modelBuilder.Entity<MatchCard>()
                .Property(c => c.CardType)
                .HasConversion<string>();

            modelBuilder.Entity<MatchGoal>()
                .Property(g => g.GoalType)
                .HasConversion<string>();

            modelBuilder.Entity<Player>()
                .Property(p => p.Position)
                .HasConversion<string>();

            modelBuilder.Entity<Group>()
                .HasMany(g => g.Matches)
                .WithOne(m => m.Group);
        }

        public DbSet<Team> Teams => Set<Team>();
        public DbSet<Player> Players => Set<Player>();
        public DbSet<Match> Matches => Set<Match>();
        public DbSet<Tournament> Tournaments => Set<Tournament>();
        public DbSet<MatchCard> MatchCards => Set<MatchCard>();
        public DbSet<MatchGoal> MatchGoals => Set<MatchGoal>();
        public DbSet<Group> Groups => Set<Group>();


    }
}
