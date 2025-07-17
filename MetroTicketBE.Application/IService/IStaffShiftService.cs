
using MetroTicketBE.Domain.DTO.Auth;
using MetroTicketBE.Domain.DTO.StaffShift;

namespace MetroTicketBE.Application.IService;

public interface IStaffShiftService
{
    Task<ResponseDTO> GetAllStaffShifts();
    Task<ResponseDTO> CraeteStaffShift(CreateShiftDTO createShiftDTO);
    Task<ResponseDTO> UpdateStaffShift(Guid id, UpdateShiftDTO dto);
}