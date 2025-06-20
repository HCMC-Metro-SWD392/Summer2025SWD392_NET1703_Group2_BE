using MetroTicketBE.Domain.Entities;
using MetroTicketBE.Infrastructure.Context;
using MetroTicketBE.Infrastructure.IRepository;
using Microsoft.EntityFrameworkCore;

namespace MetroTicketBE.Infrastructure.Repository;

public class StaffScheduleRepository: Repository<StaffSchedule>, IStaffScheduleRepository
{
    private readonly ApplicationDBContext _context;
    public StaffScheduleRepository(ApplicationDBContext context) : base(context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public Task<List<StaffSchedule>> GetSchedules(DateOnly startDate, DateOnly endDate)
    {
        var schedules = _context.StaffSchedules
            .Include(s => s.Staff)
            .Include(s => s.Shift)
            .Where(s => s.WorkingDate >= startDate && s.WorkingDate <= endDate)
            .ToListAsync();
        return schedules;
    }
    
    public Task<StaffSchedule?> GetByStaffIdAndDate(Guid staffId, DateOnly workingDate)
    {
        var schedule = _context.StaffSchedules
            .Include(s => s.Staff)
            .Include(s => s.Shift)
            .FirstOrDefaultAsync(s => s.StaffId == staffId && s.WorkingDate == workingDate);
        return schedule;
    }
}