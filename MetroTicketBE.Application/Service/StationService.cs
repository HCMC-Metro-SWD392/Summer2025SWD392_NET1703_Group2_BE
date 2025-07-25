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

        public async Task<ResponseDTO> SearchTicketRoad(Guid stationStartId, Guid stationEndId)
        {
            try
            {
                var isActiveMetro = true;
                var allMetroline = await _unitOfWork.MetroLineRepository.GetAllListAsync(isActiveMetro);

                var _graph = new StationGraph(allMetroline);

                var stationPath = _graph.FindShortestPath(stationStartId, stationEndId);

                if (stationPath == null || stationPath.Count == 0)
                {
                    return new ResponseDTO
                    {
                        IsSuccess = false,
                        StatusCode = 404,
                        Message = "Không tìm thấy đường đi giữa hai trạm"
                    };
                }

                var ticketRoad = new List<object>();
                int i = 0;

                while (i < stationPath.Count - 1)
                {
                    var currentStationId = stationPath[i];
                    var nextStationId = stationPath[i + 1];

                    var metro = allMetroline.FirstOrDefault(l =>
                        l.MetroLineStations.Any(s => s.StationId == currentStationId) &&
                        l.MetroLineStations.Any(s => s.StationId == nextStationId));

                    if (metro != null)
                    {
                        var segmentStations = new List<object>();
                        segmentStations.Add(new
                        {
                            stationId = currentStationId,
                            stationName = metro.MetroLineStations
                                .First(s => s.StationId == currentStationId).Station.Name
                        });

                        while (i < stationPath.Count - 1 && metro.MetroLineStations.Any(s => s.StationId == stationPath[i + 1]))
                        {
                            i++;
                            var stationId = stationPath[i];
                            segmentStations.Add(new
                            {
                                stationId = stationId,
                                stationName = metro.MetroLineStations
                                    .First(s => s.StationId == stationId).Station.Name
                            });
                        }

                        ticketRoad.Add(new
                        {
                            id = metro.Id,
                            metroName = metro.MetroName,
                            stations = segmentStations,
                            status = metro.Status
                        });
                    }
                    else
                    {
                        i++; // Nếu không tìm thấy meto thi bỏ qua
                    }
                }


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

        public async Task<ResponseDTO> SearchTicketRoadV2(Guid ticketId)
        {
            try
            {

                var ticket = await _unitOfWork.TicketRepository.GetByIdAsync(ticketId);
                if (ticket is null)
                {
                    return new ResponseDTO
                    {
                        IsSuccess = false,
                        StatusCode = 404,
                        Message = "Vé không tồn tại"
                    };
                }

                if (ticket.SubscriptionTicket is null && ticket.TicketRoute is not null)
                {
                    return await SearchTicketRoad(ticket.TicketRoute.StartStationId, ticket.TicketRoute.EndStationId);
                }
                else if (ticket.SubscriptionTicket is not null && ticket.TicketRoute is null)
                {
                    return await SearchTicketRoad(ticket.SubscriptionTicket.StartStationId, ticket.SubscriptionTicket.EndStationId);
                }
                else if (ticket.SubscriptionTicket is not null && ticket.TicketRoute is not null)
                {
                    if (ticket.TicketRoute.EndStationId == ticket.SubscriptionTicket.StartStationId)
                    {
                        return await SearchTicketRoad(ticket.TicketRoute.StartStationId, ticket.SubscriptionTicket.EndStationId);
                    }
                    else
                    {
                        return await SearchTicketRoad(ticket.TicketRoute.EndStationId, ticket.SubscriptionTicket.StartStationId);
                    }
                }
                else
                {
                    return new ResponseDTO
                    {
                        IsSuccess = false,
                        StatusCode = 400,
                        Message = "Lỗi vé bị thiếu thông tin"
                    };
                }
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
