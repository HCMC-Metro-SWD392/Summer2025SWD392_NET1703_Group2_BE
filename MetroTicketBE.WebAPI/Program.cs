using MetroTicketBE.Application.Mappings;
using MetroTicketBE.Domain.Constants;
using MetroTicketBE.Domain.Entities;
using MetroTicketBE.Infrastructure.Context;
using MetroTicketBE.Infrastructure.SignalR;
using MetroTicketBE.WebAPI.Extentions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace MetroTicketBE.WebAPI;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Cấu hình Kestrel
        //builder.WebHost.ConfigureKestrel(options =>
        //{
        //    options.ListenAnyIP(5000); // Lắng nghe trên cổng 5000 cho tất cả IP 
        //});

        builder.Configuration
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        // Add services to the container.

        builder.Services.AddControllers();
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddHttpClient();

        builder.Services.AddDbContext<ApplicationDBContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString(StaticConnectionString.POSTGRE_DefaultConnection))
            .EnableSensitiveDataLogging()
            .LogTo(Console.WriteLine, LogLevel.Information));

        // Set time token
        builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
            options.TokenLifespan = TimeSpan.FromMinutes(60));

        // Register AutoMapper 
        builder.Services.AddAutoMapper(typeof(AutoMappingProfile));

        // Register services life cycle
        // Base on Extensions.ServiceCollectionExtensions
        builder.Services.RegisterService();

        // Register redis services life cycle
        // Base on Extensions.RedisServiceExtensions
        builder.AddRedisCache();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();

        // Register Authentication
        // Base on Extensions.WebApplicationBuilderExtensions
        builder.AddAppAuthentication();

        builder.Services.AddAuthorization();

        // Register SignalR
        builder.AddSignalR();

        // Register custom UserIdProvider for SignalR
        builder.Services.AddSingleton<IUserIdProvider, CustomUserIdProvider>();

        // Register SwaggerGen and config for Authorize
        // Base on Extensions.WebApplicationBuilderExtensions
        builder.AddSwaggerGen();

        builder.Services.AddCors(
            options =>
            {
                options.AddPolicy("AllowFrontend", policyBuilder =>
                {
                    policyBuilder.WithOrigins("http://localhost:5173", "http://54.251.226.229", "https://metrohcmc.xyz")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });

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
        else
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        app.UseCors("AllowFrontend");
        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.MapHub<NotificationHub>("/notificationHub");    

        app.Run();

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
    }
}
