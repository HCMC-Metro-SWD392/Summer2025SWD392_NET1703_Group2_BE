using AutoMapper;
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
            services.AddScoped<IEmailService, EmailService>();
            // Registering ICustomerService with its implementation CustomerService
            services.AddScoped<ICustomerService, CustomerService>();
            // Registering ICustomerService with its implementation CustomerService
            services.AddScoped<IFareRuleService, FareRuleService>();
            // Registering ICustomerService with its implementation CustomerService
            services.AddScoped<IStationService, StationService>();
            // Registering IMetroLineService with its implementation MetroLineService
            services.AddScoped<IMetroLineService, MetroLineService>();
            // Registering IPaymentService with its implementation PaymentService
            services.AddScoped<ISubscriptionService, SubscriptionService>();
            // Registering IMetroLineStationService with its implementation MetroLineStationService
            services.AddScoped<IMetroLineStationService, MetroLineStationService>();
            // Registering ITicketRouteService with its implementation TicketRouteService
            services.AddScoped<ITicketRouteService, TicketRouteService>();
            services.AddScoped<StationGraph>();
            // Registering IPaymentService with its implementation PaymentService
            services.AddScoped<IPaymentService, PaymentService>();
            // Registering IPromotionService with its implementation PromotionService
            services.AddScoped<IPromotionService, PromotionService>();
            // Registering ITicketService with its implementation TicketService
            services.AddScoped<ITicketService, TicketService>();
            // Registering ITrainScheduleService with its implementation TrainScheduleService
            services.AddScoped<ITrainScheduleService, TrainScheduleService>();
            // Registering ISubscriptionTicketTypeService with its implementation SubscriptionTicketTypeService
            services.AddScoped<ISubscriptionTicketTypeService, SubscriptionTicketTypeService>();
            // Registering IUserService with its implementation UserService
            services.AddScoped<IUserService, UserService>();
            // Registering IFareRuleRepository with its implementation FareRuleRepository
            services.AddScoped<IFareRuleRepository, FareRuleRepository>();
            // Registering ISubscriptionTicketTypeRepository with its implementation SubscriptionTicketTypeRepository
            services.AddScoped<ISubscriptionTicketTypeRepository, SubscriptionTicketTypeRepository>();
            // Registering IStaffScheduleService with its implementation StaffScheduleService
            services.AddScoped<IStaffScheduleService, StaffScheduleService>();
            // Registering IStaffShiftService with its implementation StaffShiftService
            services.AddScoped<IStaffShiftService, StaffShiftService>();
            // Registering IFormRequestService with its implementation FormRequestService
            services.AddScoped<IFormRequestService, FormRequestService>();
            // Registering IS3Service with its implementation S3Service
            services.AddScoped<IS3Service, S3Service>();
            // Registering IStaffService with its implementation StaffService
            services.AddScoped<IStaffService, StaffService>();
            // Registering IDashBoardService with its implementation DashBoardService
            services.AddScoped<IDashBoardService, DashBoardService>();
            // Registering ITicketProcessService with its implementation TicketProcessService
            services.AddScoped<ITicketProcessService, TicketProcessService>();
            // Registering ILogService with its implementation LogService
            services.AddScoped<ILogService, LogService>();

            // Register the Identity services with default configuration
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDBContext>()
                .AddDefaultTokenProviders();

            return services;
        }
    }
}
