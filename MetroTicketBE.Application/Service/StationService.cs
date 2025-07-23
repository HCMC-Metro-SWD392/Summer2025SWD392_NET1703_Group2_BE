using System.Security.Claims;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MetroTicketBE.Application.IService;
using MetroTicketBE.Domain.DTO.Auth;
using MetroTicketBE.Domain.DTO.Station;
using MetroTicketBE.Domain.Entities;
using MetroTicketBE.Domain.Enum;
using MetroTicketBE.Infrastructure.IRepository;
using MetroTicketBE.WebAPI.Extentions;
using Microsoft.EntityFrameworkCore;

namespace MetroTicketBE.Application.Service
{
    public class StationService : IStationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogService _logService;
        private const string EntityName = "Trạm Metro";

        public StationService(IUnitOfWork unitOfWork, IMapper mapper, ILogService logService)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logService = logService ?? throw new ArgumentNullException(nameof(logService));
        }

        public async Task<ResponseDTO> CreateStation(ClaimsPrincipal user, CreateStationDTO createStationDTO)
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
                    Description = createStationDTO.Description,
                };

                await _unitOfWork.StationRepository.AddAsync(station);
                await _unitOfWork.SaveAsync();
                await _logService.AddLogAsync(LogType.Create, user.FindFirstValue(ClaimTypes.NameIdentifier), EntityName, station.Name);

                return new ResponseDTO
                {
                    IsSuccess = true,
                    StatusCode = 200,
                    Message = "Tạo trạm Metro thành công",
                    Result = _mapper.Map<GetStationDTO>(station)
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

        public async Task<ResponseDTO> UpdateStation(ClaimsPrincipal user, Guid stationId, UpdateStationDTO updateStationDTO)
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
                    var isExistName = await _unitOfWork.StationRepository.IsExistByNameAndStationId(updateStationDTO.Name, stationId);
                    if (isExistName)
                    {
                        return new ResponseDTO
                        {
                            IsSuccess = false,
                            StatusCode = 400,
                            Message = "Tên trạm đã tồn tại"
                        };
                    }

                    station.Name = updateStationDTO.Name.Trim();
                }

                if (!string.IsNullOrEmpty(updateStationDTO.Address))
                {
                    station.Address = updateStationDTO.Address.Trim();
                }

                if (!string.IsNullOrEmpty(updateStationDTO.Description))
                {
                    station.Description = updateStationDTO.Description;
                }

                _unitOfWork.StationRepository.Update(station);
                await _unitOfWork.SaveAsync();
                _logService.AddLogAsync(LogType.Update, user.FindFirstValue(ClaimTypes.NameIdentifier), EntityName, station.Name);
                return new ResponseDTO
                {
                    IsSuccess = true,
                    StatusCode = 200,
                    Message = "Cập nhật trạm Metro thành công",
                    Result = _mapper.Map<GetStationDTO>(station)
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
        public async Task<ResponseDTO> GetAllStations(bool? isAscending, int pageNumber,
            int pageSize, bool? isActive = null)
        {
            try
            {
                var stations = _unitOfWork.StationRepository.GetAllStationDTOAsync(isAscending, isActive);
                var stationDTO = await stations.ProjectTo<GetStationDTO>(_mapper.ConfigurationProvider).ToListAsync();
                if (pageNumber > 0 || pageSize > 0)
                {
                    stationDTO = stationDTO.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                }

                return new ResponseDTO
                {
                    IsSuccess = true,
                    StatusCode = 200,
                    Message = "Lấy danh sách trạm Metro thành công",
                    Result = stationDTO
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

        public async Task<ResponseDTO> GetStationById(Guid stationId)
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

                return new ResponseDTO
                {
                    IsSuccess = true,
                    StatusCode = 200,
                    Message = "Lấy thông tin trạm Metro thành công",
                    Result = station
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO
                {
                    IsSuccess = false,
                    StatusCode = 500,
                    Message = "Lỗi khi lấy thông tin trạm Metro: " + ex.Message
                };
            }
        }

        public async Task<ResponseDTO> SearchStationsByName(string? name, bool? isActive = null)
        {
            var stations = await _unitOfWork.StationRepository.SearchStationsByName(name, isActive);
            if (stations.Count == 0)
            {
                return new ResponseDTO
                {
                    IsSuccess = false,
                    StatusCode = 404,
                    Message = "Không tìm thấy trạm Metro nào với tên đã cho"
                };
            }
            return new ResponseDTO
            {
                IsSuccess = true,
                StatusCode = 200,
                Message = "Tìm kiếm trạm Metro thành công",
                Result = _mapper.Map<List<GetStationDTO>>(stations)
            };
        }

        public async Task<ResponseDTO> SetIsActiveStation(ClaimsPrincipal user, Guid stationId, bool isActive)
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

                station.IsActive = isActive;
                _unitOfWork.StationRepository.Update(station);
                await _unitOfWork.SaveAsync();
                await _logService.AddLogAsync(LogType.Update, user.FindFirstValue(ClaimTypes.NameIdentifier), EntityName, $"Trạng thái {station.Name}: {(isActive ? "hoạt động" : "Ngừng hoạt động")}");
                return new ResponseDTO
                {
                    IsSuccess = true,
                    StatusCode = 200,
                    Message = "Cập nhật trạng thái hoạt động của trạm Metro thành công",
                    Result = _mapper.Map<GetStationDTO>(station)
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO
                {
                    IsSuccess = false,
                    StatusCode = 500,
                    Message = "Lỗi khi cập nhật trạng thái hoạt động của trạm Metro: " + ex.Message
                };
            }
        }

        public async Task<ResponseDTO> SearchTicketRoad(Guid stationStart, Guid stationEnd)
        {
            try
            {
                var allMetroline = await _unitOfWork.MetroLineRepository.GetAllListAsync(true);

                var _graph = new StationGraph(allMetroline);

                var stationPath = _graph.FindShortestPath(stationStart, stationEnd);

                if (stationPath == null || stationPath.Count == 0)
                {
                    return new ResponseDTO
                    {
                        IsSuccess = false,
                        StatusCode = 404,
                        Message = "Không tìm thấy đường đi giữa hai trạm"
                    };
                }

                var ticketRoad = stationPath.Select(id =>
                    allMetroline.SelectMany(l => l.MetroLineStations)
                        .FirstOrDefault(s => s.StationId == id)?.Station.Name ?? id.ToString()).ToList();


                return new ResponseDTO
                {
                    IsSuccess = true,
                    StatusCode = 200,
                    Message = "Tìm kiếm đường đi thành công",
                    Result = ticketRoad
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO
                {
                    IsSuccess = false,
                    StatusCode = 500,
                    Message = "Lỗi khi tìm kiếm đường đi: " + ex.Message
                };
            }
        }
    }
}
