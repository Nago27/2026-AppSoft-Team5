using Microsoft.EntityFrameworkCore;
using TodoRPG.Api.Data;

namespace TodoRPG.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            const string AllowTodoRpgWeb = "AllowTodoRpgWeb";

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite("Data Source=todorpg.db"));

            // Blazor web CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(AllowTodoRpgWeb, policy =>
                {
                    policy.withOrigins(
                        "https://localhost:7137",
                        "https://localhost:5093"
                    )
                    .AllowAnyHeader()
                    .AllowAnyMethod();
                });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();

            app.UseCors(AllowTodoRpgWeb);

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
