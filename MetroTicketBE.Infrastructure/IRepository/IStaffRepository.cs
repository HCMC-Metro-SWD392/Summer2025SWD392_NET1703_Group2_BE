using MetroTicketBE.Domain.Entities;

namespace MetroTicketBE.Infrastructure.IRepository;

public interface IStaffRepository: IRepository<Staff>
{
    public Task<Staff?> GetByUserIdAsync(string userId);
}