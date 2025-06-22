using MetroTicketBE.Domain.Entities;
using MetroTicketBE.Infrastructure.Context;
using MetroTicketBE.Infrastructure.IRepository;

namespace MetroTicketBE.Infrastructure.Repository;

public class StaffRepository: Repository<Staff>, IStaffRepository
{
    private readonly ApplicationDBContext _context;
    public StaffRepository(ApplicationDBContext context) : base(context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }
    
}