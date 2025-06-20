using MetroTicketBE.Application.IService;
using MetroTicketBE.Domain.DTO.Auth;
using MetroTicketBE.Domain.DTO.StaffShift;
using MetroTicketBE.Domain.Entities;
using MetroTicketBE.Infrastructure.IRepository;

namespace MetroTicketBE.Application.Service;

public class StaffShiftService: IStaffShiftService
{
    private readonly IUnitOfWork _unitOfWork;
    
    public StaffShiftService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<ResponseDTO> CraeteStaffShift(CreateShiftDTO dto)
    {
        try
        {
            if (dto.StartTime >= dto.EndTime)
            {
                return new ResponseDTO()
                {
                    IsSuccess = false,
                    StatusCode = 400,
                    Message = "Thời gian bắt đầu phải nhỏ hơn thời gian kết thúc"
                };
            }
            var isExistShift = await _unitOfWork.StaffShiftRepository.IsExistedByName(dto.ShiftName);
            if (isExistShift)
            {
                return new ResponseDTO()
                {
                    IsSuccess = false,
                    StatusCode = 400,
                    Message = "Ca làm việc đã tồn tại"
                };
            }
            var isSameShiftExists = await _unitOfWork.StaffShiftRepository.IsSameShiftExists(dto.StartTime, dto.EndTime);
            if (isSameShiftExists)
            {
                return new ResponseDTO()
                {
                    IsSuccess = false,
                    StatusCode = 400,
                    Message = "Ca làm việc đã tồn tại với thời gian bắt đầu và kết thúc giống nhau"
                };
            }
            var staffShift = new StaffShift
            {
                ShiftName = dto.ShiftName,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
            };
            await _unitOfWork.StaffShiftRepository.AddAsync(staffShift);
            await _unitOfWork.SaveAsync();
            return new ResponseDTO()
            {
                IsSuccess = true,
                StatusCode = 200,
                Message = "Tạo ca làm việc thành công",
                Result = staffShift
            };
        }catch(Exception ex)
        {
            return new ResponseDTO()
            {
                IsSuccess = false,
                StatusCode = 500,
                Message = "Lỗi khi tạo ca làm việc: " + ex.Message
            };
        }
    }

    public async Task<ResponseDTO> GetAllStaffShifts()
    {
        return new ResponseDTO()
        {
            IsSuccess = true,
            StatusCode = 200,
            Message = "Lấy danh sách ca làm việc thành công",
            Result = await _unitOfWork.StaffShiftRepository.GetAllAsync()
        };
    }
}