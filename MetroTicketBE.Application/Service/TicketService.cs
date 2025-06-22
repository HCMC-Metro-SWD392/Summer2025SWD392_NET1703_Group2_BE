using AutoMapper;
using MetroTicketBE.Application.IService;
using MetroTicketBE.Domain.DTO.Auth;
using MetroTicketBE.Domain.DTO.Ticket;
using MetroTicketBE.Domain.Entities;
using MetroTicketBE.Domain.Enums;
using MetroTicketBE.Infrastructure.IRepository;
using System.Security.Claims;
using MetroTicketBE.Domain.Enum;
using MetroTicketBE.WebAPI.Extentions;
using Microsoft.AspNetCore.SignalR;
using System.Net.Sockets;
using Microsoft.Extensions.Logging;
using MetroTicketBE.Domain.DTO.Payment;
using MetroTicketBE.Domain.DTO.TicketRoute;

namespace MetroTicketBE.Application.Service
{
    public class TicketService : ITicketService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly Random random;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly ILogger<TicketService> _logger; // Add this field
        private readonly ITicketRouteService _ticketRouteService;
        public TicketService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ITokenService tokenService,
            IHubContext<NotificationHub> hubContext,
            ILogger<TicketService> logger,
            ITicketRouteService ticketRouteService)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            random = new Random();
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _ticketRouteService = new TicketRouteService(_unitOfWork, _mapper);
        }

        public async Task<ResponseDTO> GetAllTicketRoutes(
            ClaimsPrincipal user,
            string? filterOn,
            string? filterQuery,
            double? fromPrice,
            double? toPrice,
            string? sortBy,
            bool? isAcsending,
            TicketStatus ticketType,
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
                var tickets = (await _unitOfWork.TicketRepository.GetAllAsync(includeProperties: "TicketRoute,TicketRoute.StartStation,TicketRoute.EndStation,SubscriptionTicket,SubscriptionTicket.StartStation,SubscriptionTicket.EndStation"))
                        .Where(t => t.CustomerId == customer.Id);

                if (ticketType == TicketStatus.Inactive)
                {
                    tickets = tickets.Where(t => t.TicketRtStatus == TicketStatus.InActiveOverStation || t.TicketRtStatus == ticketType);
                }
                else if (ticketType == TicketStatus.Active)
                {
                    tickets = tickets.Where(t => t.TicketRtStatus == TicketStatus.ActiveOverStation || t.TicketRtStatus == ticketType);
                }
                else
                {
                    tickets = tickets.Where(t => t.TicketRtStatus == ticketType);
                }

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

        public async Task<ResponseDTO> CreateTicketForSpecialCase(ClaimsPrincipal user, Guid subscriptionId)
        {
            try
            {
                var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return new ResponseDTO
                    {
                        Message = "Không tìm thấy người dùng",
                        IsSuccess = false,
                        StatusCode = 404
                    };
                }

                var customer = await _unitOfWork.CustomerRepository.GetByUserIdAsync(userId);

                if (customer is null)
                {
                    return new ResponseDTO
                    {
                        Message = "Không tìm thấy khách hàng",
                        IsSuccess = false,
                        StatusCode = 404
                    };

                }

                if (customer.CustomerType != CustomerType.OlderPerson && customer.CustomerType != CustomerType.Military)
                {
                    return new ResponseDTO
                    {
                        Message = "Chỉ khách hàng người cao tuổi hoặc quân đội mới có thể tạo vé lượt đặc biệt",
                        IsSuccess = false,
                        StatusCode = 403
                    };
                }
                var subscription = await _unitOfWork.SubscriptionRepository.GetByIdAsync(subscriptionId);
                if (subscription is null)
                {
                    return new ResponseDTO
                    {
                        Message = "Không tìm thấy vé đặc biệt với ID này",
                        IsSuccess = false,
                        StatusCode = 404
                    };
                }
                var item = subscription.TicketName;
                var orderCode = Math.Abs(int.Parse(DateTimeOffset.Now.ToString("fffffff")) + customer.Id.GetHashCode());
                PaymentTransaction transaction = new PaymentTransaction()
                {
                    CustomerId = customer.Id,
                    OrderCode = Convert.ToString(orderCode),
                    DataJson = item,
                    TotalPrice = subscription.Price,
                    PaymentMethodId = _unitOfWork.PaymentMethodRepository.GetByNameAsync("PAYOS").Result.Id,
                    Status = PaymentStatus.Paid
                };
                await _unitOfWork.PaymentTransactionRepository.AddAsync(transaction);
                await _unitOfWork.SaveAsync();
                Ticket ticket = new Ticket()
                {
                    CustomerId = customer.Id,
                    SubscriptionTicketId = subscriptionId,
                    Price = subscription.Price,
                    TicketSerial = string.Concat(Enumerable.Range(0, 10).Select(_ => random.Next(0, 10).ToString())),
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddDays(subscription.TicketType.Expiration),
                    QrCode = Guid.NewGuid().ToString(),
                };
                await _unitOfWork.TicketRepository.AddAsync(ticket);
                await _unitOfWork.SaveAsync();
                return new ResponseDTO()
                {
                    Message = "Tạo vé lượt đặc biệt thành công",
                    IsSuccess = true,
                    StatusCode = 201,
                    Result = _mapper.Map<GetTicketDTO>(ticket)
                };
            }
            catch (Exception exception)
            {
                return new ResponseDTO()
                {
                    Message = "Đã xảy ra lỗi khi tạo vé lượt đặc biệt: " + exception.Message,
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

                if (ticket.TicketRtStatus == TicketStatus.Used)
                {
                    return new ResponseDTO
                    {
                        Message = "Vé lượt đã sử dụng",
                        IsSuccess = false,
                        StatusCode = 400
                    };
                }

                ticket.TicketRtStatus = ticket.TicketRtStatus == TicketStatus.Active ? TicketStatus.Inactive : TicketStatus.Active;

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

        public async Task<ResponseDTO> CheckInTicketProcess(string qrCode, Guid stationId)
        {
            try
            {
                var ticket = await _unitOfWork.TicketRepository.GetByQrCodeAsync(qrCode);
                if (ticket is null)
                {
                    var ticketId = await _tokenService.GetValueByKeyAsync($"qrCode:{qrCode}-ticketId");
                    if (ticketId is null)
                    {
                        return new ResponseDTO
                        {
                            Message = "QR code không hợp lệ hoặc đã hết hạn.",
                            IsSuccess = false,
                            StatusCode = 404
                        };
                    }
                    else
                    {
                        ticket = await _unitOfWork.TicketRepository.GetByIdAsync(Guid.Parse(ticketId));
                    }
                }

                if (ticket is null)
                {
                    return new ResponseDTO
                    {
                        Message = "Không tìm thấy vé cần xử lý.",
                        IsSuccess = false,
                        StatusCode = 404
                    };
                }

                if (ticket.EndDate < DateTime.UtcNow)
                {
                    return new ResponseDTO
                    {
                        Message = "Vé đã hết hạn sử dụng.",
                        IsSuccess = false,
                        StatusCode = 400
                    };
                }

                var customer = await _unitOfWork.CustomerRepository.GetByIdAsync(ticket.CustomerId);
                if (customer is null)
                {
                    return new ResponseDTO
                    {
                        Message = "Không tìm thấy khách hàng.",
                        IsSuccess = false,
                        StatusCode = 404
                    };
                }

                if (ticket.TicketRtStatus == TicketStatus.Used)
                {
                    return new ResponseDTO
                    {
                        Message = "Vé đã được sử dụng.",
                        IsSuccess = false,
                        StatusCode = 400
                    };
                }

                else if (ticket.TicketRtStatus == TicketStatus.Active || ticket.TicketRtStatus == TicketStatus.ActiveOverStation)
                {
                    await SendNotifyToUser(customer.UserId, "Vé đã được check-in trước đó.");
                    return new ResponseDTO
                    {
                        Message = "Vé đã được check-in trước đó.",
                        IsSuccess = false,
                        StatusCode = 400
                    };
                }
                else if (ticket.TicketRtStatus == TicketStatus.Inactive)
                {
                    var allMetroLines = await _unitOfWork.MetroLineRepository.GetAllListAsync();

                    var _graph = new StationGraph(allMetroLines);

                    var stationPath = new List<Guid>();

                    if (ticket.TicketRoute is not null)
                    {
                        stationPath = _graph.FindShortestPath(ticket.TicketRoute.StartStation.Id, ticket.TicketRoute.EndStationId);
                        return await CheckInTicketRouteAndSubProcess(ticket, stationId, stationPath, customer.UserId);
                    }
                    else if (ticket.SubscriptionTicket is not null)
                    {
                        stationPath = _graph.FindShortestPath(ticket.SubscriptionTicket.StartStationId, ticket.SubscriptionTicket.EndStationId);
                        return await CheckInTicketRouteAndSubProcess(ticket, stationId, stationPath, customer.UserId);
                    }
                    else
                    {
                        return new ResponseDTO
                        {
                            Message = "Vé lỗi định dạng không thể xác định vé lượt hay vé kỳ",
                            IsSuccess = false,
                            StatusCode = 404
                        };
                    }
                }
                else
                {
                    var allMetroLines = await _unitOfWork.MetroLineRepository.GetAllListAsync();

                    var _graph = new StationGraph(allMetroLines);

                    var stationPath = new List<Guid>();
                    var stationOverPath = new List<Guid>();
                    var totalPath = new List<Guid>();

                    if (ticket.TicketRoute is null)
                    {
                        return new ResponseDTO
                        {
                            Message = "Vé không có hỗ trợ vượt trạm",
                            IsSuccess = false,
                            StatusCode = 404
                        };
                    }

                    stationOverPath = _graph.FindShortestPath(ticket.TicketRoute.StartStation.Id, ticket.TicketRoute.EndStationId);
                    stationPath = _graph.FindShortestPath(ticket.SubscriptionTicket.StartStationId, ticket.SubscriptionTicket.EndStationId);

                    totalPath = stationOverPath.Concat(stationPath).ToList();

                    return await CheckInOverStationSubProcess(ticket, stationId, totalPath, customer.UserId);
                }
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

        private async Task<ResponseDTO> CheckInOverStationSubProcess(Ticket ticket, Guid stationId, List<Guid> stationPath, string userId)
        {
            if (stationPath.Contains(stationId))
            {
                ticket.TicketRtStatus = TicketStatus.ActiveOverStation;
                _unitOfWork.TicketRepository.Update(ticket);
                await _unitOfWork.SaveAsync();
                return new ResponseDTO
                {
                    Message = "Vé đã được check-in thành công.",
                    IsSuccess = true,
                    StatusCode = 200
                };
            }
            else
            {
                await FindTicketRouteForOverStationAndGetPrice(ticket, stationId, userId);
                return new ResponseDTO
                {
                    Message = "Trạm không đúng với phạm vi cho phép bắt đầu của vé (nằm ngoài vùng cho phép check-in).",
                    IsSuccess = false,
                    StatusCode = 400,
                    Result = ticket.Id
                };
            }
        }

        public async Task<ResponseDTO> CheckOutTicketProcess(string qrCode, Guid stationId)
        {
            try
            {
                var ticket = await _unitOfWork.TicketRepository.GetByQrCodeAsync(qrCode);
                if (ticket is null)
                {
                    var ticketId = await _tokenService.GetValueByKeyAsync($"qrCode:{qrCode}-ticketId");
                    if (ticketId is null)
                    {
                        return new ResponseDTO
                        {
                            Message = "QR code không hợp lệ hoặc đã hết hạn.",
                            IsSuccess = false,
                            StatusCode = 404
                        };
                    }
                    else
                    {
                        ticket = await _unitOfWork.TicketRepository.GetByIdAsync(Guid.Parse(ticketId));
                    }
                }

                if (ticket is null)
                {
                    return new ResponseDTO
                    {
                        Message = "Không tìm thấy vé cần xử lý.",
                        IsSuccess = false,
                        StatusCode = 404
                    };
                }
                var customer = await _unitOfWork.CustomerRepository.GetByIdAsync(ticket.CustomerId);
                if (customer is null)
                {
                    return new ResponseDTO
                    {
                        Message = "Không tìm thấy khách hàng.",
                        IsSuccess = false,
                        StatusCode = 404
                    };
                }

                if (ticket.EndDate < DateTime.UtcNow)
                {
                    await SendNotifyToUser(customer.UserId, "Vé đã hết hạn sử dụng.");
                    return new ResponseDTO
                    {
                        Message = "Vé đã hết hạn sử dụng.",
                        IsSuccess = false,
                        StatusCode = 400
                    };
                }

                if (ticket.TicketRtStatus == TicketStatus.Used)
                {
                    await SendNotifyToUser(customer.UserId, "Vé đã được sử dụng.");
                    return new ResponseDTO
                    {
                        Message = "Vé đã được sử dụng.",
                        IsSuccess = false,
                        StatusCode = 400
                    };
                }

                else if (ticket.TicketRtStatus == TicketStatus.Inactive || ticket.TicketRtStatus == TicketStatus.InActiveOverStation)
                {
                    await SendNotifyToUser(customer.UserId, "Vé chưa được check-in trước đó.");
                    return new ResponseDTO
                    {
                        Message = "Vé chưa được check-in trước đó.",
                        IsSuccess = false,
                        StatusCode = 400
                    };
                }
                else if (ticket.TicketRtStatus == TicketStatus.Active)
                {

                    var allMetroLines = await _unitOfWork.MetroLineRepository.GetAllListAsync();

                    var _graph = new StationGraph(allMetroLines);

                    var stationPath = new List<Guid>();

                    if (ticket.TicketRoute is not null)
                    {
                        stationPath = _graph.FindShortestPath(ticket.TicketRoute.StartStation.Id, ticket.TicketRoute.EndStationId);
                        return await CheckOutTicketRouteProcess(ticket, stationId, stationPath, customer.UserId);
                    }
                    else if (ticket.SubscriptionTicket is not null)
                    {
                        stationPath = _graph.FindShortestPath(ticket.SubscriptionTicket.StartStationId, ticket.SubscriptionTicket.EndStationId);
                        return await CheckOutProcessSubscriptionTicket(ticket, stationId, stationPath, customer.UserId);
                    }
                    else
                    {
                        return new ResponseDTO
                        {
                            Message = "Vé lỗi định dạng không thể xác định vé lượt hay vé kỳ",
                            IsSuccess = false,
                            StatusCode = 404
                        };
                    }
                }
                else
                {
                    var allMetroLines = await _unitOfWork.MetroLineRepository.GetAllListAsync();

                    var _graph = new StationGraph(allMetroLines);

                    var stationPath = new List<Guid>();
                    var stationOverPath = new List<Guid>();
                    var totalPath = new List<Guid>();

                    if (ticket.TicketRoute is null)
                    {
                        await SendNotifyToUser(customer.UserId, "Vé không có hỗ trợ vượt trạm");
                        return new ResponseDTO
                        {
                            Message = "Vé không có hỗ trợ vượt trạm",
                            IsSuccess = false,
                            StatusCode = 404
                        };
                    }

                    stationOverPath = _graph.FindShortestPath(ticket.TicketRoute.StartStation.Id, ticket.TicketRoute.EndStationId);
                    stationPath = _graph.FindShortestPath(ticket.SubscriptionTicket.StartStationId, ticket.SubscriptionTicket.EndStationId);

                    totalPath = stationOverPath.Concat(stationPath).ToList();

                    return await CheckOutOverStationSubProcess(ticket, stationId, totalPath, customer.UserId);
                }
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

        private async Task<ResponseDTO> CheckOutOverStationSubProcess(Ticket ticket, Guid stationId, List<Guid> stationPath, string userId)
        {
            if (stationPath.Contains(stationId))
            {
                ticket.TicketRtStatus = TicketStatus.Inactive;
                var priceTicketRoute = await _unitOfWork.FareRuleRepository.CalculatePriceFromDistance(ticket.TicketRoute?.Distance);
                Ticket newTicket = new Ticket
                {
                    CustomerId = ticket.CustomerId,
                    SubscriptionTicketId = null,
                    TicketRouteId = ticket.TicketRouteId,
                    Price = priceTicketRoute,
                    TicketSerial = $"Serial vé tích hợp: {ticket.TicketSerial}",
                    StartDate = ticket.StartDate,
                    EndDate = ticket.EndDate,
                    TicketRtStatus = TicketStatus.Used,
                    QrCode = ticket.QrCode,
                };

                ticket.TicketRoute = null;
                ticket.QrCode = null;
                _unitOfWork.TicketRepository.Update(ticket);
                await _unitOfWork.TicketRepository.AddAsync(newTicket);
                await _unitOfWork.SaveAsync();
                await SendNotifyToUser(userId, "Vé đã được check-out thành công.");
                return new ResponseDTO
                {
                    Message = "Vé đã được check-out thành công.",
                    IsSuccess = true,
                    StatusCode = 200
                };
            }
            else
            {
                await FindTicketRouteForOverStationAndGetPrice(ticket, stationId, userId);
                return new ResponseDTO
                {
                    Message = "Trạm không đúng với phạm vi cho phép kết thúc của vé (nằm ngoài vùng cho phép check-out).",
                    IsSuccess = false,
                    StatusCode = 400,
                };
            }
        }
        private async Task<ResponseDTO> CheckOutProcessSubscriptionTicket(Ticket ticket, Guid stationId, List<Guid> stationPath, string userId)
        {

            if (stationPath.Contains(stationId))
            {
                ticket.TicketRtStatus = TicketStatus.Inactive;
                _unitOfWork.TicketRepository.Update(ticket);
                await _unitOfWork.SaveAsync();
                await SendNotifyToUser(userId, "Vé đã được check-out thành công.");
                return new ResponseDTO
                {
                    Message = "Vé đã được check-out thành công.",
                    IsSuccess = true,
                    StatusCode = 200
                };
            }
            else
            {
                await FindTicketRouteForOverStationAndGetPrice(ticket, stationId, userId);
                return new ResponseDTO
                {
                    Message = "Trạm không đúng với phạm vi cho phép kết thúc của vé (nằm ngoài vùng cho phép check-out).",
                    IsSuccess = false,
                    StatusCode = 400
                };
            }
        }




        private async Task<ResponseDTO> CheckInTicketRouteAndSubProcess(Ticket ticket, Guid stationId, List<Guid> stationPath, string userId)
        {
            // Check log stations path
            //var stationNames = stationPath.Select(id =>
            //allMetroLines.SelectMany(l => l.MetroLineStations)
            //    .FirstOrDefault(s => s.StationId == id)?.Station.Name ?? id.ToString()).ToList();
            //Console.WriteLine("Đường đi: " + string.Join(" -> ", stationNames));

            // Check if the station is within the allowed range for check-out

            if (stationPath.Contains(stationId))
            {
                ticket.TicketRtStatus = TicketStatus.Active;
                _unitOfWork.TicketRepository.Update(ticket);
                await _unitOfWork.SaveAsync();
                await SendNotifyToUser(userId, "Vé đã được check-in thành công.");
                return new ResponseDTO
                {
                    Message = "Vé đã được check-in thành công.",
                    IsSuccess = true,
                    StatusCode = 200
                };
            }
            else
            {
                await FindTicketRouteForOverStationAndGetPrice(ticket, stationId, userId);
                return new ResponseDTO
                {
                    Message = "Trạm không đúng với phạm vi cho phép bắt đầu của vé (nằm ngoài vùng cho phép check-in).",
                    IsSuccess = false,
                    StatusCode = 400,
                    Result = ticket.Id
                };
            }
        }

        private async Task<ResponseDTO> FindTicketRouteForOverStationAndGetPrice(Ticket ticket, Guid stationId, string userId)
        {
            var allMetroLines = await _unitOfWork.MetroLineRepository.GetAllListAsync();

            var _graph = new StationGraph(allMetroLines);

            var stationPathWithStartStation = new List<Guid>();
            var stationPathWithEndStation = new List<Guid>();

            double? distance = 0;
            int totalPrice = 0;
            TicketRoute? ticketRoute = null;

            if (ticket.TicketRoute is not null)
            {
                stationPathWithStartStation = _graph.FindShortestPath(ticket.TicketRoute.StartStationId, stationId);
                stationPathWithEndStation = _graph.FindShortestPath(ticket.TicketRoute.EndStationId, stationId);

                if (stationPathWithStartStation.Count == 0 || stationPathWithEndStation.Count == 0)
                {
                    return new ResponseDTO
                    {
                        Message = "Không tìm thấy đường đi từ ga xuất phát hoặc ga kết thúc đến ga cần mua thêm",
                        IsSuccess = false,
                        StatusCode = 404
                    };
                }
                else
                {
                    if (stationPathWithStartStation.Count < stationPathWithEndStation.Count)
                    {
                        var TicketRouteRight = await _unitOfWork.TicketRouteRepository.GetTicketRouteByStartAndEndStation(stationId, ticket.TicketRoute.EndStationId);
                        if (TicketRouteRight is null)
                        {
                            CreateTicketRouteDTO create = new CreateTicketRouteDTO
                            {
                                StartStationId = stationId,
                                EndStationId = ticket.TicketRoute.EndStationId
                            };
                            await _ticketRouteService.CraeteTicketRoute(create);
                        }
                        ticketRoute = await _unitOfWork.TicketRouteRepository.GetTicketRouteByStartAndEndStation(stationId, ticket.TicketRoute.EndStationId);
                        distance = _graph.GetPathDistance(stationPathWithEndStation);
                    }
                    else
                    {
                        var TicketRouteLeft = await _unitOfWork.TicketRouteRepository.GetTicketRouteByStartAndEndStation(ticket.TicketRoute.StartStationId, stationId);
                        if (TicketRouteLeft is null)
                        {
                            CreateTicketRouteDTO create = new CreateTicketRouteDTO
                            {
                                StartStationId = ticket.TicketRoute.StartStationId,
                                EndStationId = stationId
                            };
                            await _ticketRouteService.CraeteTicketRoute(create);
                        }
                        ticketRoute = await _unitOfWork.TicketRouteRepository.GetTicketRouteByStartAndEndStation(ticket.TicketRoute.StartStationId, stationId);
                        distance = _graph.GetPathDistance(stationPathWithStartStation);
                    }
                }
                int ticketPrice = await _unitOfWork.FareRuleRepository.CalculatePriceFromDistance(distance);
                totalPrice = Math.Abs(ticketPrice - ticket.Price);
            }
            else
            {
                stationPathWithStartStation = _graph.FindShortestPath(ticket.SubscriptionTicket.StartStationId, stationId);
                stationPathWithEndStation = _graph.FindShortestPath(ticket.SubscriptionTicket.EndStationId, stationId);

                if (stationPathWithStartStation.Count == 0 || stationPathWithEndStation.Count == 0)
                {
                    return new ResponseDTO
                    {
                        Message = "Không tìm thấy đường đi từ ga xuất phát hoặc ga kết thúc đến ga cần mua thêm",
                        IsSuccess = false,
                        StatusCode = 404
                    };
                }
                else
                {
                    if (stationPathWithStartStation.Count >= stationPathWithEndStation.Count)
                    {
                        var TicketRouteLeft = await _unitOfWork.TicketRouteRepository.GetTicketRouteByStartAndEndStation(ticket.SubscriptionTicket.EndStationId, stationId);
                        if (TicketRouteLeft is null)
                        {
                            CreateTicketRouteDTO create = new CreateTicketRouteDTO
                            {
                                StartStationId = ticket.SubscriptionTicket.EndStationId,
                                EndStationId = stationId
                            };
                            await _ticketRouteService.CraeteTicketRoute(create);
                        }
                        ticketRoute = await _unitOfWork.TicketRouteRepository.GetTicketRouteByStartAndEndStation(ticket.SubscriptionTicket.EndStationId, stationId);
                        distance = _graph.GetPathDistance(stationPathWithEndStation);
                    }
                    else
                    {
                        var TicketRouteLeft = await _unitOfWork.TicketRouteRepository.GetTicketRouteByStartAndEndStation(stationId, ticket.SubscriptionTicket.StartStationId);
                        if (TicketRouteLeft is null)
                        {
                            CreateTicketRouteDTO create = new CreateTicketRouteDTO
                            {
                                StartStationId = stationId,
                                EndStationId = ticket.SubscriptionTicket.StartStationId
                            };
                            await _ticketRouteService.CraeteTicketRoute(create);
                        }
                        ticketRoute = await _unitOfWork.TicketRouteRepository.GetTicketRouteByStartAndEndStation(stationId, ticket.SubscriptionTicket.StartStationId);
                        distance = _graph.GetPathDistance(stationPathWithStartStation);
                    }

                    totalPrice = await _unitOfWork.FareRuleRepository.CalculatePriceFromDistance(distance);
                }
            }
            if (totalPrice == 0)
            {
                await SendNotifyOverStation(ticket, stationId, $"Bạn đã vượt trạm nhưng nằm trong vùng đổi vé miễn phí, vui lòng xác nhận để chuyển sang vé phù hợp.", userId);
            }
            else
            {
                await SendNotifyOverStation(ticket, stationId, $"Bạn đã vượt trạm! Vui lòng thanh toán thêm {totalPrice}VND", userId);
            }
            return new ResponseDTO
            {
                Message = "Tìm đường đi thành công",
                IsSuccess = true,
                StatusCode = 200
            };
        }

        private async Task<ResponseDTO> CheckOutTicketRouteProcess(Ticket ticket, Guid stationId, List<Guid> stationPath, string userId)
        {
            // Check log stations path
            //var stationNames = stationPath.Select(id =>
            //allMetroLines.SelectMany(l => l.MetroLineStations)
            //    .FirstOrDefault(s => s.StationId == id)?.Station.Name ?? id.ToString()).ToList();
            //Console.WriteLine("Đường đi: " + string.Join(" -> ", stationNames));

            // Check if the station is within the allowed range for check-out

            if (stationPath.Contains(stationId))
            {
                ticket.TicketRtStatus = TicketStatus.Used;
                _unitOfWork.TicketRepository.Update(ticket);
                await _unitOfWork.SaveAsync();
                await SendNotifyToUser(userId, "Vé lượt của bạn đã được check-out thành công.");
                return new ResponseDTO
                {
                    Message = "Vé lượt đã được check-out thành công.",
                    IsSuccess = true,
                    StatusCode = 200
                };
            }
            else
            {
                await FindTicketRouteForOverStationAndGetPrice(ticket, stationId, userId);
                return new ResponseDTO
                {
                    Message = "Trạm không đúng với phạm vi cho phép kết thúc của vé lượt (nằm ngoài vùng cho phép check-out).",
                    IsSuccess = false,
                    StatusCode = 400,
                    Result = ticket.Id
                };
            }
        }

        public async Task<ResponseDTO> GetORCode(ClaimsPrincipal user, Guid ticketId)
        {
            try
            {
                var ticket = await _unitOfWork.TicketRepository.GetByIdAsync(ticketId);

                if (ticket is null)
                {
                    return new ResponseDTO
                    {
                        Message = "Không tìm thấy vé cần lấy QR code.",
                        IsSuccess = false,
                        StatusCode = 404
                    };
                }
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

                if (customer is null || customer.Id != ticket.CustomerId)
                {
                    await SendNotifyToUser(userId, "Bạn không có quyền truy cập mã QR của vé này.");
                    return new ResponseDTO
                    {
                        Message = "Bạn không có quyền truy cập mã QR của vé này.",
                        IsSuccess = false,
                        StatusCode = 403
                    };
                }

                if (ticket.TicketRoute is not null && ticket.SubscriptionTicket is null)
                {
                    ticket.QrCode = _tokenService.GenerateQRCodeAsync();
                    _unitOfWork.TicketRepository.Update(ticket);
                    await _unitOfWork.SaveAsync();
                }
                else
                {
                    ticket.QrCode = await _tokenService.GetQRCodeAndRefreshAsync(ticket.Id);
                }

                return new ResponseDTO
                {
                    Message = "Lấy QR code thành công",
                    IsSuccess = true,
                    StatusCode = 200,
                    Result = ticket.QrCode
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO
                {
                    Message = "Đã xảy ra lỗi khi lấy QR code: " + ex.Message,
                    IsSuccess = false,
                    StatusCode = 500
                };
            }
        }

        private async Task<ResponseDTO> SendNotifyToUser(string userId, string message)
        {
            if (userId is null)
            {
                return new ResponseDTO
                {
                    Message = "Không tìm thấy người dùng.",
                    IsSuccess = false,
                    StatusCode = 404
                };
            }

            await _hubContext.Clients.User(userId).SendAsync("NotifyOverStation", new
            {
                Message = message
            });

            return new ResponseDTO
            {
                Message = "Thông báo đã được gửi thành công.",
                IsSuccess = true,
                StatusCode = 200
            };
        }

        private async Task<ResponseDTO> SendNotifyOverStation(Ticket ticket, Guid stationId, string message, string userId)
        {
            await _hubContext.Clients.User(userId).SendAsync("NotifyOverStation", new
            {
                TicketId = ticket.Id,
                StationId = stationId,
                Message = message
            });

            return new ResponseDTO
            {
                Message = "Thông báo vượt trạm đã được gửi thành công.",
                IsSuccess = true,
                StatusCode = 200
            };
        }

        //private async Task<bool> CheckOutTicket(Ticket ticket, Guid stationId, Guid metroLineId)
        //{
        //    var isSuccess = false;
        //    int orderStart = await _unitOfWork.StationRepository.GetOrderStationById(ticket.TicketRoute.StartStation.Id, metroLineId);
        //    int orderEnd = await _unitOfWork.StationRepository.GetOrderStationById(ticket.TicketRoute.EndStation.Id, metroLineId);
        //    int order = await _unitOfWork.StationRepository.GetOrderStationById(stationId, metroLineId);

        //    if ((orderStart <= order && order <= orderEnd) || (orderStart >= order && order >= orderEnd))
        //    {
        //        ticket.TicketRtStatus = TicketStatus.Used;
        //        _unitOfWork.TicketRepository.Update(ticket);
        //        await _unitOfWork.SaveAsync();
        //        isSuccess = true;
        //    }
        //    return isSuccess;
        //}
    }
}
