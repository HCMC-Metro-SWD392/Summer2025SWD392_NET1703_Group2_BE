using MetroTicketBE.Application.IService;
using MetroTicketBE.Domain.DTO.Auth;
using MetroTicketBE.Domain.DTO.MetroLineStation;
using MetroTicketBE.Domain.Entities;
using MetroTicketBE.Infrastructure.IRepository;

namespace MetroTicketBE.Application.Service
{
    public class MetroLineStationService : IMetroLineStationService
    {
        private readonly IUnitOfWork _unitOfWork;
        public MetroLineStationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }
        public async Task<ResponseDTO> CreateMetroLineStation(CreateMetroLineStationDTO createMetroLineStationDTO)
        {
            try
            {
                var isExistMetroLine = await _unitOfWork.MetroLineRepository.IsExistById(createMetroLineStationDTO.MetroLineId);
                if (isExistMetroLine is false)
                {
                    return new ResponseDTO
                    {
                        StatusCode = 404,
                        Message = "Metro line không tồn tại",
                        IsSuccess = false
                    };
                }

                var isExistStation = await _unitOfWork.StationRepository.IsExistById(createMetroLineStationDTO.StationId);
                if (isExistStation is false)
                {
                    return new ResponseDTO
                    {
                        StatusCode = 404,
                        Message = "Trạm metro không tồn tại",
                        IsSuccess = false
                    };
                }
                
                var isStationAlreadyInLine = await _unitOfWork.MetroLineStationRepository.GetAsync(mls => mls.MetroLineId == createMetroLineStationDTO.MetroLineId && mls.StationId == createMetroLineStationDTO.StationId);
                if (isStationAlreadyInLine is not null)
                {
                    return new ResponseDTO
                    {
                        StatusCode = 400,
                        Message = "Trạm metro đã tồn tại trong tuyến này",
                        IsSuccess = false
                    };
                }
                var stationsToUpdate = await _unitOfWork.MetroLineStationRepository.GetAllAsync(filter: s =>
                    s.MetroLineId == createMetroLineStationDTO.MetroLineId && s.StationOrder >= createMetroLineStationDTO.StationOder && s.IsActive,
                    orderBy: q => q.OrderBy(s => s.StationOrder));

                foreach (var station in stationsToUpdate)
                {
                    station.StationOrder++;
                    _unitOfWork.MetroLineStationRepository.Update(station);
                }
                
                MetroLineStation metroLineStation = new MetroLineStation
                {
                    MetroLineId = createMetroLineStationDTO.MetroLineId,
                    StationId = createMetroLineStationDTO.StationId,
                    DistanceFromStart = createMetroLineStationDTO.DistanceFromStart,
                    StationOrder = createMetroLineStationDTO.StationOder
                };

                await _unitOfWork.MetroLineStationRepository.AddAsync(metroLineStation);
                await _unitOfWork.SaveAsync();

                return new ResponseDTO
                {
                    StatusCode = 201,
                    Message = "Tạo trạm metro thành công",
                    IsSuccess = true,
                    Result = metroLineStation
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO
                {
                    StatusCode = 500,
                    Message = $"Lỗi MetroLineStationService: {ex.Message}",
                    IsSuccess = false
                };
            }
        }

        public async Task<ResponseDTO> GetStationByMetroLineIdAsync(Guid metroLineId, bool? isActive = null)
        {
            try
            {
                if (metroLineId == Guid.Empty)
                {
                    return new ResponseDTO
                    {
                        StatusCode = 400,
                        Message = "ID tuyến metro không hợp lệ",
                        IsSuccess = false
                    };
                }

                var station = await _unitOfWork.MetroLineStationRepository.GetStationByMetroLineIdAsync(metroLineId, isActive);
                if (station == null)
                {
                    return new ResponseDTO
                    {
                        StatusCode = 404,
                        Message = "Không tìm thấy trạm metro cho tuyến này",
                        IsSuccess = false
                    };
                }

                return new ResponseDTO
                {
                    StatusCode = 200,
                    Message = "Lấy trạm metro thành công",
                    IsSuccess = true,
                    Result = station
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO
                {
                    StatusCode = 500,
                    Message = $"Lỗi MetroLineStationService: {ex.Message}",
                    IsSuccess = false
                };
            }
        }

        public async Task<ResponseDTO> UpdateMetroLineStation(Guid id, UpdateMetroLineStationDTO updateDTO)
        {
            try
            {
                // Kiểm tra xem có thông tin gì để cập nhật không
                if (!updateDTO.StationOrder.HasValue && !updateDTO.DistanceFromStart.HasValue)
                {
                    return new ResponseDTO { StatusCode = 400, Message = "Không có thông tin để cập nhật.", IsSuccess = false };
                }

                var stationToUpdate = await _unitOfWork.MetroLineStationRepository.GetAsync(mls => mls.Id == id);
                if (stationToUpdate == null)
                {
                    return new ResponseDTO { StatusCode = 404, Message = "MetroLineStation không tồn tại.", IsSuccess = false };
                }

                var metroLineId = stationToUpdate.MetroLineId;

                if (updateDTO.DistanceFromStart.HasValue)
                {
                    double newDistance = updateDTO.DistanceFromStart.Value;
                    if (newDistance < 0)
                    {
                        return new ResponseDTO { StatusCode = 400, Message = "Khoảng cách không thể là số âm.", IsSuccess = false };
                    }

                    int finalOrder = updateDTO.StationOrder ?? stationToUpdate.StationOrder;

                    var prevStation = await _unitOfWork.MetroLineStationRepository.GetAsync(s => s.MetroLineId == metroLineId && s.StationOrder == finalOrder - 1);
                    var nextStation = await _unitOfWork.MetroLineStationRepository.GetAsync(s => s.MetroLineId == metroLineId && s.StationOrder == finalOrder + 1);

                    if (prevStation != null && newDistance <= prevStation.DistanceFromStart)
                    {
                        return new ResponseDTO
                        {
                            StatusCode = 400,
                            Message = $"Khoảng cách ({newDistance}m) phải lớn hơn khoảng cách của trạm trước đó ({prevStation.DistanceFromStart}m).",
                            IsSuccess = false
                        };
                    }

                    if (nextStation != null && newDistance >= nextStation.DistanceFromStart)
                    {
                        return new ResponseDTO { StatusCode = 400, Message = $"Khoảng cách ({newDistance}m) phải nhỏ hơn khoảng cách của trạm kế tiếp ({nextStation.DistanceFromStart}m).", IsSuccess = false };
                    }
                }

                if (updateDTO.StationOrder.HasValue && updateDTO.StationOrder.Value != stationToUpdate.StationOrder)
                {
                    int oldOrder = stationToUpdate.StationOrder;
                    int newOrder = updateDTO.StationOrder.Value;

                    if (newOrder > oldOrder)
                    {
                        var stationsToShift = await _unitOfWork.MetroLineStationRepository.GetAllAsync(s => s.MetroLineId == metroLineId && s.StationOrder > oldOrder && s.StationOrder <= newOrder);
                        foreach (var s in stationsToShift) s.StationOrder--;
                    }
                    else
                    {
                        var stationsToShift = await _unitOfWork.MetroLineStationRepository.GetAllAsync(s => s.MetroLineId == metroLineId && s.StationOrder >= newOrder && s.StationOrder < oldOrder);
                        foreach (var s in stationsToShift) s.StationOrder++;
                    }
                    stationToUpdate.StationOrder = newOrder;
                }

                if (updateDTO.DistanceFromStart.HasValue)
                {
                    stationToUpdate.DistanceFromStart = updateDTO.DistanceFromStart.Value;
                }

                _unitOfWork.MetroLineStationRepository.Update(stationToUpdate);
                await _unitOfWork.SaveAsync();

                return new ResponseDTO
                {
                    StatusCode = 200,
                    Message = "Cập nhật trạm metro thành công",
                    IsSuccess = true,
                    Result = stationToUpdate
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO { StatusCode = 500, Message = $"Lỗi khi cập nhật: {ex.Message}", IsSuccess = false };
            }
        }
        
        public async Task<ResponseDTO> RemoveMetroLineStation(Guid metroLineStationId)
        {
            try
            {
                var metroLineStation = await _unitOfWork.MetroLineStationRepository.GetAsync(mts => mts.Id == metroLineStationId);
                if (metroLineStation == null)
                {
                    return new ResponseDTO
                    {
                        StatusCode = 404,
                        Message = "Trạm metro không tồn tại",
                        IsSuccess = false
                    };
                }

                var stationsToReOrder = await _unitOfWork.MetroLineStationRepository.GetAllAsync(filter: s =>
                    s.MetroLineId == metroLineStation.MetroLineId && s.Id != metroLineStationId && s.IsActive);
                var stationsAfter = stationsToReOrder.Where(s => s.StationOrder > metroLineStation.StationOrder).ToList();
                foreach (var station in stationsAfter)
                {
                    station.StationOrder--;
                    _unitOfWork.MetroLineStationRepository.Update(station);
                }

                metroLineStation.IsActive = false;
                _unitOfWork.MetroLineStationRepository.Update(metroLineStation);
                await _unitOfWork.SaveAsync();

                return new ResponseDTO
                {
                    StatusCode = 200,
                    Message = "Cập nhật trạng thái trạm metro thành công",
                    IsSuccess = true,
                    Result = metroLineStation
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO
                {
                    StatusCode = 500,
                    Message = $"Lỗi MetroLineStationService: {ex.Message}",
                    IsSuccess = false
                };
            }
        }
    }
}
