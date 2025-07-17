using MetroTicketBE.Domain.Entities;
using MetroTicketBE.Infrastructure.Context;
using MetroTicketBE.Infrastructure.IRepository;
using Microsoft.EntityFrameworkCore;

namespace MetroTicketBE.Infrastructure.Repository;

public class StaffRepository: Repository<Staff>, IStaffRepository
{
    private readonly ApplicationDBContext _context;
    public StaffRepository(ApplicationDBContext context) : base(context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<Staff?> GetByUserIdAsync(string userId)
    {
        return await _context.Staffs
            .FirstOrDefaultAsync(s => s.UserId == userId);
    }
}