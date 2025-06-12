using AutoMapper;
using MetroTicketBE.Application.IService;
using MetroTicketBE.Domain.DTO.Auth;
using MetroTicketBE.Domain.DTO.TrainSchedule;
using MetroTicketBE.Domain.Entities;
using MetroTicketBE.Domain.Enum;
using MetroTicketBE.Domain.Enums;
using MetroTicketBE.Infrastructure.IRepository;

namespace MetroTicketBE.Application.Service
{
    public class TrainScheduleService : ITrainScheduleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public TrainScheduleService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<ResponseDTO> CreateTrainSchedule(CreateTrainScheduleDTO createTrainScheduleDTO)
        {
            try
            {
                var schedulesToAdd = new List<TrainSchedule>();
                TimeSpan interval = TimeSpan.FromMinutes(12);
                TimeSpan currentTime = createTrainScheduleDTO.StartTime;
                var metroLine = await _unitOfWork.MetroLineRepository.GetByIdAsync(createTrainScheduleDTO.MetroLineId);

                while (currentTime <= createTrainScheduleDTO.EndTime)
                {
                    // Kiểm tra trùng lịch trình tại mỗi thời điểm
                    var isExistTime = await _unitOfWork.TrainScheduleRepository
                        .IsExistTrainSchedule(createTrainScheduleDTO.MetroLineId, createTrainScheduleDTO.StationId, currentTime, createTrainScheduleDTO.Direction);

                    if (!isExistTime)
                    {
                        schedulesToAdd.Add(new TrainSchedule
                        {
                            MetroLineId = createTrainScheduleDTO.MetroLineId,
                            StationId = createTrainScheduleDTO.StationId,
                            StartTime = currentTime,
                            Direction = createTrainScheduleDTO.Direction
                        });
                    }

                    currentTime = currentTime.Add(interval);
                }

                if (schedulesToAdd.Count == 0)
                {
                    return new ResponseDTO
                    {
                        StatusCode = 409,
                        Message = "Tất cả các khung giờ đã tồn tại. Không có lịch trình mới nào được tạo.",
                        IsSuccess = false
                    };
                }

                await _unitOfWork.TrainScheduleRepository.AddRangeAsync(schedulesToAdd);
                await _unitOfWork.SaveAsync();

                return new ResponseDTO
                {
                    StatusCode = 201,
                    Message = $"Đã tạo thành công {schedulesToAdd.Count} lịch trình tàu.",
                    IsSuccess = true,
                    Result = schedulesToAdd
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO
                {
                    StatusCode = 500,
                    Message = "Đã xảy ra lỗi khi tạo lịch trình tàu: " + ex.Message,
                    IsSuccess = false
                };
            }
        }
        
        public async Task<ResponseDTO> CreateMetroSchedule(Guid metroLineId)
        {
             var metroLine = await _unitOfWork.MetroLineRepository.GetByIdAsync(metroLineId);
    if (metroLine == null) return new ResponseDTO()
    {
        StatusCode = 404,
        Message = "Tuyến metro không tồn tại.",
        IsSuccess = false
    };

    var orderedStations = await _unitOfWork.MetroLineStationRepository.GetStationByMetroLineIdAsync(metroLineId);
    if (orderedStations == null || !orderedStations.Any()) return new ResponseDTO()
    {
        StatusCode = 404,
        Message = "Không tìm thấy trạm nào trong tuyến metro này.",
        IsSuccess = false
    };

    var schedulesToAdd = new List<TrainSchedule>();

    TimeSpan intervalBetweenStations = TimeSpan.FromMinutes(3);       // Thời gian di chuyển giữa các ga
    TimeSpan intervalBetweenTrains = TimeSpan.FromMinutes(12);        // Giãn cách giữa các chuyến
    TimeSpan turnaroundBreakTime = TimeSpan.FromMinutes(3);           // Thời gian nghỉ để quay đầu
    TimeSpan totalOperationDuration = TimeSpan.FromHours(17);         // Tổng thời gian vận hành
    TimeSpan firstDepartureTime = metroLine.StartTime;                // Giờ bắt đầu từ metroLine
    TimeSpan lastDepartureTime = firstDepartureTime.Add(totalOperationDuration); // Giới hạn giờ khởi hành chuyến Up cuối
    
    
    
    TimeSpan currentUpDeparture = firstDepartureTime;

    while (currentUpDeparture <= lastDepartureTime)
    {
        // === Chiều UP: từ ga đầu → ga cuối ===
        TimeSpan upTime = currentUpDeparture;
        foreach (var station in orderedStations)
        {
            schedulesToAdd.Add(new TrainSchedule
            {
                Id = Guid.NewGuid(),
                MetroLineId = metroLineId,
                StationId = station.Id,
                StartTime = upTime,
                Direction = TrainScheduleType.Up,
                Status = TrainScheduleStatus.Normal
            });

            upTime = upTime.Add(intervalBetweenStations);
        }

        // === Chiều DOWN: từ ga cuối → ga đầu (sau khi nghỉ) ===
        TimeSpan downDeparture = upTime.Add(turnaroundBreakTime);
        TimeSpan downTime = downDeparture;

        for (int i = orderedStations.Count - 1; i >= 0; i--)
        {
            schedulesToAdd.Add(new TrainSchedule
            {
                Id = Guid.NewGuid(),
                MetroLineId = metroLineId,
                StationId = orderedStations[i].Id,
                StartTime = downTime,
                Direction = TrainScheduleType.Down,
                Status = TrainScheduleStatus.Normal
            });

            downTime = downTime.Add(intervalBetweenStations);
        }

        // Tăng thời gian xuất phát của chuyến kế tiếp
        currentUpDeparture = currentUpDeparture.Add(intervalBetweenTrains);
    }

    await _unitOfWork.TrainScheduleRepository.AddRangeAsync(schedulesToAdd);
    await _unitOfWork.SaveAsync();
            return new ResponseDTO()
            {
                StatusCode = 201,
                Message = "Đã tạo lịch trình cho tất cả các trạm thành công.",
                IsSuccess = true,
                Result = null
            };
        }

        public async Task<ResponseDTO> GetTrainSchedulesByStationId(Guid stationId)
        {
            if (stationId == Guid.Empty)
            {
                return new ResponseDTO
                {
                    StatusCode = 400,
                    Message = "ID ga không hợp lệ.",
                    IsSuccess = false
                };
            }
            var schedules = await _unitOfWork.TrainScheduleRepository.GetByStationIdSortedAsync(stationId);
            return new ResponseDTO()
            {
                StatusCode = 200,
                Message = "Lấy lịch trình thành công.",
                IsSuccess = true,
                Result = schedules
            };
        }
        public async Task<ResponseDTO> CancelTrainSchedule(Guid trainScheduleId)
        {
            try
            {
                var trainSchedule = await _unitOfWork.TrainScheduleRepository.GetByIdAsync(trainScheduleId);
                if (trainSchedule is null)
                {
                    return new ResponseDTO
                    {
                        StatusCode = 404,
                        Message = "Lịch trình tàu không tồn tại.",
                        IsSuccess = false
                    };
                }

                trainSchedule.Status = TrainScheduleStatus.Cancelled;

                _unitOfWork.TrainScheduleRepository.Update(trainSchedule);
                await _unitOfWork.SaveAsync();

                return new ResponseDTO
                {
                    StatusCode = 200,
                    Message = "Lịch trình tàu đã được hủy thành công.",
                    IsSuccess = true
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO
                {
                    StatusCode = 500,
                    Message = "Đã xảy ra lỗi khi hủy lịch trình tàu: " + ex.Message,
                    IsSuccess = false
                };
            }
        }

        public async Task<ResponseDTO> GetTrainSchedules(Guid trainScheduleId)
        {
            try
            {
                var trainSchedule = await _unitOfWork.TrainScheduleRepository.GetByIdAsync(trainScheduleId);
                if (trainSchedule is null)
                {
                    return new ResponseDTO
                    {
                        StatusCode = 404,
                        Message = "Lịch trình tàu không tồn tại.",
                        IsSuccess = false
                    };
                }

                var getTrainSchedule = _mapper.Map<GetTrainScheduleDTO>(trainSchedule);

                return new ResponseDTO
                {
                    StatusCode = 200,
                    Message = "Lịch trình tàu đã được lấy thành công.",
                    IsSuccess = true,
                    Result = getTrainSchedule
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO
                {
                    StatusCode = 500,
                    Message = "Đã xảy ra lỗi khi lấy lịch trình tàu: " + ex.Message,
                    IsSuccess = false
                };
            }
        }

        public async Task<ResponseDTO> UpdateTrainSchedule(UpdateTrainScheduleDTO updateTrainScheduleDTO)
        {
            try
            {
                var trainSchedule = await _unitOfWork.TrainScheduleRepository.GetByIdAsync(updateTrainScheduleDTO.Id);
                if (trainSchedule is null)
                {
                    return new ResponseDTO
                    {
                        StatusCode = 404,
                        Message = "Lịch trình tàu không tồn tại.",
                        IsSuccess = false
                    };
                }

                var isExistTime = await _unitOfWork.TrainScheduleRepository
                    .IsExistTrainSchedule(updateTrainScheduleDTO.MetroLineId, updateTrainScheduleDTO.StationId, updateTrainScheduleDTO.StartTime, updateTrainScheduleDTO.Direction);
                if (isExistTime)
                {
                    return new ResponseDTO
                    {
                        StatusCode = 409,
                        Message = "Lịch trình đã tồn tại cho tuyến metro này, ga này, thời gian bắt đầu và hướng di chuyển.",
                        IsSuccess = false
                    };
                }

                var hasChanged = true;

                if (updateTrainScheduleDTO.MetroLineId.HasValue && trainSchedule.MetroLineId != updateTrainScheduleDTO.MetroLineId)
                {
                    trainSchedule.MetroLineId = updateTrainScheduleDTO.MetroLineId.Value;
                    hasChanged = false;
                }

                if (updateTrainScheduleDTO.StationId.HasValue && trainSchedule.StationId != updateTrainScheduleDTO.StationId)
                {
                    trainSchedule.StationId = updateTrainScheduleDTO.StationId.Value;
                    hasChanged = false;
                }

                if (updateTrainScheduleDTO.StartTime.HasValue && trainSchedule.StartTime != updateTrainScheduleDTO.StartTime)
                {
                    trainSchedule.StartTime = updateTrainScheduleDTO.StartTime.Value;
                    hasChanged = false;
                }

                if (updateTrainScheduleDTO.Direction.HasValue && trainSchedule.Direction != updateTrainScheduleDTO.Direction)
                {
                    trainSchedule.Direction = updateTrainScheduleDTO.Direction.Value;
                    hasChanged = false;
                }

                if (updateTrainScheduleDTO.Status.HasValue && trainSchedule.Status != updateTrainScheduleDTO.Status)
                {
                    trainSchedule.Status = updateTrainScheduleDTO.Status.Value;
                    hasChanged = false;
                }

                if (hasChanged)
                {
                    return new ResponseDTO
                    {
                        StatusCode = 200,
                        Message = "Không có thay đổi nào được thực hiện.",
                        IsSuccess = true
                    };
                }

                _unitOfWork.TrainScheduleRepository.Update(trainSchedule);
                await _unitOfWork.SaveAsync();

                var updatedTrainSchedule = _mapper.Map<GetTrainScheduleDTO>(trainSchedule);

                return new ResponseDTO
                {
                    StatusCode = 200,
                    Message = "Lịch trình tàu đã được cập nhật thành công.",
                    IsSuccess = true,
                    Result = updatedTrainSchedule
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO
                {
                    StatusCode = 500,
                    Message = "Đã xảy ra lỗi khi cập nhật lịch trình tàu: " + ex.Message,
                    IsSuccess = false
                };
            }
        }

        public async Task<ResponseDTO> GetAll(string? filterOn, string? filterQuery, string? sortBy, bool? isAcsending, int pageNumber, int pageSize)
        {
            try
            {
                var trainSchedules = (await _unitOfWork.TrainScheduleRepository.GetAllAsync(includeProperties: "MetroLine,Station"))
                    .Where(ts => ts.Status == TrainScheduleStatus.Normal || ts.Status == TrainScheduleStatus.OutOfService);

                if (!string.IsNullOrEmpty(filterOn) && !string.IsNullOrEmpty(filterQuery))
                {
                    string filter = filterOn.Trim().ToLower();
                    string query = filterQuery.Trim().ToLower();

                    trainSchedules = filter switch
                    {
                        "metroid" => trainSchedules.Where(ts => ts.MetroLineId == Guid.Parse(query)),
                        "stationid" => trainSchedules.Where(ts => ts.StationId == Guid.Parse(query)),

                        _ => trainSchedules
                    };
                }

                if (!string.IsNullOrEmpty(sortBy))
                {
                    trainSchedules = sortBy.Trim().ToLower() switch
                    {
                        "metrolinename" => isAcsending is true
                            ? trainSchedules.OrderBy(ts => ts.MetroLine.MetroName)
                            : trainSchedules.OrderByDescending(ts => ts.MetroLine.MetroName),
                        "stationname" => isAcsending is true
                            ? trainSchedules.OrderBy(ts => ts.Station.Name)
                            : trainSchedules.OrderByDescending(ts => ts.Station.Name),
                        "starttime" => isAcsending is true
                            ? trainSchedules.OrderBy(ts => ts.StartTime)
                            : trainSchedules.OrderByDescending(ts => ts.StartTime),


                        _ => trainSchedules
                    };
                }

                if (pageNumber <= 0 || pageSize <= 0)
                {
                    return new ResponseDTO
                    {
                        StatusCode = 400,
                        Message = "Số trang và kích thước trang phải lớn hơn 0.",
                        IsSuccess = false
                    };
                }
                else
                {
                    trainSchedules = trainSchedules.Skip((pageNumber - 1) * pageSize).Take(pageSize);
                }

                if (!trainSchedules.Any() || trainSchedules is null)
                {
                    return new ResponseDTO
                    {
                        StatusCode = 404,
                        Message = "Không tìm thấy lịch trình tàu nào.",
                        IsSuccess = false
                    };
                }

                var trainScheduleDTOs = _mapper.Map<List<GetTrainScheduleDTO>>(trainSchedules);

                return new ResponseDTO
                {
                    StatusCode = 200,
                    Message = "Danh sách lịch trình tàu đã được lấy thành công.",
                    IsSuccess = true,
                    Result = trainScheduleDTOs
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO
                {
                    StatusCode = 500,
                    Message = "Đã xảy ra lỗi khi lấy danh sách lịch trình tàu: " + ex.Message,
                    IsSuccess = false
                };
            }
        }
    }
}