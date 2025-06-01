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
            // Registering ITokenService with its implementation TokenService
            services.AddScoped<ITokenService, TokenService>();
            // Registering IRedisService with its implementation RedisService
            services.AddScoped<IRedisService, RedisService>();
            // Registering IEmailService with its implementation EmailSerivce
            services.AddScoped<IEmailService, EmailSerivce>();
            // Registering ICustomerService with its implementation CustomerService
            services.AddScoped<ICustomerService, CustomerService>();
            // Registering ICustomerService with its implementation CustomerService
            services.AddScoped<IFareRuleService, FareRuleService>();
            // Registering ICustomerService with its implementation CustomerService
            services.AddScoped<IStationService, StationService>();

            // Register the Identity services with default configuration
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDBContext>()
                .AddDefaultTokenProviders();

            return services;
        }
    }
}
