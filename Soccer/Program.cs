using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using Business.Services.Auth;
using Business.Services.Groups;
using Business.Services.Tournaments;
using Business.Services.Teams;
using Business.Services.Players;
using Business.Services.Standings;
using Business.Services.Matches;
using Infra;
using Infra.DBContext;
using Soccer.Services;
using Soccer.Services.Fcm;
using Infra.Interceptors;
using Infra.Interface;
using Infra.Persistent;
using Infra.Seeding;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;


namespace Soccer
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            try
            {
                await RunAsync(args);
            }
            catch (Exception ex)
            {
                // Log startup failure to stdout so hosting (IIS, Azure, etc.) can show it in logs
                Console.WriteLine("ASP.NET Core startup failed:");
                Console.WriteLine(ex.ToString());
                throw;
            }
        }

        private static async Task RunAsync(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Require connection string (avoid 500.30 from invalid/missing DB on production)
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException(
                    "ConnectionStrings:DefaultConnection is not set. " +
                    "On production, set it in appsettings.Production.json or environment variables (e.g. ConnectionStrings__DefaultConnection). " +
                    "Use a real SQL Server connection string; (localdb)\\MSSQLLocalDB is only for local development.");
            var isProduction = builder.Environment.IsProduction();
            if (isProduction && connectionString.Contains("localdb", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException(
                    "Production must not use LocalDB. Set ConnectionStrings:DefaultConnection to your production SQL Server (e.g. via environment variable ConnectionStrings__DefaultConnection).");

            // Register EF Core interceptor used by the DbContext
            builder.Services.AddSingleton<SoftDeleteInterceptor>();

            // Database Configuration (register DbContext with interceptor from DI)
            builder.Services.AddDbContext<SoccerDbContext>((provider, options) =>
            {
                // Enable a retry-on-failure execution strategy and set a command timeout
                options.UseSqlServer(
                        connectionString,
                        sqlServerOptions => sqlServerOptions.EnableRetryOnFailure()
                                                        .CommandTimeout(30)
                    ).AddInterceptors(provider.GetRequiredService<SoftDeleteInterceptor>());
            });

            // Unit of Work & Repository Registration
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Open generic base repository
            builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            builder.Services.AddScoped<IGroupService, GroupService>();
            builder.Services.AddScoped<ITournamentService, TournamentService>();
            builder.Services.AddScoped<ITeamService, TeamService>();
            builder.Services.AddScoped<IPlayerService, PlayerService>();
            builder.Services.AddScoped<IStandingsService, StandingsService>();
            builder.Services.AddScoped<IMatchService, MatchService>();

            // FCM push notifications (lineup reminder)
            builder.Services.AddSingleton<IFcmService, FcmService>();
            builder.Services.AddHostedService<LineupReminderBackgroundService>();
            builder.Services.AddHostedService<MatchStatusBackgroundService>();

            // Auth: JWT + password hashing + auth service
            builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
            builder.Services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
            builder.Services.AddScoped<IAuthService, AuthService>();

            var jwtSecret = builder.Configuration["Jwt:SecretKey"];
            if (string.IsNullOrWhiteSpace(jwtSecret))
                throw new InvalidOperationException(
                    "Jwt:SecretKey is not set. For production, set it in appsettings.Production.json or environment variables (e.g. Jwt__SecretKey). " +
                    "Use a secure key at least 32 characters long.");
            var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "SoccerApi";
            var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "SoccerApiUsers";
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
                        ValidateIssuer = true,
                        ValidIssuer = jwtIssuer,
                        ValidateAudience = true,
                        ValidAudience = jwtAudience,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero
                    };
                });

            // Auto-register concrete repository implementations (Scrutor)
            try
            {
                builder.Services.Scan(scan => scan
                    .FromAssembliesOf(typeof(UnitOfWork))
                    .AddClasses(classes => classes.Where(c => c.Name.EndsWith("Repository")))
                    .AsImplementedInterfaces()
                    .WithScopedLifetime());
            }
            catch (ReflectionTypeLoadException ex)
            {
                Console.WriteLine("ReflectionTypeLoadException caught!");
                foreach (var le in ex.LoaderExceptions)
                    Console.WriteLine(le?.Message ?? "(null)");
            }


            // CORS Configuration (optional - configure as needed)
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Ensure database is migrated and seed bogus data when empty (for testing)
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<SoccerDbContext>();
                try
                {
                    //await db.Database.MigrateAsync();
                    //await DataSeeder.SeedIfEmptyAsync(db);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Database migration or seeding failed. Check ConnectionStrings:DefaultConnection (use a real SQL Server on production, not LocalDB).");
                    Console.WriteLine(ex.ToString());
                    throw;
                }
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Soccer API v1");
                c.RoutePrefix = string.Empty; // Swagger UI at root
            });

            app.UseCors("AllowAll");

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                // app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            await app.RunAsync();
        }
    }
}
