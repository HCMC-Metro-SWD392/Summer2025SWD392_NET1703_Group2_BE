using MetroTicket.Domain.Entities;
using MetroTicketBE.Application.IService;
using MetroTicketBE.Application.Service;
using MetroTicketBE.Infrastructure.Context;
using MetroTicketBE.Infrastructure.IRepository;
using MetroTicketBE.Infrastructure.Repository;
using Microsoft.AspNetCore.Identity;
using System.Runtime.CompilerServices;

namespace MetroTicketBE.WebAPI.Extentions
{
    public static class ServiceCollectionExtentions
    {
        public static IServiceCollection RegisterService(this IServiceCollection services)
        {
            // Register services

            // Registering IAuthService with its implementation AuthService
            services.AddScoped<IAuthService, AuthService>();
            // Registering IUnitOfWork with its implementation UnitOfWork
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            // Registering IUserManagerRepository with its implementation UserManagerRepository
            services.AddScoped<IUserManagerRepository, UserManagerRepository>();
            // Registering ITokenService with its implementation TokenService
            services.AddScoped<ITokenService, TokenService>();
            // Registering IRedisService with its implementation RedisService
            services.AddScoped<IRedisService, RedisService>();

            // Register the Identity services with default configuration
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDBContext>()
                .AddDefaultTokenProviders();

            return services;
        }
    }
}
