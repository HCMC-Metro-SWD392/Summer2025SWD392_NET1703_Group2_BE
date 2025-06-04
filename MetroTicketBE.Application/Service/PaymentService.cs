using MetroTicketBE.Application.IService;
using MetroTicketBE.Domain.Constants;
using MetroTicketBE.Domain.DTO.Auth;
using MetroTicketBE.Domain.DTO.Payment;
using MetroTicketBE.Domain.Entities;
using MetroTicketBE.Domain.Enum;
using MetroTicketBE.Infrastructure.IRepository;
using MetroTicketBE.WebAPI.Extentions;
using Microsoft.Extensions.Configuration;
using Net.payOS;
using Net.payOS.Types;
using System.Security.Claims;

namespace MetroTicketBE.Application.Service
{
    public class PaymentService : IPaymentService
    {
        private readonly IConfiguration _configuration;
        private readonly PayOS _payos;
        private readonly IUnitOfWork _unitOfWork;

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
        }
        public async Task<ResponseDTO> CreateLinkPaymentTicketRoutePayOS(ClaimsPrincipal user, CreateLinkPaymentRoutePayOSDTO createLinkDTO)
        {
            try
            {
                var userId = "60baa127-8a42-487b-9f7c-470bd56d97b6";

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
                // Gộp các vé trùng lặp và phân loại ra theo tên vé và giá
                var ticketRouteItems = createLinkDTO.TicketRoute?
                    .GroupBy(rt => new { rt.TicketName, rt.Price })
                    .Select(g =>
                    {
                        var firtstItem = g.First();
                        return new ItemData
                        (
                            name: firtstItem.TicketName,
                            price: firtstItem.Price,
                            quantity: g.Count()
                        );
                    });

                var subscriptionTicketItems = createLinkDTO.SubscriptionTickets?
                    .GroupBy(st => new { st.Id })
                    .Select(g =>
                    {
                        var firstItem = g.First();
                        return new ItemData
                        (
                            name: firstItem.TicketName,
                            price: firstItem.Price,
                            quantity: g.Count()
                        );
                    });

                var ticketRouteTotal = ticketRouteItems.Sum(i => i.price * i.quantity);
                var subscriptionTicketTotal = subscriptionTicketItems.Sum(i => i.price * i.quantity);

                // tính tổng giá của các vé

                var discountedTicketRoutePrice = await CalculatePriceApplyPromo(ticketRouteTotal, promotion?.Id);

                var totalPrice = discountedTicketRoutePrice + subscriptionTicketTotal;

                var allItems = ticketRouteItems.Concat(subscriptionTicketItems).ToList();
                // Tạo mã đơn hàng duy nhất dựa trên thời gian hiện tại (orderCode)
                PaymentData paymentLinkRequest = new PaymentData
            (
                orderCode: int.Parse(DateTimeOffset.Now.ToString("ffffff")),
                amount: totalPrice,
                description: createLinkDTO.Description,
                items: allItems,
                returnUrl: "https://youtube.com",
                cancelUrl: "https://facebook.com"
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
                    OrderCode = Convert.ToString(createLinkDTO.OrderCode),
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
                    StatusCode = 200
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

        public async Task<ResponseDTO> UpdatePaymentTickerStatusPayOS(ClaimsPrincipal user, Guid paymentTransactionId)
        {
            try
            {
                var paymentTransaction = await _unitOfWork.PaymentTransactionRepository.GetByIdAsync(paymentTransactionId);
                if (paymentTransaction is null)
                {
                    return new ResponseDTO
                    {
                        Message = "Không tìm thấy giao dịch",
                        IsSuccess = false,
                        StatusCode = 404
                    };
                }
                var oderCode = long.Parse(paymentTransaction.OrderCode ?? throw new Exception("Mã giao dịch không tồn tại"));
                var paymentStatus = _payos.getPaymentLinkInformation(oderCode);

                if (paymentStatus is null)
                {
                    return new ResponseDTO
                    {
                        Message = "Không tìm thấy thông tin giao dịch trên hệ thống PayOS",
                        IsSuccess = false,
                        StatusCode = 404
                    };
                } else
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
                    
                }
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

            decimal discountPercentage = promotion.Percentage / 100m;

            var finalPrice = price * (1 - discountPercentage);

            return (int)finalPrice;
        }
    }
}