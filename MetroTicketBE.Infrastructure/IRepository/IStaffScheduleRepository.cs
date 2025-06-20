using MetroTicketBE.Domain.Entities;

namespace MetroTicketBE.Infrastructure.IRepository;

public interface IStaffScheduleRepository: IRepository<StaffSchedule>
{
    Task<List<StaffSchedule>> GetSchedules(DateOnly startDate, DateOnly endDate);
    Task<StaffSchedule?> GetByStaffIdAndDate(Guid staffId, DateOnly workingDate);
}