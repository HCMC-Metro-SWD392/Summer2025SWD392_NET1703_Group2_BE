using MetroTicketBE.Domain.Constants;
using MetroTicketBE.Infrastructure.Context;
using MetroTicketBE.WebAPI.Extentions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MetroTicketBE.WebAPI;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddCors(
            options =>
            {
                options.AddPolicy("AllowFrontend", policyBuilder =>
                {
                    policyBuilder.WithOrigins("http://localhost:5173")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });
                

        builder.Services.AddDbContext<ApplicationDBContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString(StaticConnectionString.POSTGRE_DefaultConnection)));

        // Register services life cycle
        // Base on Extensions.ServiceCollectionExtensions
        builder.Services.RegisterService();

        // Register redis services life cycle
        // Base on Extensions.RedisServiceExtensions
        builder.AddRedisCache();

        builder.Services.Configure<IdentityOptions>(options =>
        {
            options.Lockout.AllowedForNewUsers = true;
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            options.Lockout.MaxFailedAccessAttempts = 5;
        });

        var app = builder.Build();
        //ApplyMigration();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseCors();
        app.UseAuthorization();


        app.MapControllers();

        //void ApplyMigration()
        //{
        //    using (var scope = app.Services.CreateScope())
        //    {
        //        var context = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();

        //        if (context.Database.GetPendingMigrations().Any())
        //        {
        //            context.Database.Migrate();
        //        }
        //    }
        //}
        app.Run();
    }
}