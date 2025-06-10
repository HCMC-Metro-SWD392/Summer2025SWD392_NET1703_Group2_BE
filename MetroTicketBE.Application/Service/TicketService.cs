using AutoMapper;
using MetroTicketBE.Application.IService;
using MetroTicketBE.Domain.DTO.Auth;
using MetroTicketBE.Domain.DTO.Ticket;
using MetroTicketBE.Domain.Entities;
using MetroTicketBE.Domain.Enums;
using MetroTicketBE.Infrastructure.IRepository;
using System.Net.Sockets;
using System.Security.Claims;

namespace MetroTicketBE.Application.Service
{
    public class TicketService : ITicketService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public TicketService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<ResponseDTO> GetAllTicketRoutes(
            ClaimsPrincipal user,
            string? filterOn,
            string? filterQuery,
            double? fromPrice,
            double? toPrice,
            string? sortBy,
            bool? isAcsending,
            TicketRouteStatus ticketType,
            int pageNumber,
            int pageSize)
        {
            try
            {
                var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return new ResponseDTO
                    {
                        Message = "Người dùng không hợp lệ.",
                        IsSuccess = false,
                        StatusCode = 400
                    };
                }

                var customer = await _unitOfWork.CustomerRepository.GetByUserIdAsync(userId);
                if (customer is null)
                {
                    return new ResponseDTO
                    {
                        Message = "Không tìm thấy khách hàng.",
                        IsSuccess = false,
                        StatusCode = 404
                    };
                }

                var tickets = (await _unitOfWork.TicketRepository.GetAllAsync(includeProperties: "TicketRoute,TicketRoute.StartStation,TicketRoute.EndStation"))
                    .Where(t => t.CustomerId == customer.Id && t.TicketRtStatus == ticketType);

                if (!string.IsNullOrEmpty(filterOn) && !string.IsNullOrEmpty(filterQuery))
                {
                    string filter = filterOn.Trim().ToLower();
                    string query = filterQuery.Trim();

                    tickets = filter switch
                    {
                        "ticketname" => tickets.Where(t => t.TicketRoute.TicketName.Contains(query, StringComparison.CurrentCultureIgnoreCase)),
                        "startstation" => tickets.Where(t => t.TicketRoute.StartStation.Name.Contains(query, StringComparison.CurrentCultureIgnoreCase)),
                        "endstation" => tickets.Where(t => t.TicketRoute.EndStation.Name.Contains(query, StringComparison.CurrentCultureIgnoreCase)),
                        _ => tickets
                    };
                }

                if (!string.IsNullOrEmpty(sortBy))
                {
                    tickets = sortBy.Trim().ToLower() switch
                    {
                        "ticketname" => isAcsending is true ? tickets.OrderBy(t => t.TicketRoute.TicketName) : tickets.OrderByDescending(t => t.TicketRoute.TicketName),
                        "startstation" => isAcsending is true ? tickets.OrderBy(t => t.TicketRoute.StartStation.Name) : tickets.OrderByDescending(t => t.TicketRoute.StartStation.Name),
                        "endstation" => isAcsending is true ? tickets.OrderBy(t => t.TicketRoute.EndStation.Name) : tickets.OrderByDescending(t => t.TicketRoute.EndStation.Name),
                        _ => tickets
                    };
                }

                if (pageNumber > 0 && pageSize > 0)
                {
                    tickets = tickets.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                }

                if (tickets is null || !tickets.Any())
                {
                    return new ResponseDTO
                    {
                        Message = "Không tìm thấy vé lượt nào.",
                        IsSuccess = false,
                        StatusCode = 404
                    };
                }


                var getTickets = _mapper.Map<List<GetTicketDTO>>(tickets);

                return new ResponseDTO
                {
                    Message = "Lấy danh sách vé lượt thành công",
                    IsSuccess = true,
                    StatusCode = 200,
                    Result = getTickets
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO
                {
                    Message = "Đã xảy ra lỗi khi lấy danh sách vé lượt: " + ex.Message,
                    IsSuccess = false,
                    StatusCode = 500
                };
            }
        }

        public async Task<ResponseDTO> GetTicketBySerial(string serial)
        {
            try
            {
                var ticket = await _unitOfWork.TicketRepository.GetTicketBySerialAsync(serial);
                if (ticket is null)
                {
                    return new ResponseDTO
                    {
                        Message = "Không tìm thấy vé lượt với serial này.",
                        IsSuccess = false,
                        StatusCode = 404
                    };
                }
                var getTicketRoute = _mapper.Map<GetTicketDTO>(ticket);
                return new ResponseDTO
                {
                    Message = "Lấy vé lượt thành công",
                    IsSuccess = true,
                    StatusCode = 200,
                    Result = getTicketRoute
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO
                {
                    Message = "Đã xảy ra lỗi khi lấy vé lượt: " + ex.Message,
                    IsSuccess = false,
                    StatusCode = 500
                };
            }
        }

        public async Task<ResponseDTO> ChangeTicketRouteStatus(Guid ticketRouteId)
        {
            try
            {
                var ticket = await _unitOfWork.TicketRepository.GetByIdAsync(ticketRouteId);

                if (ticket is null)
                {
                    return new ResponseDTO
                    {
                        Message = "Không tìm thấy vé lượt cần thay đổi trạng thái.",
                        IsSuccess = false,
                        StatusCode = 404
                    };
                }

                if (ticket.TicketRtStatus == TicketRouteStatus.Used)
                {
                    return new ResponseDTO
                    {
                        Message = "Vé lượt đã sử dụng",
                        IsSuccess = false,
                        StatusCode = 400
                    };
                }

                ticket.TicketRtStatus = ticket.TicketRtStatus == TicketRouteStatus.Active ? TicketRouteStatus.Inactive : TicketRouteStatus.Active;

                _unitOfWork.TicketRepository.Update(ticket);
                await _unitOfWork.SaveAsync();

                return new ResponseDTO
                {
                    Message = "Thay đổi trạng thái vé lượt thành công",
                    IsSuccess = true,
                    StatusCode = 201
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO
                {
                    Message = "Đã xảy ra lỗi khi thay đổi trạng thái vé lượt: " + ex.Message,
                    IsSuccess = false,
                    StatusCode = 500
                };
            }
        }

        public async Task<ResponseDTO> TicketRouteProcess(Guid ticketRouteId, Guid stationId, Guid metroLineId)
        {
            try
            {
                var ticket = await _unitOfWork.TicketRepository.GetByIdAsync(ticketRouteId);

                if (ticket is null)
                {
                    return new ResponseDTO
                    {
                        Message = "Không tìm thấy vé lượt cần xử lý.",
                        IsSuccess = false,
                        StatusCode = 404
                    };
                }

                if (ticket.SubscriptionTicketId is not null)
                {
                    return new ResponseDTO
                    {
                        Message = "Vé này là vé định kỳ, không thể xử lý.",
                        IsSuccess = false,
                        StatusCode = 400
                    };
                }

                if (ticket.TicketRtStatus == TicketRouteStatus.Inactive)
                {
                    if (ticket.TicketRoute?.StartStation is null)
                    {
                        return new ResponseDTO
                        {
                            Message = "Lỗi không tìm thấy trạm bắt đầu.",
                            IsSuccess = false,
                            StatusCode = 400
                        };
                    }

                    var isCorrectStation = ticket.TicketRoute.StartStation.Id == stationId;

                    if (isCorrectStation)
                    {
                        ticket.TicketRtStatus = TicketRouteStatus.Active;
                        _unitOfWork.TicketRepository.Update(ticket);
                        await _unitOfWork.SaveAsync();

                        return new ResponseDTO
                        {
                            Message = "Vé lượt đã được check-in thành công.",
                            IsSuccess = true,
                            StatusCode = 200
                        };
                    }
                    else
                    {
                        return new ResponseDTO
                        {
                            Message = "Trạm không đúng với trạm bắt đầu của vé lượt.",
                            IsSuccess = false,
                            StatusCode = 400
                        };
                    }
                }

                else if (ticket.TicketRtStatus == TicketRouteStatus.Active)
                {
                    if (ticket.TicketRoute?.EndStation is null)
                    {
                        return new ResponseDTO
                        {
                            Message = "Lỗi không tìm thấy trạm kết thúc hoặc chưa check-in.",
                            IsSuccess = false,
                            StatusCode = 400
                        };
                    }

                    var result = await CheckOutValidation(ticket, stationId, metroLineId);

                    if (result)
                    {
                        return new ResponseDTO
                        {
                            Message = "Vé lượt đã được check-out thành công.",
                            IsSuccess = true,
                            StatusCode = 200
                        };
                    }

                    return new ResponseDTO
                    {
                        Message = "Trạm không đúng với phạm vi cho phép kết thúc của vé lượt (nằm ngoài vùng cho phép check-uot).",
                        IsSuccess = false,
                        StatusCode = 400
                    };
                }

                else if (ticket.TicketRtStatus == TicketRouteStatus.Used)
                {
                    return new ResponseDTO
                    {
                        Message = "Vé lượt đã được sử dụng.",
                        IsSuccess = false,
                        StatusCode = 400
                    };
                }

                return new ResponseDTO
                {
                    Message = "Đã xảy ra lỗi không kích hoạt luồng điều khiển sử dụng vé",
                    IsSuccess = false,
                    StatusCode = 400
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO
                {
                    Message = "Đã xảy ra lỗi khi xử lý vé lượt: " + ex.Message,
                    IsSuccess = false,
                    StatusCode = 500
                };
            }
        }

        private async Task<bool> CheckOutValidation(Ticket ticket, Guid stationId, Guid metroLineId)
        {
            var isSuccess = false;

            if (ticket.TicketRoute.EndStation.Id == stationId)
            {
                ticket.TicketRtStatus = TicketRouteStatus.Used;
                _unitOfWork.TicketRepository.Update(ticket);
                await _unitOfWork.SaveAsync();
                isSuccess = true;
            }
            else if (await _unitOfWork.MetroLineRepository.IsSameMetroLine(ticket.TicketRoute.EndStation.Id, stationId))
            {
                int orderStart = await _unitOfWork.StationRepository.GetOrderStationById(ticket.TicketRoute.StartStation.Id, metroLineId);
                int orderEnd = await _unitOfWork.StationRepository.GetOrderStationById(ticket.TicketRoute.EndStation.Id, metroLineId);
                int order = await _unitOfWork.StationRepository.GetOrderStationById(stationId, metroLineId);

                if ((orderStart <= order && order <= orderEnd) || (orderStart >= order && order >= orderEnd))
                {
                    ticket.TicketRtStatus = TicketRouteStatus.Used;
                    _unitOfWork.TicketRepository.Update(ticket);
                    await _unitOfWork.SaveAsync();
                    isSuccess = true;
                }
            }
            return isSuccess;
        }
    }
}
