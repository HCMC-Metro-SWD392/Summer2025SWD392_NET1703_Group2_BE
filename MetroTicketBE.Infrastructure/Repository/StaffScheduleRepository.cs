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

    public async Task<List<StaffSchedule>> GetSchedules(DateOnly startDate, DateOnly endDate)
    {
        var schedules = await _context.StaffSchedules
            .Where(s => s.WorkingDate >= startDate && s.WorkingDate <= endDate)
            .Include(s => s.WorkingStation)
            .Include(s => s.Staff).ThenInclude(s => s.User)
            .Include(s => s.Shift).ToListAsync();
        return schedules;
    }
    public async Task<List<StaffSchedule>> GetSchedulesForStaff(Guid staffId, DateOnly? fromDate, DateOnly? toDate)
    {
        var query = _context.StaffSchedules
            .Where(s => s.StaffId == staffId)
            .Include(s => s.WorkingStation)
            .Include(s => s.Staff).ThenInclude(s => s.User)
            .Include(s => s.Shift)
            .AsQueryable();

        if (fromDate.HasValue)
        {
            query = query.Where(s => s.WorkingDate >= fromDate);
        }

        if (toDate.HasValue)
        {
            query = query.Where(s => s.WorkingDate <= toDate);
        }
        return await query.OrderBy(s => s.WorkingDate).ToListAsync();
    }
    public async Task<List<StaffSchedule>> GetByStationIdAndDate(Guid stationId, DateOnly workingDate)
    {
        var schedules = await _context.StaffSchedules
            .Where(s => s.WorkingStationId == stationId && s.WorkingDate == workingDate)
            .Include(s => s.WorkingStation)
            .Include(s => s.Staff).ThenInclude(s => s.User)
            .Include(s => s.Shift).ToListAsync();;
        return schedules;
    }
    
    public async Task<StaffSchedule?> GetByStaffIdAndDate(Guid staffId, DateOnly workingDate)
    {
        var schedule = await _context.StaffSchedules
            .Include(s => s.WorkingStation)
            .Include(s => s.Staff).ThenInclude(s => s.User)
            .Include(s => s.Shift)
            .FirstOrDefaultAsync(s => s.StaffId == staffId && s.WorkingDate == workingDate);
        return schedule;
    } 
}