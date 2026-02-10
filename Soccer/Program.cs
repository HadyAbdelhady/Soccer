using System.Reflection;
using System.Text.Json.Serialization;
using Business.Services.Groups;
using Business.Services.Tournaments;
using Business.Services.Teams;
using Business.Services.Players;
using Business.Services.Standings;
using Business.Services.Matches;
using Infra.DBContext;
using Infra.Interceptors;
using Infra.Interface;
using Infra.Persistent;
using Microsoft.EntityFrameworkCore;


namespace Soccer
{
    public class Program
    {
        public static void Main(string[] args)
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
                    Console.WriteLine(le.Message);
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

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
