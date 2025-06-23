using MetroTicketBE.Application.IService;
using MetroTicketBE.Domain.DTO.Auth;
using MetroTicketBE.Infrastructure.IRepository;

namespace MetroTicketBE.Application.Service
{
    public class PaymentTransactionService : IPaymentTransactionService
    {
        private readonly IUnitOfWork _unitOfWork;
        public PaymentTransactionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }
        public async Task<ResponseDTO> ViewRevenueMonth(int month)
        {
            try
            {
                if (month < 1 || month > 12)
                {
                    return new ResponseDTO
                    {
                        Message = "Tháng không hợp lệ. Vui lòng nhập tháng từ 1 đến 12.",
                        IsSuccess = false,
                        StatusCode = 400
                    };
                }

                if (month > DateTime.Now.Month)
                {
                    return new ResponseDTO
                    {
                        Message = "Chỉ có thể xem doanh thu của tháng thuộc năm hiện tại.",
                        IsSuccess = false,
                        StatusCode = 400
                    };
                }

                var revenue = (await _unitOfWork.PaymentTransactionRepository.GetAllAsync())
                    .Where(pt => pt.CreatedAt.Month == month && pt.CreatedAt.Year == DateTime.Now.Year);

                if (!revenue.Any())
                {
                    return new ResponseDTO
                    {
                        Message = "Không có giao dịch nào trong tháng này.",
                        IsSuccess = false,
                        StatusCode = 404
                    };
                }

                var totalRevenue = revenue.Sum(pt => pt.TotalPrice);
                return new ResponseDTO
                {
                    Result = totalRevenue,
                    Message = "Lấy tổng doanh thu tháng thành công.",
                    IsSuccess = true,
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO
                {
                    Result = null,
                    Message = $"Lỗi khi lấy tổng doanh thu tháng: {ex.Message}",
                    IsSuccess = false,
                    StatusCode = 500
                };
            }
        }

        public async Task<ResponseDTO> ViewRevenueOverTime(DateTime dateFrom, DateTime dateTo)
        {
            try
            {
                var revenue = (await _unitOfWork.PaymentTransactionRepository.GetAllAsync())
                    .Where(pt => pt.CreatedAt >= dateFrom && pt.CreatedAt <= dateTo);

                if (!revenue.Any())
                {
                    return new ResponseDTO
                    {
                        Message = "Không có giao dịch nào trong khoảng thời gian này.",
                        IsSuccess = false,
                        StatusCode = 404
                    };
                }

                var totalRevenue = revenue.Sum(pt => pt.TotalPrice);

                return new ResponseDTO
                {
                    Result = totalRevenue,
                    Message = "Lấy tổng doanh thu thành công .",
                    IsSuccess = true,
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO
                {
                    Result = null,
                    Message = $"Lỗi khi lấy tổng doanh thu: {ex.Message}",
                    IsSuccess = false,
                    StatusCode = 500
                };
            }
        }

        public async Task<ResponseDTO> ViewRevenueYear(int year)
        {
            try
            {
                var revenue = (await _unitOfWork.PaymentTransactionRepository.GetAllAsync())
                    .Where(pt => pt.CreatedAt.Year == year);

                if (!revenue.Any())
                {
                    return new ResponseDTO
                    {
                        Message = "Không có giao dịch nào trong năm này.",
                        IsSuccess = false,
                        StatusCode = 404
                    };
                }

                var totalRevenue = revenue.Sum(pt => pt.TotalPrice);
                return new ResponseDTO
                {
                    Result = totalRevenue,
                    Message = "Lấy tổng doanh thu năm thành công.",
                    IsSuccess = true,
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO
                {
                    Result = null,
                    Message = $"Lỗi khi lấy tổng doanh thu năm: {ex.Message}",
                    IsSuccess = false,
                    StatusCode = 500
                };
            }
        }
    }
}
