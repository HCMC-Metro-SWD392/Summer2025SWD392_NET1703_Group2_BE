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

                var isExistOrderNumber = await _unitOfWork.MetroLineStationRepository.IsExistByOrderNumer(createMetroLineStationDTO.StationOder);
                if (isExistOrderNumber is true)
                {
                    return new ResponseDTO
                    {
                        StatusCode = 400,
                        Message = "Trạm metro đã tồn tại trong tuyến này với thứ tự này",
                        IsSuccess = false
                    };
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

        public async Task<ResponseDTO> GetStationByMetroLineIdAsync(Guid metroLineId)
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

                var station = await _unitOfWork.MetroLineStationRepository.GetStationByMetroLineIdAsync(metroLineId);
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
    }
}
