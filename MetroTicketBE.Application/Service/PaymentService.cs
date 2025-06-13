using MetroTicketBE.Application.IService;
using MetroTicketBE.Domain.Constants;
using MetroTicketBE.Domain.DTO.Auth;
using MetroTicketBE.Domain.DTO.Payment;
using MetroTicketBE.Domain.Entities;
using MetroTicketBE.Domain.Enum;
using MetroTicketBE.Domain.Enums;
using MetroTicketBE.Infrastructure.IRepository;
using MetroTicketBE.WebAPI.Extentions;
using Microsoft.Extensions.Configuration;
using Net.payOS;
using Net.payOS.Types;
using System.Security.Claims;
using System.Text.Json;
using SubscriptionTicketType = MetroTicketBE.Domain.Entities.SubscriptionTicketType;

namespace MetroTicketBE.Application.Service
{
    public class PaymentService : IPaymentService
    {
        private readonly IConfiguration _configuration;
        private readonly PayOS _payos;
        private readonly IUnitOfWork _unitOfWork;
        private readonly Random random;

        public PaymentService
        (
            IConfiguration configuration,
            StationGraph stationGraph,
            IUnitOfWork unitOfWork
        )
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _payos = new PayOS(
                    _configuration["Payos:CLIENT_ID"] ?? throw new Exception("Cannot find PAYOS_CLIENT_ID"),
                    _configuration["Payos:API_KEY"] ?? throw new Exception("Cannot find PAYOS_API_KEY"),
                    _configuration["Payos:CHECKSUM_KEY"] ?? throw new Exception("Cannot find PAYOS_CHECKSUM_KEY")
                );
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            random = new Random();
        }

       
        public async Task<ResponseDTO> CreateLinkPaymentTicketRoutePayOS(ClaimsPrincipal user, CreateLinkPaymentRoutePayOSDTO createLinkDTO)
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
                Promotion? promotion = null;

                if (!string.IsNullOrWhiteSpace(createLinkDTO.CodePromotion))
                {
                    // Kiểm tra mã khuyến mãi nếu có
                    promotion = await _unitOfWork.PromotionRepository.GetByCodeAsync(createLinkDTO.CodePromotion);
                    if (promotion is null)
                    {
                        return new ResponseDTO
                        {
                            Message = "Không tìm thấy mã khuyến mãi",
                            IsSuccess = false,
                            StatusCode = 404
                        };
                    }
                }

                var ticketRoute = createLinkDTO.TicketRouteId is not null
                    ? await _unitOfWork.TicketRouteRepository.GetByIdAsync(createLinkDTO.TicketRouteId)
                    : null;

                var subscriptionTicket = createLinkDTO.SubscriptionTicketId is not null
                    ? await _unitOfWork.SubscriptionRepository.GetByIdAsync(createLinkDTO.SubscriptionTicketId)
                    : null;

                if (ticketRoute is null && subscriptionTicket is null)
                {
                    return new ResponseDTO
                    {
                        Message = "Không có vé nào để thanh toán",
                        IsSuccess = false,
                        StatusCode = 400
                    };
                }
                var isStudent = customer.CustomerType == CustomerType.Student;
                bool isExpired = customer.StudentExpiration.HasValue && customer.StudentExpiration.Value < DateTime.UtcNow;
                if (!isStudent || isExpired && subscriptionTicket is not null && subscriptionTicket.TicketType.Name is "student" )
                {
                    return new ResponseDTO
                    {
                        Message = "Khách hàng không đủ điều kiện để mua vé sinh viên",
                        IsSuccess = false,
                        StatusCode = 403
                    };
                }

                int ticketRoutePrice = ticketRoute?.Distance is not null
                    ? await _unitOfWork.FareRuleRepository.CalculatePriceFromDistance(ticketRoute.Distance)
                    : 0;

                var items = new List<ItemData> {
                    ticketRoute is not null
                    ? new ItemData(ticketRoute.TicketName, 1, ticketRoutePrice)
                    : new ItemData(subscriptionTicket.TicketName, 1, subscriptionTicket.Price)
                    };


                var ticketPrice = ticketRoutePrice + (subscriptionTicket?.Price ?? 0);
                var totalPrice = ticketRoute is not null
                    ? await CalculatePriceApplyPromo(ticketPrice, promotion?.Id)
                    : ticketPrice;

                // Tạo mã đơn hàng duy nhất dựa trên thời gian hiện tại (orderCode)
                var orderCode = Math.Abs(int.Parse(DateTimeOffset.Now.ToString("fffffff")) + customer.Id.GetHashCode());
                PaymentData paymentLinkRequest = new PaymentData
            (
                orderCode: orderCode,
                amount: totalPrice,
                description: "METRO HCMC",
                items: items,
                returnUrl: StaticURL.Frontend_Url_Return_Payment,
                cancelUrl: StaticURL.Frontend_Url_Return_Payment
            );

                if (paymentLinkRequest is null)
                {
                    return new ResponseDTO
                    {
                        Message = "Thiếu thông tin không thể tạo mã thanh toán",
                        IsSuccess = false,
                        StatusCode = 400
                    };
                }

                var paymentMethod = await _unitOfWork.PaymentMethodRepository.GetByNameAsync(StaticPaymentMethod.PayOSMethod);

                if (paymentMethod is null)
                {
                    return new ResponseDTO
                    {
                        Message = "Không tìm thấy phương thức thanh toán PayOS",
                        IsSuccess = false,
                        StatusCode = 404
                    };
                }

                // Tạo mã thanh toán
                var response = await _payos.createPaymentLink(paymentLinkRequest);

                PaymentTransaction paymentTransaction = new PaymentTransaction()
                {
                    CustomerId = customer.Id,
                    OrderCode = Convert.ToString(orderCode),
                    ItemDataJson = JsonSerializer.Serialize(items),
                    TotalPrice = totalPrice,
                    PromotionId = promotion?.Id,
                    PaymentMethodId = paymentMethod.Id,
                    Status = PaymentStatus.Unpaid
                };

                await _unitOfWork.PaymentTransactionRepository.AddAsync(paymentTransaction);
                await _unitOfWork.SaveAsync();

                return new ResponseDTO
                {
                    Result = new
                    {
                        PaymentLink = response,
                        PaymentTransactionId = paymentTransaction.Id
                    },
                    Message = "Tạo liên kết thanh toán thành công",
                    IsSuccess = true,
                    StatusCode = 201
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO
                {
                    Message = $"Đã xảy ra lỗi khi tạo liên kết thanh toán: {ex.Message}",
                    IsSuccess = false,
                    StatusCode = 500
                };
            }
        }

        public async Task<ResponseDTO> UpdatePaymentTickerStatusPayOS(ClaimsPrincipal user, string orderCode)
        {
            try
            {
                var paymentTransaction = await _unitOfWork.PaymentTransactionRepository.GetByOrderCode(orderCode);
                if (paymentTransaction is null)
                {
                    return new ResponseDTO
                    {
                        Message = $"Không tìm thấy mã giao dịch: {orderCode}.",
                        IsSuccess = false,
                        StatusCode = 404
                    };
                }
                var paymentStatus = _payos.getPaymentLinkInformation(long.Parse(orderCode));

                if (paymentStatus is null)
                {
                    return new ResponseDTO
                    {
                        Message = "Không tìm thấy thông tin giao dịch trên hệ thống PayOS",
                        IsSuccess = false,
                        StatusCode = 404
                    };
                }
                else
                {
                    paymentTransaction.Status = paymentStatus.Result.status switch
                    {
                        "PAID" => PaymentStatus.Paid,
                        "UNPAID" => PaymentStatus.Unpaid,
                        "CANCELED" => PaymentStatus.Canceled,
                        _ => paymentTransaction.Status
                    };
                }

                _unitOfWork.PaymentTransactionRepository.Update(paymentTransaction);

                if (paymentTransaction.Status is PaymentStatus.Paid)
                {
                    var items = JsonSerializer.Deserialize<List<ItemData>>(paymentTransaction.ItemDataJson ?? "");
                    var item = items?.FirstOrDefault();
                    if (item is null)
                    {
                        return new ResponseDTO
                        {
                            Message = "Không có vé nào để thanh toán",
                            IsSuccess = false,
                            StatusCode = 400
                        };
                    }

                    var ticketRoute = await _unitOfWork.TicketRouteRepository.GetByNameAsync(item.name);
                    var subTicket = await _unitOfWork.SubscriptionRepository.GetByNameAsync(item.name);

                    var expiration = ticketRoute is not null
                        ? TimeSpan.FromDays(30)
                        : TimeSpan.FromDays(subTicket.TicketType.Expiration);

                    int ticketPrice = ticketRoute?.Distance is not null
                    ? await _unitOfWork.FareRuleRepository.CalculatePriceFromDistance(ticketRoute.Distance)
                    : subTicket.Price;

                    Ticket ticket = new Ticket()
                    {
                        CustomerId = paymentTransaction.CustomerId,
                        SubscriptionTicketId = subTicket?.Id,
                        TicketRouteId = ticketRoute?.Id,
                        TransactionId = paymentTransaction.Id,
                        Price = ticketPrice,
                        TicketSerial = string.Concat(Enumerable.Range(0, 10).Select(_ => random.Next(0, 10).ToString())),
                        StartDate = DateTime.UtcNow,
                        EndDate = DateTime.UtcNow.Add(expiration)
                    };

                    await _unitOfWork.TicketRepository.AddAsync(ticket);
                }

                await _unitOfWork.SaveAsync();
                return new ResponseDTO
                {
                    Message = $"Cập nhật trạng thái thanh toán thành công",
                    IsSuccess = true,
                    StatusCode = 201
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO
                {
                    Message = $"Đã xảy ra lỗi khi cập nhật trạng thái thanh toán: {ex.Message}",
                    IsSuccess = false,
                    StatusCode = 500
                };
            }
        }

        private async Task<int> CalculatePriceApplyPromo(int price, Guid? promotionId)
        {
            if (promotionId is null || promotionId == Guid.Empty)
            {
                return price;
            }

            // Kiểm tra xem có tồn tại mã khuyến mãi không  
            var promotion = await _unitOfWork.PromotionRepository.GetByIdAsync(promotionId.Value);

            if (promotion is null)
            {
                return price;
            }

            var finalPrice = promotion.PromotionType == PromotionType.Percentage && promotion.Percentage.HasValue
                ? price * (1 - promotion.Percentage.Value / 100m)
                : price - (promotion.FixedAmount ?? 0);

            return (int)finalPrice;
        }
    }
}