using AutoMapper;
using AutoMapper.QueryableExtensions;
using MetroTicketBE.Application.IService;
using MetroTicketBE.Domain.DTO.Auth;
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
            if (dto.ShiftId == Guid.Empty)
            {
                return new ResponseDTO()
                {
                    IsSuccess = false,
                    Message = "Ca làm việc không hợp lệ.",
                    Result = null,
                    StatusCode = 400,
                };
            }
            var workingStation = _unitOfWork.StationRepository.GetAsync(s => s.Id == dto.WorkingStationId);
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


            var schedule = new StaffSchedule()
            {
                StaffId = dto.StaffId,
                ShiftId = dto.ShiftId,
                WorkingDate = dto.WorkingDate,
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
            var schedules =  _unitOfWork.StaffScheduleRepository.GetSchedules(startDate, endDate);
            var shedulesDTO = await schedules
                .ProjectTo<GetScheduleDTO>(_mapper.ConfigurationProvider)
                .ToListAsync();
            return new ResponseDTO()
            {
                IsSuccess = true,
                Message = "Lấy danh sách ca làm việc thành công.",
                Result =  shedulesDTO,
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
            
            var schedules =  _unitOfWork.StaffScheduleRepository.GetByStationIdAndDate(stationId, workingDate);
            var shedulesDTO = await schedules
                .ProjectTo<GetScheduleDTO>(_mapper.ConfigurationProvider)
                .ToListAsync();
            return new ResponseDTO()
            {
                IsSuccess = true,
                Message = "Lấy danh sách ca làm việc theo trạm và ngày thành công.",
                Result = shedulesDTO,
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
}