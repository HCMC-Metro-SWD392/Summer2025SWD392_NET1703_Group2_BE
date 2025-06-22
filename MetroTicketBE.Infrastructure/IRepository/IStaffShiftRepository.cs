using MetroTicketBE.Domain.Entities;

namespace MetroTicketBE.Infrastructure.IRepository;

public interface IStaffShiftRepository: IRepository<StaffShift>
{
    Task<bool> IsExistedByName(string name);
    Task<bool> IsSameShiftExists(TimeSpan startTime, TimeSpan endTime);
}