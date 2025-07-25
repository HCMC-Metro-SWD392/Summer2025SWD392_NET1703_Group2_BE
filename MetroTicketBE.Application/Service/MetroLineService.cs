﻿using MetroTicketBE.Application.IService;
using MetroTicketBE.Domain.DTO.Auth;
using MetroTicketBE.Domain.DTO.MetroLine;
using MetroTicketBE.Domain.Entities;
using MetroTicketBE.Infrastructure.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using MetroTicketBE.Domain.Enum;
using MetroTicketBE.Domain.Enums;
using MetroTicketBE.WebAPI.Extentions;

namespace MetroTicketBE.Application.Service
{
    public class MetroLineService : IMetroLineService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogService _logService;
        private readonly string EntityName = "Tuyến Metro";

        public MetroLineService(IUnitOfWork unitOfWork, IMapper mapper, ILogService logService)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logService = logService ?? throw new ArgumentNullException(nameof(logService));
        }

        public async Task<ResponseDTO> CreateMetroLine(ClaimsPrincipal user, CreateMetroLineDTO createMetroLineDTO)
        {
            try
            {
                var startStation = await _unitOfWork.StationRepository.IsExistById(createMetroLineDTO.StartStationId);
                if (startStation is false)
                {
                    return new ResponseDTO
                    {
                        StatusCode = 404,
                        Message = "Không tìm thấy trạm bắt đầu",
                        IsSuccess = false
                    };
                }

                var endStation = await _unitOfWork.StationRepository.IsExistById(createMetroLineDTO.EndStationId);
                if (endStation is false)
                {
                    return new ResponseDTO
                    {
                        StatusCode = 404,
                        Message = "Không tìm thấy trạm kết thúc ",
                        IsSuccess = false
                    };
                }

                var metroCount = (await _unitOfWork.MetroLineRepository.GetAllAsync()).Count().ToString();

                MetroLine metroLine = new MetroLine
                {
                    MetroLineNumber = metroCount,
                    MetroName = createMetroLineDTO.MetroName,
                    StartStationId = createMetroLineDTO.StartStationId,
                    EndStationId = createMetroLineDTO.EndStationId,
                    StartTime = createMetroLineDTO.StartTime,
                    EndTime = createMetroLineDTO.EndTime,
                };

                await _unitOfWork.MetroLineRepository.AddAsync(metroLine);
                await _unitOfWork.SaveAsync();
                await _logService.AddLogAsync(LogType.Create, user.FindFirstValue(ClaimTypes.NameIdentifier), EntityName, metroLine.MetroName);
                return new ResponseDTO
                {
                    Result = _mapper.Map<GetMetroLineDTO>(metroLine),
                    IsSuccess = true,
                    StatusCode = 201,
                    Message = "Tạo tuyến Metro thành công"
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO
                {
                    IsSuccess = false,
                    StatusCode = 500,
                    Message = "Lỗi khi tạo tuyến Metro: " + ex.InnerException?.Message ?? ex.Message
                };
            }
        }

        public async Task<ResponseDTO> GetAllMetroLines(bool? isActive = null)
        {
            try
            {
                var metroLines = await _unitOfWork.MetroLineRepository.GetAllListAsync(isActive);
                if (metroLines is null || !metroLines.Any())
                {
                    return new ResponseDTO
                    {
                        IsSuccess = false,
                        StatusCode = 404,
                        Message = "Không tìm thấy tuyến Metro nào"
                    };
                }

                var getMetroLines = _mapper.Map<List<GetMetroLineDTO>>(metroLines);
                return new ResponseDTO
                {
                    Message = "Danh sách tuyến Metro",
                    IsSuccess = true,
                    StatusCode = 200,
                    Result = getMetroLines
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO
                {
                    IsSuccess = false,
                    StatusCode = 500,
                    Message = "Lỗi khi lấy danh sách tuyến Metro: " + ex.InnerException?.Message ?? ex.Message
                };
            }
        }

        public async Task<ResponseDTO> GetMetroLineById(Guid metroLineId)
        {
            try
            {
                var metroLine = await _unitOfWork.MetroLineRepository.GetByIdAsync(metroLineId);
                if (metroLine is null)
                {
                    return new ResponseDTO
                    {
                        IsSuccess = false,
                        StatusCode = 404,
                        Message = "Không tìm thấy tuyến Metro"
                    };
                }

                var getMetroLine = _mapper.Map<GetMetroLineDTO>(metroLine);
                return new ResponseDTO
                {
                    IsSuccess = true,
                    StatusCode = 200,
                    Result = getMetroLine
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO
                {
                    IsSuccess = false,
                    StatusCode = 500,
                    Message = "Lỗi khi lấy thông tin tuyến Metro: " + ex.InnerException?.Message ?? ex.Message
                };
            }
        }

        public async Task<ResponseDTO> UpdateMetroLine(ClaimsPrincipal user, Guid metroLineId, UpdateMetroLineDTO updateMetroLineDTO)
        {
            var metroLine = await _unitOfWork.MetroLineRepository.GetByIdAsync(metroLineId);
            if (metroLine is null)
            {
                return new ResponseDTO
                {
                    IsSuccess = false,
                    StatusCode = 404,
                    Message = "Không tìm thấy tuyến Metro"
                };
            }
            try
            {
                var startStation = await _unitOfWork.StationRepository.IsExistById(updateMetroLineDTO.StartStationId ?? metroLine.StartStationId);
                if (startStation is false)
                {
                    return new ResponseDTO
                    {
                        StatusCode = 404,
                        Message = "Không tìm thấy trạm bắt đầu",
                        IsSuccess = false
                    };
                }

                var endStation = await _unitOfWork.StationRepository.IsExistById(updateMetroLineDTO.EndStationId ?? metroLine.EndStationId);
                if (endStation is false)
                {
                    return new ResponseDTO
                    {
                        StatusCode = 404,
                        Message = "Không tìm thấy trạm kết thúc ",
                        IsSuccess = false
                    };
                }

                var isExistMetroLine =
                    await _unitOfWork.MetroLineRepository.IsExistByMetroLineNumber(updateMetroLineDTO.MetroLineNumber ?? metroLine.MetroLineNumber, metroLineId);
                if (isExistMetroLine)
                {
                    return new ResponseDTO
                    {
                        IsSuccess = false,
                        StatusCode = 400,
                        Message = "Số tuyến Metro đã tồn tại"
                    };
                }
                PatchMetroLine(metroLine, updateMetroLineDTO);
                _unitOfWork.MetroLineRepository.Update(metroLine);
                await _unitOfWork.SaveAsync();
                await _logService.AddLogAsync(LogType.Update, user.FindFirstValue(ClaimTypes.NameIdentifier), EntityName, metroLine.MetroName);
                return new ResponseDTO()
                {
                    IsSuccess = true,
                    StatusCode = 200,
                    Message = "Cập nhật tuyến Metro thành công",
                    Result = _mapper.Map<GetMetroLineDTO>(metroLine)
                };

            }
            catch (Exception exception)
            {
                return new ResponseDTO()
                {
                    IsSuccess = false,
                    StatusCode = 500,
                    Message = "Lỗi khi cập nhật tuyến Metro: " + exception.Message
                };
            }
        }

        public async Task<ResponseDTO> SetIsActiveMetroLine(ClaimsPrincipal user, Guid metroLineId, bool isActive)
        {
            try
            {
                var metroLine = await _unitOfWork.MetroLineRepository.GetByIdAsync(metroLineId);
                if (metroLine is null)
                {
                    return new ResponseDTO
                    {
                        IsSuccess = false,
                        StatusCode = 404,
                        Message = "Không tìm thấy tuyến Metro"
                    };
                }
                metroLine.IsActive = isActive;
                await _unitOfWork.SaveAsync();
                await _logService.AddLogAsync(LogType.Update, user.FindFirstValue(ClaimTypes.NameIdentifier), EntityName, $"Trạng thái {metroLine.MetroName}: {(isActive ? "hoạt động" : "Ngừng hoạt động")}");
                return new ResponseDTO
                {
                    IsSuccess = true,
                    StatusCode = 200,
                    Message = "Cập nhật trạng thái tuyến Metro thành công",
                    Result = _mapper.Map<GetMetroLineDTO>(metroLine)
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO
                {
                    IsSuccess = false,
                    StatusCode = 500,
                    Message = "Lỗi khi cập nhật trạng thái tuyến Metro: " + ex.InnerException?.Message ?? ex.Message
                };
            }
        }

        private static void PatchMetroLine(MetroLine metroLine, UpdateMetroLineDTO updateMetroLineDTO)
        {
            if (!string.IsNullOrEmpty(updateMetroLineDTO.MetroLineNumber))
            {
                metroLine.MetroLineNumber = updateMetroLineDTO.MetroLineNumber;
            }
            if (!string.IsNullOrEmpty(updateMetroLineDTO.MetroName))
            {
                metroLine.MetroName = updateMetroLineDTO.MetroName;
            }
            if (updateMetroLineDTO.StartStationId.HasValue)
            {
                metroLine.StartStationId = updateMetroLineDTO.StartStationId.Value;
            }
            if (updateMetroLineDTO.EndStationId.HasValue)
            {
                metroLine.EndStationId = updateMetroLineDTO.EndStationId.Value;
            }
            if (updateMetroLineDTO.StartTime.HasValue)
            {
                metroLine.StartTime = updateMetroLineDTO.StartTime.Value;
            }

            if (updateMetroLineDTO.EndTime.HasValue)
            {
                metroLine.EndTime = updateMetroLineDTO.EndTime.Value;
            }
            metroLine.CreatedAt = DateTime.UtcNow;
        }

        public async Task<ResponseDTO> ChangeMetroLineStatus(Guid metroLineId, MetroLineStatus metroLineStatus)
        {
            try
            {
                var metroLine = await _unitOfWork.MetroLineRepository.GetByIdAsync(metroLineId);
                if (metroLine is null)
                {
                    return new ResponseDTO
                    {
                        IsSuccess = false,
                        StatusCode = 404,
                        Message = "Không tìm thấy tuyến Metro"
                    };
                }

                metroLine.Status = metroLineStatus;
                // _unitOfWork.MetroLineRepository.Update(metroLine);
                _unitOfWork.SaveAsync().Wait();

                return new ResponseDTO
                {
                    IsSuccess = true,
                    StatusCode = 200,
                    Message = "Cập nhật trạng thái tuyến Metro thành công"
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO
                {
                    IsSuccess = false,
                    StatusCode = 500,
                    Message = "Lỗi khi cập nhật trạng thái tuyến Metro: " + ex.Message
                };
            }
        }

        public async Task<ResponseDTO> CheckMetroLineErrorInPath(Guid stationStartId, Guid stationEndId)
        {
            try
            {
                var isActiveMetro = true;
                var allMetroline = await _unitOfWork.MetroLineRepository.GetAllListAsync(isActiveMetro);

                var graph = new StationGraph(allMetroline);
                var stationPath = graph.FindShortestPath(stationStartId, stationEndId);

                if (stationPath == null || stationPath.Count == 0)
                {
                    return new ResponseDTO
                    {
                        IsSuccess = false,
                        StatusCode = 404,
                        Message = "Không tìm thấy đường đi giữa hai trạm"
                    };
                }

                int i = 0;
                var faultyLines = new List<object>();
                while (i < stationPath.Count - 1)
                {
                    var currentStationId = stationPath[i];
                    var nextStationId = stationPath[i + 1];

                    var metro = allMetroline.FirstOrDefault(l =>
                        l.MetroLineStations.Any(s => s.StationId == currentStationId) &&
                        l.MetroLineStations.Any(s => s.StationId == nextStationId));

                    if (metro != null)
                    {
                        if (metro.Status == MetroLineStatus.Faulty)
                        {
                            faultyLines.Add(new
                            {
                                MetroId = metro.Id,
                                MetroName = metro.MetroName,
                                Status = metro.Status
                            });
                        }

                        while (i < stationPath.Count - 1 &&
                               metro.MetroLineStations.Any(s => s.StationId == stationPath[i + 1]))
                        {
                            i++;
                        }
                    }
                    else
                    {
                        i++;
                    }
                }

                if (faultyLines.Count > 0)
                {
                    return new ResponseDTO
                    {
                        IsSuccess = false,
                        StatusCode = 200,
                        Message = $"Có {faultyLines.Count} tuyến metro bị lỗi trên đường đi",
                        Result = faultyLines
                    };
                }

                return new ResponseDTO
                {
                    IsSuccess = true,
                    StatusCode = 200,
                    Message = "Không có tuyến metro nào bị lỗi trong đường đi giữa hai trạm",
                    Result = null
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO
                {
                    IsSuccess = false,
                    StatusCode = 500,
                    Message = "Lỗi khi kiểm tra tuyến metro: " + ex.Message
                };
            }
        }

        public async Task<ResponseDTO> CheckMetroLineErrorInPathV2(Guid ticketId)
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
                        Message = "Không tìm thấy vé"
                    };
                }

                var isActiveMetro = true;
                var allMetroline = await _unitOfWork.MetroLineRepository.GetAllListAsync(isActiveMetro);

                var graph = new StationGraph(allMetroline);
                var stationPath = new List<Guid>();

                if (ticket.SubscriptionTicket is null && ticket.TicketRoute is not null)
                {
                    stationPath = graph.FindShortestPath(ticket.TicketRoute.StartStationId, ticket.TicketRoute.EndStationId);
                }
                else if (ticket.SubscriptionTicket is not null && ticket.TicketRoute is null)
                {
                    stationPath = graph.FindShortestPath(ticket.SubscriptionTicket.StartStationId, ticket.SubscriptionTicket.EndStationId);
                }
                else if (ticket.SubscriptionTicket is not null && ticket.TicketRoute is not null)
                {
                    if (ticket.TicketRoute.EndStationId == ticket.SubscriptionTicket.StartStationId)
                    {
                        stationPath = graph.FindShortestPath(ticket.TicketRoute.StartStationId, ticket.SubscriptionTicket.EndStationId);
                    }
                    else
                    {
                        stationPath = graph.FindShortestPath(ticket.SubscriptionTicket.StartStationId, ticket.TicketRoute.EndStationId);
                    }
                }
                else
                {
                    return new ResponseDTO
                    {
                        IsSuccess = false,
                        StatusCode = 400,
                        Message = "Vé không có thông tin tuyến đường"
                    };
                }

                if (stationPath == null || stationPath.Count == 0)
                {
                    return new ResponseDTO
                    {
                        IsSuccess = false,
                        StatusCode = 404,
                        Message = "Không tìm thấy đường đi giữa hai trạm"
                    };
                }

                int i = 0;
                var faultyLines = new List<object>();
                while (i < stationPath.Count - 1)
                {
                    var currentStationId = stationPath[i];
                    var nextStationId = stationPath[i + 1];

                    var metro = allMetroline.FirstOrDefault(l =>
                        l.MetroLineStations.Any(s => s.StationId == currentStationId) &&
                        l.MetroLineStations.Any(s => s.StationId == nextStationId));

                    if (metro != null)
                    {
                        if (metro.Status == MetroLineStatus.Faulty)
                        {
                            faultyLines.Add(new
                            {
                                MetroId = metro.Id,
                                MetroName = metro.MetroName,
                                Status = metro.Status
                            });
                        }

                        while (i < stationPath.Count - 1 &&
                               metro.MetroLineStations.Any(s => s.StationId == stationPath[i + 1]))
                        {
                            i++;
                        }
                    }
                    else
                    {
                        i++;
                    }
                }

                if (faultyLines.Count > 0)
                {
                    return new ResponseDTO
                    {
                        IsSuccess = false,
                        StatusCode = 200,
                        Message = $"Có {faultyLines.Count} tuyến metro bị lỗi trên đường đi",
                        Result = faultyLines
                    };
                }

                return new ResponseDTO
                {
                    IsSuccess = true,
                    StatusCode = 200,
                    Message = "Không có tuyến metro nào bị lỗi trong đường đi giữa hai trạm",
                    Result = null
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO
                {
                    IsSuccess = false,
                    StatusCode = 500,
                    Message = "Lỗi khi kiểm tra tuyến metro: " + ex.Message
                };
            }
        }
    }
}
