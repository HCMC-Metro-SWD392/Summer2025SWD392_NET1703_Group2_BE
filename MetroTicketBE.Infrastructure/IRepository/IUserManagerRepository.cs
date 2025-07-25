﻿using MetroTicket.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace MetroTicketBE.Infrastructure.IRepository
{
    public interface IUserManagerRepository
    {
        Task<ApplicationUser> GetByEmailAsync(string email);
        Task<ApplicationUser> GetByPhoneNumberAsync(string phoneNumber);
        Task<IdentityResult> CreateAsync(ApplicationUser user, string password);
        Task<IdentityResult> AddtoRoleAsync(ApplicationUser user, string role);
        Task<ApplicationUser> GetByIdAsync(string id);
        Task<IdentityResult> UpdateAsync(ApplicationUser user);
        Task<bool> IsEmailExist(string email);
        Task<bool> IsPhoneNumberExist(string phoneNumber);
        Task<List<ApplicationUser>> GetAllManagerAsync();
        Task<List<ApplicationUser>> GetAllAdminAsync();
    }
}
