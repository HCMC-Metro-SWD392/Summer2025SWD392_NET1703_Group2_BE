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
        
        // private const int TravelTimeBetweenStationsInSeconds = 180;
        // private const int DwellTimeAtStationInSeconds = 30;
        // private readonly (TimeSpan Start, TimeSpan End) _peakHour1 = (new TimeSpan(7, 0, 0), new TimeSpan(9, 0, 0));
        // private readonly (TimeSpan Start, TimeSpan End) _peakHour2 = (new TimeSpan(17, 0, 0), new TimeSpan(19, 0, 0));
        // private const int PeakHourHeadwayInSeconds = 300;
        // private const int OffPeakHourHeadwayInSeconds = 600;
        public TrainScheduleService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<ResponseDTO> GenerateScheduleForMetroLine(CreateTrainScheduleDTO dto)
        {
            try
            {
                if (dto.MetroLineId == Guid.Empty)
                {
                    return new ResponseDTO()
                    {
                        StatusCode = 400,
                        Message = "ID tuyến metro không hợp lệ.",
                        IsSuccess = false
                    };
                }
                var existedSchedules = await _unitOfWork.TrainScheduleRepository.GetByMetroLineIdSortedAsync(dto.MetroLineId);
                if (existedSchedules.Any())
                {
                    _unitOfWork.TrainScheduleRepository.RemoveRange(existedSchedules);
                }
                var allSchedules = new List<TrainSchedule>();
                var orderedStations =
                    await _unitOfWork.MetroLineStationRepository.GetStationByMetroLineIdAsync(dto.MetroLineId, true);
                var metroLine = await _unitOfWork.MetroLineRepository.GetByIdAsync(dto.MetroLineId);
                if (metroLine is null)
                {
                    return new ResponseDTO()
                    {
                        StatusCode = 404,
                        Message = "Tuyến metro không tồn tại.",
                        IsSuccess = false
                    };
                }
                var forwardSchedules =
                    await GenerateDirectionSchedules(metroLine, orderedStations, TrainScheduleType.Forward, dto.TravelTimeBetweenStationsInSeconds, dto.DwellTimeAtStationInSeconds, dto.PeakHourMorningStart, dto.PeakHourMorningEnd, dto.PeakHourEveningStart, dto.PeakHourEveningEnd, dto.PeakHourHeadwayInSeconds, dto.OffPeakHourHeadwayInSeconds, metroLine.StartTime);
                allSchedules.AddRange(forwardSchedules);
                
                var backwardStations = new List<Station>(orderedStations);
                backwardStations.Reverse();
                var backwardSchedules =
                    await GenerateDirectionSchedules(metroLine, backwardStations, TrainScheduleType.Backward, dto.TravelTimeBetweenStationsInSeconds, dto.DwellTimeAtStationInSeconds, dto.PeakHourMorningStart, dto.PeakHourMorningEnd, dto.PeakHourEveningStart, dto.PeakHourEveningEnd, dto.PeakHourHeadwayInSeconds, dto.OffPeakHourHeadwayInSeconds, metroLine.StartTime);
                allSchedules.AddRange(backwardSchedules);
                await _unitOfWork.TrainScheduleRepository.AddRangeAsync(allSchedules);
                await _unitOfWork.SaveAsync();

                return new ResponseDTO()
                {
                    StatusCode = 200,
                    Message = "Lịch trình cho tuyến metro đã được tạo thành công.",
                    IsSuccess = true,
                    Result = null,
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO()
                {
                    StatusCode = 500,
                    Message = "Đã xảy ra lỗi khi tạo lịch trình cho tuyến metro: " + ex.Message,
                    IsSuccess = false
                };
            }
        }

        private async Task<List<TrainSchedule>> GenerateDirectionSchedules(MetroLine metroLine,
            List<Station> orderedStations,
            TrainScheduleType direction,
            int travelTimeBetweenStationsInSeconds,
            int dwellTimeAtStationInSeconds,
            TimeSpan peakHourMorningStart,
            TimeSpan peakHourMorningEnd,
            TimeSpan peakHourEveningStart,
            TimeSpan peakHourEveningEnd,
            int peakHourHeadwayInSeconds,
            int offPeakHourHeadwayInSeconds,
            TimeSpan? currentStartTime = null)
        {
            var directionSchedules = new List<TrainSchedule>();
            if (metroLine is null)
            {
                throw new ArgumentException("Tuyến metro không tồn tại.");
            }
    
            int stationCount = orderedStations.Count;
            int singleTripDurationInSeconds = 0;
            if (stationCount > 1)
            {
                singleTripDurationInSeconds = ((stationCount - 1) * travelTimeBetweenStationsInSeconds) + 
                                              ((stationCount - 2) * dwellTimeAtStationInSeconds);
            }
            TimeSpan tripDuration = TimeSpan.FromSeconds(singleTripDurationInSeconds);

            var initialTime = currentStartTime ?? metroLine.StartTime;
            
            var currentTime = initialTime;
            var lastPossibleDepartureTime = metroLine.EndTime - tripDuration;

            while (currentTime <= lastPossibleDepartureTime)
            {
                var tripTime = currentTime;
                for (int i = 0; i < orderedStations.Count; i++)
                {
                    if (i > 0)
                    {
                        tripTime = tripTime.Add(TimeSpan.FromSeconds(travelTimeBetweenStationsInSeconds + dwellTimeAtStationInSeconds));
                    }

                    directionSchedules.Add(new TrainSchedule()
                    {
                        MetroLineId = metroLine.Id,
                        StationId = orderedStations[i].Id,
                        StartTime = tripTime,
                        Direction = direction,
                        Status = TrainScheduleStatus.Normal
                    });
                }

                bool isPeakHour = (currentTime >= peakHourMorningStart && currentTime < peakHourMorningEnd) ||
                                  (currentTime >= peakHourEveningStart && currentTime < peakHourEveningEnd);
                int headwayInSeconds = isPeakHour ? peakHourHeadwayInSeconds : offPeakHourHeadwayInSeconds;
                currentTime = currentTime.Add(TimeSpan.FromSeconds(headwayInSeconds));
            }
            return directionSchedules;
        }

        public async Task<ResponseDTO> GetTrainSchedulesByStationId(Guid stationId,  TrainScheduleType? direction)
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
            var schedules = await _unitOfWork.TrainScheduleRepository.GetByStationIdSortedAsync(stationId, direction);
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