using MetroTicketBE.Domain.Entities;
using MetroTicketBE.Infrastructure.Context;
using MetroTicketBE.Infrastructure.IRepository;
using Microsoft.EntityFrameworkCore;

namespace MetroTicketBE.Infrastructure.Repository;

public class StaffShiftRepository: Repository<StaffShift>, IStaffShiftRepository
{
    private readonly ApplicationDBContext _context;

    public StaffShiftRepository(ApplicationDBContext context) : base(context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }
    
    public Task<bool> IsExistedByName(string name)
    {
        return _context.StaffShifts.AnyAsync(s => s.ShiftName.ToLower() == name.ToLower());
    }
    public Task<bool> IsSameShiftExists(TimeSpan startTime, TimeSpan endTime)
    {
        return _context.StaffShifts.FirstOrDefaultAsync(s => s.StartTime == startTime && s.EndTime == endTime)
            .ContinueWith(task => task.Result != null);
    }
    
}