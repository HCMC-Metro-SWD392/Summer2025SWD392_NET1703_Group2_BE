using MetroTicketBE.Domain.Entities;

namespace MetroTicketBE.Infrastructure.IRepository
{
    public interface IPromotionRepository : IRepository<Promotion>
    {
        Task<Promotion?> GetByCodeAsync(string code);
        Task<Promotion?> GetByIdAsync(Guid promotionId);
        Task<bool> IsExistByCode(string code);
        Task<bool> IsExistByCodeExceptId(string code, Guid exceptId);
    }
}
