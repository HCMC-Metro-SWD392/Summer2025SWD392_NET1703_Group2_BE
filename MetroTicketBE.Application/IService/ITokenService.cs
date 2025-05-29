using MetroTicket.Domain.Entities;

namespace MetroTicketBE.Infrastructure.IRepository
{
    public interface ITokenService
    {
        Task<string> GenerateJwtAccessTokenAsync(ApplicationUser user);
        Task<string> GenerateJwtRefreshTokenAsync(ApplicationUser user);
    }
}
