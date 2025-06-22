using MetroTicketBE.Domain.Entities;

namespace MetroTicketBE.Infrastructure.IRepository;

public interface IStaffScheduleRepository: IRepository<StaffSchedule>
{
    IQueryable<StaffSchedule> GetSchedules(DateOnly startDate, DateOnly endDate);
    Task<StaffSchedule?> GetByStaffIdAndDate(Guid staffId, DateOnly workingDate);
    IQueryable<StaffSchedule> GetByStationIdAndDate(Guid stationId, DateOnly workingDate);
}