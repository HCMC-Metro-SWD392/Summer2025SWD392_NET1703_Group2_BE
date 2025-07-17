using MetroTicketBE.Domain.Entities;

namespace MetroTicketBE.Infrastructure.IRepository;

public interface IStaffScheduleRepository : IRepository<StaffSchedule>
{
    Task<List<StaffSchedule>> GetSchedules(DateOnly startDate, DateOnly endDate);
    Task<List<StaffSchedule>> GetSchedulesForStaff(Guid staffId, DateOnly? fromDate, DateOnly? toDate);
    Task<List<StaffSchedule>> GetByStationIdAndDate(Guid stationId, DateOnly workingDate);
    Task<StaffSchedule?> GetByStaffIdDateShift(Guid staffId, DateOnly workingDate, Guid shiftId);
}