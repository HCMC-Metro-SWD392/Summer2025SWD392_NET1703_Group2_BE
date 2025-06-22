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

    public IQueryable<StaffSchedule> GetSchedules(DateOnly startDate, DateOnly endDate)
    {
        var schedules = _context.StaffSchedules
            .Where(s => s.WorkingDate >= startDate && s.WorkingDate <= endDate);
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
    public IQueryable<StaffSchedule> GetByStationIdAndDate(Guid stationId, DateOnly workingDate)
    {
        var schedules = _context.StaffSchedules
            .Where(s => s.WorkingStationId == stationId && s.WorkingDate == workingDate);
        return schedules;
    }
}