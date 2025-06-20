using MetroTicketBE.Domain.DTO.Auth;
using MetroTicketBE.Domain.DTO.StaffSchedule;

namespace MetroTicketBE.Application.IService;

public interface IStaffScheduleService
{
    Task<ResponseDTO> GetAllSchedules(DateOnly startDate, DateOnly endDate);
    Task<ResponseDTO> CreateStaffSchedule(CreateStaffScheduleDTO dto);
}