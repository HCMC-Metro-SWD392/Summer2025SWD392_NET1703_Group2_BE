using MetroTicket.Domain.Entities;
using MetroTicketBE.Application.IService;
using MetroTicketBE.Domain.Constants;
using MetroTicketBE.Domain.DTO.Auth;
using MetroTicketBE.Domain.DTO.Payos;
using MetroTicketBE.Domain.Entities;
using MetroTicketBE.Domain.Enum;
using MetroTicketBE.Infrastructure.IRepository;
using MetroTicketBE.Infrastructure.Repository;
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
        private readonly StationGraph _stationGraph;
        private readonly IUnitOfWork _unitOfWork;

        public PaymentService
        (
            IConfiguration configuration,
            PayOS payos,
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
            _stationGraph = stationGraph ?? throw new ArgumentNullException(nameof(stationGraph));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }
        public async Task<ResponseDTO> CreateLinkPaymentTicketRoute(ClaimsPrincipal user, CreateLinkPaymentRouteDTO createLinkDTO)
        {
            try
            {
                var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);

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

                var promotion = await _unitOfWork.PromotionRepository.GetByCodeAsync(createLinkDTO.CodePromotion);

                if (promotion is null)
                {
                    return new ResponseDTO
                    {
                        Message = "Không tìm thấy mã khuyến mãi",
                        IsSuccess = false,
                        StatusCode = 404
                    };
                }
                // Gộp các vé trùng lặp và phân loại ra theo tên vé và giá
                var items = createLinkDTO.Ticket
                .GroupBy(rt => new { rt.TicketName, rt.Price })
                .Select(g => new ItemData
                (
                    name: g.Key.TicketName,
                    quantity: g.Count(),
                    price: g.Key.Price
                )).ToList();

                // tính tổng giá của các vé
                var totalPrice = items.Sum(i => i.price * i.quantity);

                var finalPrice = await CalculatePriceApplyPromo(totalPrice, promotion.Id);

                // Tạo mã đơn hàng duy nhất dựa trên thời gian hiện tại (orderCode)
                var paymentLinkRequest = new PaymentData
            (
                orderCode: int.Parse(DateTimeOffset.Now.ToString("ffffff")),
                amount: finalPrice,
                description: createLinkDTO.Description,
                items: items,
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
                    TotalPrice = finalPrice,
                    PromotionId = promotion.Id,
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

        private async Task<double> CalculateDistanceOfTwoStation(Guid startStationId, Guid endStationId)
        {
            try
            {
                var allMetroLines = await _unitOfWork.MetroLineRepository.GetAllListAsync();
                var stationPath = _stationGraph.FindShortestPath(startStationId, endStationId);
                double distance = _unitOfWork.StationRepository.CalculateTotalDistance(stationPath, allMetroLines);

                return distance;
            }
            catch (Exception ex)
            {
                throw new Exception("Đã xảy ra lỗi tính khoảng cách: ", ex);
            }
        }

        private async Task<double> CalculateTicketRouteAndSave(Guid startStationId, Guid endStationId)
        {
            try
            {
                double distance = await CalculateDistanceOfTwoStation(startStationId, endStationId);

                int price = (await _unitOfWork.FareRuleRepository.GetAllAsync())
                    .Where(fr => fr.MinDistance <= distance && fr.MaxDistance >= distance)
                    .Select(fr => fr.Fare)
                    .FirstOrDefault();

                var saveTicketRoute = new TicketRoute
                {
                    StartStationId = startStationId,
                    EndStationId = endStationId,
                    Distance = distance,
                    Price = price
                };

                await _unitOfWork.TicketRouteRepository.AddAsync(saveTicketRoute);

                return price;
            }
            catch (Exception ex)
            {
                throw new Exception("Đã xảy ra lỗi tính giá vé: ", ex);
            }
        }

        private async Task<int> CalculatePriceApplyPromo(int price, Guid promotionId)
        {
            // Kiểm tra xem có tồn tại mã khuyến mãi không
            var promotion = await _unitOfWork.PromotionRepository.GetByIdAsync(promotionId);

            if (promotion is null)
            {
                return price;
            }

            decimal discountPercentage = promotion.Percentage / 100m;

            var finalPrice = price * (1 - promotion.Percentage);

            return (int)finalPrice;
        }
    }
}