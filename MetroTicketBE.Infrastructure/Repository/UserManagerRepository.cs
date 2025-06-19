using MetroTicket.Domain.Entities;
using MetroTicketBE.Infrastructure.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MetroTicketBE.Infrastructure.Repository
{
    public class UserManagerRepository : IUserManagerRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserManagerRepository(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        public async Task<IdentityResult> AddtoRoleAsync(ApplicationUser user, string role)
        {
            return await _userManager.AddToRoleAsync(user, role)
                   ?? throw new KeyNotFoundException($"Không tìm thấy người dùng với số điện thoại: {user.PhoneNumber} hoặc vai trò: {role} không tồn tại");
        }

        public async Task<IdentityResult> CreateAsync(ApplicationUser user, string password)
        {
            return await _userManager.CreateAsync(user, password);
        }

        public async Task<ApplicationUser> GetByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email)
                   ?? throw new KeyNotFoundException($"Không tìm thấy email: {email}");
        }
        
        public async Task<ApplicationUser> GetByIdAsync(string id)
        {
            return await _userManager.FindByIdAsync(id)
                   ?? throw new KeyNotFoundException($"Không tìm thấy người dùng với ID: {id}");
        }

        public async Task<IdentityResult> UpdateAsync(ApplicationUser user)
        {
            return await _userManager.UpdateAsync(user);
        }

        public async Task<bool> IsEmailExist(string email)
        {
            return await _userManager.Users.AnyAsync(u => u.Email == email);
        }

        public async Task<ApplicationUser> GetByPhoneNumberAsync(string phoneNumber)
        {
            return await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber)
                ?? throw new KeyNotFoundException($"Không tìm thấy số điện thoại: {phoneNumber}");
        }

        public async Task<bool> IsPhoneNumberExist(string phoneNumber)
        {
            return await _userManager.Users.AnyAsync(u => u.PhoneNumber == phoneNumber);
        }

        public async Task<ApplicationUser> GetUserByCustomerId(Guid customerId)
        {
            return await _userManager.Users
                .FirstOrDefaultAsync(u => u.Id == customerId.ToString())
                ?? throw new KeyNotFoundException($"Không tìm thấy người dùng với ID khách hàng: {customerId}.");
        }
    }
}
