using MetroTicketBE.Domain.Entities;

namespace MetroTicketBE.Infrastructure.IRepository
{
    public interface IFormRequestRepository : IRepository<FormRequest>
    {
        Task<FormRequest> GetByIdAsync(Guid Id);
    }
}
