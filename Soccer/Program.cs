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
            var builder = WebApplication.CreateBuilder(args);

            // Register EF Core interceptor used by the DbContext
            builder.Services.AddSingleton<SoftDeleteInterceptor>();

            // Database Configuration (register DbContext with interceptor from DI)
            builder.Services.AddDbContext<SoccerDbContext>((provider, options) =>
            {
                // Enable a retry-on-failure execution strategy and set a command timeout
                options.UseSqlServer(
                        builder.Configuration.GetConnectionString("DefaultConnection"),
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

            // Auth: JWT + password hashing + auth service
            builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
            builder.Services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
            builder.Services.AddScoped<IAuthService, AuthService>();

            var jwtSecret = builder.Configuration["Jwt:SecretKey"] ?? throw new InvalidOperationException("Jwt:SecretKey is not set. Add it to appsettings or User Secrets.");
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
                await db.Database.MigrateAsync();
                await DataSeeder.SeedIfEmptyAsync(db);
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Educational Platform API v1");
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
