using MetroTicket.Domain.Entities;

namespace MetroTicketBE.Infrastructure.IRepository
{
    public interface ITokenService
    {
        Task<string> GenerateJwtAccessTokenAsync(ApplicationUser user);
        Task<string> GenerateJwtRefreshTokenAsync(ApplicationUser user, bool rememberMe);
        Task<bool> StoreRefreshToken(string userId, string refreshToken, bool rememberMe);
        Task<bool> DeleteRefreshToken(string userId);
        Task<string> GetQRCodeAndRefreshAsync(Guid ticketId);
        string GenerateQRCodeAsync();
        Task<string?> GetValueByKeyAsync(string key);
    }
}
