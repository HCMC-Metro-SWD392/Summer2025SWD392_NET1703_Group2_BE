using MetroTicketBE.Application.IService;
using MetroTicketBE.Domain.DTO.Auth;
using MetroTicketBE.Domain.DTO.StaffSchedule;
using MetroTicketBE.Domain.DTO.StaffShift;
using MetroTicketBE.Domain.Entities;
using MetroTicketBE.Domain.Enum;
using MetroTicketBE.Infrastructure.IRepository;

namespace MetroTicketBE.Application.Service;

public class StaffScheduleService: IStaffScheduleService
{
    private readonly IUnitOfWork _unitOfWork;
    public StaffScheduleService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<ResponseDTO> CreateStaffSchedule(CreateStaffScheduleDTO dto)
    {
        try
        {
            var existingSchedule = await _unitOfWork.StaffScheduleRepository
                .GetByStaffIdAndDate(dto.StaffId, dto.WorkingDate);
            if (existingSchedule is not null)
            {
                return new ResponseDTO()
                {
                    IsSuccess = false,
                    Message = "Nhân viên đã có ca làm việc vào ngày này.",
                    Result = null,
                    StatusCode = 400,
                };
            }

            var schedule = new StaffSchedule()
            {
                StaffId = dto.StaffId,
                ShiftId = dto.ShiftId,
                WorkingDate = dto.WorkingDate,
                Status = StaffScheduleStatus.Normal
            };
            await _unitOfWork.StaffScheduleRepository.AddAsync(schedule);
            await _unitOfWork.SaveAsync();
            return new ResponseDTO()
            {
                IsSuccess = true,
                Message = "Tạo ca làm việc thành công.",
                Result = schedule,
                StatusCode = 201,
            };
        }
        catch (Exception ex)
        {
            return new ResponseDTO()
            {
                IsSuccess = false,
                Message = "Đã xảy ra lỗi khi tạo ca làm việc: " + ex.Message,
                Result = null,
                StatusCode = 500,
            };
        }
    }
    
    public async Task<ResponseDTO> GetAllSchedules(DateOnly startDate, DateOnly endDate)
    {
        try
        {
            var schedules = await _unitOfWork.StaffScheduleRepository.GetSchedules(startDate, endDate);
            return new ResponseDTO()
            {
                IsSuccess = true,
                Message = "Lấy danh sách ca làm việc thành công.",
                Result = schedules,
                StatusCode = 200,
            };
        }
        catch (Exception ex)
        {
            return new ResponseDTO()
            {
                IsSuccess = false,
                Message = "Đã xảy ra lỗi khi lấy danh sách ca làm việc: " + ex.Message,
                Result = null,
                StatusCode = 500,
            };
        }
    }
}