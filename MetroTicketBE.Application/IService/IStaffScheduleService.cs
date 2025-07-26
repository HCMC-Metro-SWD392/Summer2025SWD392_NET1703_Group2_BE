using System.Security.Claims;
using MetroTicketBE.Domain.DTO.Auth;
using MetroTicketBE.Domain.DTO.StaffSchedule;

namespace MetroTicketBE.Application.IService;

public interface IStaffScheduleService
{
    Task<ResponseDTO> GetAllSchedules(DateOnly startDate, DateOnly endDate);
    Task<ResponseDTO> CreateStaffSchedule(CreateStaffScheduleDTO dto);
    Task<ResponseDTO> GetSchedulesByStationIdAndDate(Guid stationId, DateOnly workingDate);
    Task<ResponseDTO> GetSchedulesByStaffIdAndDate(ClaimsPrincipal user, DateOnly? fromDate, DateOnly? toDate);
    Task<ResponseDTO> AssignStaffToExistedSchedule(Guid staffId,Guid shiftId, Guid scheduleId, Guid? workingStationId);
    Task<ResponseDTO> GetUnscheduledStaff(Guid shiftId, DateOnly workingDate);
}