using MetroTicketBE.Domain.Entities;

namespace MetroTicketBE.Infrastructure.IRepository;

public interface IStaffScheduleRepository : IRepository<StaffSchedule>
{
    Task<List<StaffSchedule>> GetSchedules(DateOnly startDate, DateOnly endDate);
    Task<List<StaffSchedule>> GetSchedulesForStaff(Guid staffId, DateOnly? fromDate, DateOnly? toDate);
    Task<List<StaffSchedule>> GetByStationIdAndDate(Guid stationId, DateOnly workingDate);
    Task<StaffSchedule?> GetByStaffIdDateShift(Guid staffId, DateOnly workingDate, Guid shiftId);
    Task<List<Staff>> GetUnscheduledStaffAsync(Guid shiftId, DateOnly workingDate);
    Task<bool> IsExisted(Guid staffId, DateOnly workingDate, Guid shiftId);
    Task<bool> HasTimeConflictAsync(Guid staffId, DateOnly workingDate, TimeSpan newStartTime, TimeSpan newEndTime);
}