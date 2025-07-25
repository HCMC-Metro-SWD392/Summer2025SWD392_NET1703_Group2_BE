using System.Security.Claims;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MetroTicketBE.Application.IService;
using MetroTicketBE.Domain.DTO.Auth;
using MetroTicketBE.Domain.DTO.Staff;
using MetroTicketBE.Domain.DTO.StaffSchedule;
using MetroTicketBE.Domain.DTO.StaffShift;
using MetroTicketBE.Domain.Entities;
using MetroTicketBE.Domain.Enum;
using MetroTicketBE.Infrastructure.IRepository;
using Microsoft.EntityFrameworkCore;

namespace MetroTicketBE.Application.Service;

public class StaffScheduleService: IStaffScheduleService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    public StaffScheduleService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<ResponseDTO> CreateStaffSchedule(CreateStaffScheduleDTO dto)
    {
        try
        {
            // var existingSchedule = await _unitOfWork.StaffScheduleRepository
            //     .GetByStaffIdDateShift(dto.StaffId, dto.WorkingDate, dto.ShiftId);
            // if (existingSchedule is not null)
            // {
            //     return new ResponseDTO()
            //     {
            //         IsSuccess = false,
            //         Message = "Nhân viên đã có ca làm việc này vào ngày này.",
            //         Result = null,
            //         StatusCode = 400,
            //     };
            // }
            var isExistSchedule =
                await _unitOfWork.StaffScheduleRepository.DoesStaffHaveSchedule(dto.StaffId, dto.WorkingDate);
            if (isExistSchedule)
            {
                return new ResponseDTO()
                {
                    IsSuccess = false,
                    Message = "Nhân viên đã có ca làm việc vào ngày này.",
                    Result = null,
                    StatusCode = 400,
                };
            }
            if (dto.WorkingDate < DateOnly.FromDateTime(DateTime.Now))
            {
                return new ResponseDTO()
                {
                    IsSuccess = false,
                    Message = "Ngày làm việc không hợp lệ. Ngày làm việc phải là ngày hiện tại hoặc tương lai.",
                    Result = null,
                    StatusCode = 400,
                };
            }
            var shift = await _unitOfWork.StaffShiftRepository.GetAsync(s => s.Id == dto.ShiftId);
            if (shift is null)
            {
                return new ResponseDTO()
                {
                    IsSuccess = false,
                    Message = "Ca làm việc không hợp lệ.",
                    Result = null,
                    StatusCode = 400,
                };
            }
            var workingStation = await _unitOfWork.StationRepository.GetAsync(s => s.Id == dto.WorkingStationId);
            if (workingStation is null)
            {
                return new ResponseDTO()
                {
                    IsSuccess = false,
                    Message = "Trạm làm việc không hợp lệ.",
                    Result = null,
                    StatusCode = 400,
                };
            }
            // var isConflictTime = await _unitOfWork.StaffScheduleRepository.HasTimeConflictAsync(dto.StaffId, dto.WorkingDate, shift.StartTime, shift.EndTime);
            // if (isConflictTime)
            // {
            //     return new ResponseDTO()
            //     {
            //         IsSuccess = false,
            //         Message = "Nhân viên đã có ca làm việc trùng thời gian với ca làm việc này.",
            //         Result = null,
            //         StatusCode = 400,
            //     };
            // }
            var schedule = new StaffSchedule()
            {
                StaffId = dto.StaffId,
                ShiftId = dto.ShiftId,
                WorkingDate = dto.WorkingDate,
                StartTime = shift.StartTime,
                EndTime = shift.EndTime,
                WorkingStationId = dto.WorkingStationId,
                Status = StaffScheduleStatus.Normal
            };
            await _unitOfWork.StaffScheduleRepository.AddAsync(schedule);
            await _unitOfWork.SaveAsync();
            return new ResponseDTO()
            {
                IsSuccess = true,
                Message = "Tạo ca làm việc thành công.",
                Result = _mapper.Map<GetScheduleDTO>(schedule),
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
            var schedules =  await _unitOfWork.StaffScheduleRepository.GetSchedules(startDate, endDate);
          
            return new ResponseDTO()
            {
                IsSuccess = true,
                Message = "Lấy danh sách ca làm việc thành công.",
                Result =  _mapper.Map<List<GetScheduleDTO>>(schedules),
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
    
    public async Task<ResponseDTO> GetSchedulesByStationIdAndDate(Guid stationId, DateOnly workingDate)
    {
        try
        {
            var isExistedStation = await _unitOfWork.StationRepository.IsExistById(stationId);
            if (!isExistedStation)
            {
                return new ResponseDTO()
                {
                    IsSuccess = false,
                    Message = "Trạm làm việc không tồn tại.",
                    Result = null,
                    StatusCode = 404,
                };
            }
            
            var schedules = await  _unitOfWork.StaffScheduleRepository.GetByStationIdAndDate(stationId, workingDate);
            
            return new ResponseDTO()
            {
                IsSuccess = true,
                Message = "Lấy danh sách ca làm việc theo trạm và ngày thành công.",
                Result =  _mapper.Map<List<GetScheduleDTO>>(schedules),
                StatusCode = 200,
            };
        }
        catch (Exception ex)
        {
            return new ResponseDTO()
            {
                IsSuccess = false,
                Message = "Đã xảy ra lỗi khi lấy danh sách ca làm việc theo trạm và ngày: " + ex.Message,
                Result = null,
                StatusCode = 500,
            };
        }
    }
    
    public async Task<ResponseDTO> GetSchedulesByStaffIdAndDate(ClaimsPrincipal user, DateOnly? fromDate = null, DateOnly? toDate = null)
    {
        try
        {
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return new ResponseDTO()
                {
                    IsSuccess = false,
                    Message = "Không tìm thấy thông tin người dùng.",
                    Result = null,
                    StatusCode = 400,
                };
            }
            var staff = await _unitOfWork.StaffRepository.GetAsync(s => s.UserId == userId);
            if (staff is null)
            {
                return new ResponseDTO()
                {
                    IsSuccess = false,
                    Message = "Nhân viên không tồn tại.",
                    Result = null,
                    StatusCode = 404,
                };
            }
            var schedules = await _unitOfWork.StaffScheduleRepository.GetSchedulesForStaff(staff.Id, fromDate, toDate);
            if (schedules is null)
            {
                return new ResponseDTO()
                {
                    IsSuccess = false,
                    Message = "Không tìm thấy ca làm việc cho nhân viên vào ngày này.",
                    Result = null,
                    StatusCode = 404,
                };
            }
            return new ResponseDTO()
            {
                IsSuccess = true,
                Message = "Lấy danh sách ca làm việc theo nhân viên và ngày thành công.",
                Result = _mapper.Map<List<GetScheduleDTO>>(schedules),
                StatusCode = 200,
            };
        }
        catch (Exception ex)
        {
            return new ResponseDTO()
            {
                IsSuccess = false,
                Message = "Đã xảy ra lỗi khi lấy danh sách ca làm việc theo nhân viên và ngày: " + ex.Message,
                Result = null,
                StatusCode = 500,
            };
        }
    }
    
    public async Task<ResponseDTO> GetUnscheduledStaff(Guid shiftId, DateOnly workingDate)
    {
        try
        {
            var unscheduledStaff = await _unitOfWork.StaffScheduleRepository.GetUnscheduledStaffAsync(shiftId, workingDate);
            if (unscheduledStaff is null || !unscheduledStaff.Any())
            {
                return new ResponseDTO()
                {
                    IsSuccess = false,
                    Message = "Không tìm thấy nhân viên chưa được sắp xếp cho ca làm việc này.",
                    Result = null,
                    StatusCode = 404,
                };
            }
            return new ResponseDTO()
            {
                IsSuccess = true,
                Message = "Lấy danh sách nhân viên chưa được sắp xếp thành công.",
                Result = _mapper.Map<List<GetStaffDTO>>(unscheduledStaff),
                StatusCode = 200,
            };
        }
        catch (Exception ex)
        {
            return new ResponseDTO()
            {
                IsSuccess = false,
                Message = "Đã xảy ra lỗi khi lấy danh sách nhân viên chưa được sắp xếp: " + ex.Message,
                Result = null,
                StatusCode = 500,
            };
        }
    }

    public async Task<ResponseDTO> AssignStaffToExistedSchedule(Guid staffId, Guid scheduleId, Guid? workingStationId = null)
    {
        var schedule = await _unitOfWork.StaffScheduleRepository.GetAsync(s => s.Id == scheduleId);
        if (schedule is null)
        {
            return new ResponseDTO()
            {
                IsSuccess = false,
                Message = "Ca làm việc không tồn tại.",
                Result = null,
                StatusCode = 404,
            };
        }
        // var staff = await _unitOfWork.StaffRepository.GetAsync(s => s.Id == staffId);
        // if (staff is null)
        // {
        //     return new ResponseDTO()
        //     {
        //         IsSuccess = false,
        //         Message = "Nhân viên không tồn tại.",
        //         Result = null,
        //         StatusCode = 404,
        //     };
        // }

            // var isStaffHasSchedule = await _unitOfWork.StaffScheduleRepository.IsExisted(staffId , schedule.WorkingDate, schedule.ShiftId);
            // if (isStaffHasSchedule)
            // {
            //     return new ResponseDTO()
            //     {
            //         IsSuccess = false,
            //         Message = "Nhân viên đã có ca làm việc này vào ngày này.",
            //         Result = null,
            //         StatusCode = 400,
            //     };
            // }
        
        schedule.WorkingStationId = workingStationId ?? schedule.WorkingStationId;
        schedule.StaffId = staffId;
        _unitOfWork.StaffScheduleRepository.Update(schedule);
        await _unitOfWork.SaveAsync();
        return new ResponseDTO()
        {
            IsSuccess = true,
            Message = "Gán nhân viên vào ca làm việc thành công.",
            Result = null,
            StatusCode = 200,
        };
    }
}