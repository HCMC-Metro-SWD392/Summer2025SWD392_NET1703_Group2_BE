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
        
        private const int TravelTimeBetweenStationsInSeconds = 180;
        private const int DwellTimeAtStationInSeconds = 45;
        private readonly (TimeSpan Start, TimeSpan End) _peakHour1 = (new TimeSpan(7, 0, 0), new TimeSpan(9, 0, 0));
        private readonly (TimeSpan Start, TimeSpan End) _peakHour2 = (new TimeSpan(17, 0, 0), new TimeSpan(19, 0, 0));
        private const int PeakHourHeadwayInSeconds = 300;
        private const int OffPeakHourHeadwayInSeconds = 600;
        public TrainScheduleService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<ResponseDTO> GenerateScheduleForMetroLine(Guid metroLineId)
        {
            try
            {
                if (metroLineId == Guid.Empty)
                {
                    return new ResponseDTO()
                    {
                        StatusCode = 400,
                        Message = "ID tuyến metro không hợp lệ.",
                        IsSuccess = false
                    };
                }
                var allSchedules = new List<TrainSchedule>();
                var orderedStations =
                    await _unitOfWork.MetroLineStationRepository.GetStationByMetroLineIdAsync(metroLineId);
                var metroLine = await _unitOfWork.MetroLineRepository.GetByIdAsync(metroLineId);

                var forwardSchedules =
                    await GenerateDirectionSchedules(metroLineId, orderedStations, TrainScheduleType.Forward);
                allSchedules.AddRange(forwardSchedules);

                int singleTripDurationInSeconds = (orderedStations.Count - 1) *
                                                  (TravelTimeBetweenStationsInSeconds + DwellTimeAtStationInSeconds);
                int turnAroundTimeInSeconds = 300; // Giả sử thời gian quay đầu là 5 phút

                TimeSpan backwardStartTime = metroLine.StartTime
                                             + TimeSpan.FromSeconds(singleTripDurationInSeconds +
                                                                    turnAroundTimeInSeconds);
                var backwardStations = new List<Station>(orderedStations);
                backwardStations.Reverse();
                var backwardSchedules =
                    await GenerateDirectionSchedules(metroLineId, backwardStations, TrainScheduleType.Backward, backwardStartTime);
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

        private async Task<List<TrainSchedule>> GenerateDirectionSchedules(Guid metroLineId, List<Station> orderedStations,
            TrainScheduleType direction, TimeSpan? currentStartTime = null)
        {
            var directionSchedules = new List<TrainSchedule>();
            // Sử dụng await thay vì .Result để tránh deadlock
            var metroLine = await _unitOfWork.MetroLineRepository.GetByIdAsync(metroLineId);
            if (metroLine is null)
            {
                throw new ArgumentException("Tuyến metro không tồn tại.");
            }
    
            // FIX 1: Tính toán tổng thời gian của một chuyến đi
            int singleTripDurationInSeconds = (orderedStations.Count - 1) * (TravelTimeBetweenStationsInSeconds + DwellTimeAtStationInSeconds);
            TimeSpan tripDuration = TimeSpan.FromSeconds(singleTripDurationInSeconds);

            var currentTime = metroLine.StartTime;
            // FIX 1: Điều kiện dừng mới: thời gian khởi hành cộng với thời gian chuyến đi phải nhỏ hơn hoặc bằng thời gian kết thúc
            var lastPossibleDepartureTime = metroLine.EndTime - tripDuration;

            while (currentTime <= lastPossibleDepartureTime)
            {
                var tripTime = currentTime;
                for (int i = 0; i < orderedStations.Count; i++)
                {
                    if (i > 0)
                    {
                        tripTime = tripTime.Add(TimeSpan.FromSeconds(TravelTimeBetweenStationsInSeconds + DwellTimeAtStationInSeconds));
                    }

                    directionSchedules.Add(new TrainSchedule()
                    {
                        MetroLineId = metroLineId,
                        StationId = orderedStations[i].Id,
                        StartTime = tripTime,
                        Direction = direction,
                        Status = TrainScheduleStatus.Normal
                    });
                }

                bool isPeakHour = (currentTime >= _peakHour1.Start && currentTime < _peakHour1.End) ||
                                  (currentTime >= _peakHour2.Start && currentTime < _peakHour2.End);
                int headwayInSeconds = isPeakHour ? PeakHourHeadwayInSeconds : OffPeakHourHeadwayInSeconds;
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