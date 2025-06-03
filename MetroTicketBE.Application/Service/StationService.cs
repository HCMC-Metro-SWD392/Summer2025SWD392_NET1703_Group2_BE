using MetroTicketBE.Application.IService;
using MetroTicketBE.Domain.DTO.Auth;
using MetroTicketBE.Domain.DTO.Station;
using MetroTicketBE.Domain.Entities;
using MetroTicketBE.Infrastructure.IRepository;

namespace MetroTicketBE.Application.Service
{
    public class StationService : IStationService
    {
        private readonly IUnitOfWork _unitOfWork;

        public StationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<ResponseDTO> CreateStation(CreateStationDTO createStationDTO)
        {
            try
            {
                var isExistStation = await _unitOfWork.StationRepository.IsExistByName(createStationDTO.Name);

                if (isExistStation is true)
                {
                    return new ResponseDTO
                    {
                        IsSuccess = false,
                        StatusCode = 400,
                        Message = "Trạm Metro đã tồn tại"
                    };
                }

                Station station = new Station
                {
                    Name = createStationDTO.Name,
                    Address = createStationDTO.Address,
                    Description = createStationDTO.Description
                };

                await _unitOfWork.StationRepository.AddAsync(station);
                await _unitOfWork.SaveAsync();

                return new ResponseDTO
                {
                    IsSuccess = true,
                    StatusCode = 200,
                    Message = "Tạo trạm Metro thành công",
                    Result = station
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO
                {
                    IsSuccess = false,
                    StatusCode = 500,
                    Message = "Lỗi khi tạo trạm Metro: " + ex.Message
                };
            }
        }

        public async Task<ResponseDTO> UpdateStation(Guid stationId, UpdateStationDTO updateStationDTO)
        {
            try
            {
                if (stationId == Guid.Empty)
                {
                    return new ResponseDTO
                    {
                        IsSuccess = false,
                        StatusCode = 400,
                        Message = "ID trạm không hợp lệ"
                    };
                }

                var station = await _unitOfWork.StationRepository.GetAsync(s => s.Id == stationId);
                if (station == null)
                {
                    return new ResponseDTO
                    {
                        IsSuccess = false,
                        StatusCode = 404,
                        Message = "Trạm Metro không tồn tại"
                    };
                }

                if (!string.IsNullOrEmpty(updateStationDTO.Name))
                {
                    var isExistName = await _unitOfWork.StationRepository.IsExistByName(updateStationDTO.Name);
                    if (isExistName)
                    {
                        return new ResponseDTO
                        {
                            IsSuccess = false,
                            StatusCode = 400,
                            Message = "Tên trạm đã tồn tại"
                        };
                    }

                    station.Name = updateStationDTO.Name;
                }

                if (!string.IsNullOrEmpty(updateStationDTO.Address))
                {
                    var isExistAddress = await _unitOfWork.StationRepository.IsExistByAddress(updateStationDTO.Address);
                    if (isExistAddress)
                    {
                        return new ResponseDTO
                        {
                            IsSuccess = false,
                            StatusCode = 400,
                            Message = "Địa chỉ trạm đã tồn tại"
                        };
                    }

                    station.Address = updateStationDTO.Address;
                }

                if (!string.IsNullOrEmpty(updateStationDTO.Description))
                {
                    station.Description = updateStationDTO.Description;
                }

                _unitOfWork.StationRepository.Update(station);
                await _unitOfWork.SaveAsync();
                return new ResponseDTO
                {
                    IsSuccess = true,
                    StatusCode = 200,
                    Message = "Cập nhật trạm Metro thành công",
                    Result = station
                };
            }
            catch (Exception exception)
            {
                return new ResponseDTO()
                {
                    IsSuccess = false,
                    StatusCode = 500,
                    Message = "Lỗi khi cập nhật trạm Metro: " + exception.Message
                };
            }
            
        }
        public async Task<ResponseDTO> GetAllStations()
        {
            try
            {
                var stations = await _unitOfWork.StationRepository.GetAllAsync();

                return new ResponseDTO
                {
                    IsSuccess = true,
                    StatusCode = 200,
                    Message = "Lấy danh sách trạm Metro thành công",
                    Result = stations
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO
                {
                    IsSuccess = false,
                    StatusCode = 500,
                    Message = "Lỗi khi lấy danh sách trạm Metro: " + ex.Message
                };
            }
        }
}
}
