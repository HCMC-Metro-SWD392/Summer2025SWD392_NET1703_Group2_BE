using AutoMapper;
using MetroTicketBE.Application.IService;
using MetroTicketBE.Domain.DTO.Auth;
using MetroTicketBE.Domain.DTO.DashBoard;
using MetroTicketBE.Domain.Enum;
using MetroTicketBE.Infrastructure.IRepository;

namespace MetroTicketBE.Application.Service
{
    public class DashBoardService : IDashBoardService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public DashBoardService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
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

        public async Task<ResponseDTO> ViewTicketStatistics(DateTime dateFrom, DateTime dateTo, bool? isAccendingCreated, int pageNumber, int pageSize)
        {
            try
            {
                var transactions = (await _unitOfWork.PaymentTransactionRepository.GetAllAsync(includeProperties: "Customer.User"))
                    .Where(pt => pt.CreatedAt >= dateFrom && pt.CreatedAt <= dateTo);

                if (!transactions.Any())
                {
                    return new ResponseDTO
                    {
                        Message = "Không có giao dịch nào trong khoảng thời gian này.",
                        IsSuccess = false,
                        StatusCode = 404
                    };
                }

                transactions = isAccendingCreated == true
                    ? transactions.OrderBy(pt => pt.CreatedAt)
                    : transactions.OrderByDescending(pt => pt.CreatedAt);

                if (pageNumber < 1 || pageSize < 1)
                {
                    return new ResponseDTO
                    {
                        Message = "Số trang hoặc kích thước trang không hợp lệ.",
                        IsSuccess = false,
                        StatusCode = 400
                    };
                }
                else
                {
                    transactions = transactions.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                }

                var getAllTransactions = _mapper.Map<List<GetTicketStatisticDTO>>(transactions);
                return new ResponseDTO
                {
                    Result = getAllTransactions,
                    Message = "Lấy thống kê vé thành công.",
                    IsSuccess = true,
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO
                {
                    Message = $"Lỗi khi lấy thống kê vé: {ex.Message}",
                    IsSuccess = false,
                    StatusCode = 500
                };
            }
        }

        public async Task<ResponseDTO> ViewTicketRouteStatisticsNumber(DateTime dateFrom, DateTime dateTo, PaymentStatus status)
        {
            try
            {
                var transactions = (await _unitOfWork.PaymentTransactionRepository.GetAllAsync(includeProperties: "Ticket"))
                    .Where(pt => pt.CreatedAt >= dateFrom && pt.CreatedAt <= dateTo && pt.Status == status &&
                    (pt.Ticket.SubscriptionTicket == null || (pt.Ticket.SubscriptionTicket != null && pt.Ticket.TicketRoute != null)));

                var count = transactions.Count();

                return new ResponseDTO
                {
                    Result = count,
                    Message = "Lấy số lượng thống kê vé thành công.",
                    IsSuccess = true,
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO
                {
                    Result = null,
                    Message = $"Lỗi khi lấy số lượng thống kê vé: {ex.Message}",
                    IsSuccess = false,
                    StatusCode = 500
                };
            }
        }

        public async Task<ResponseDTO> ViewSubscriptionTicketStatisticsNumber(DateTime dateFrom, DateTime dateTo, PaymentStatus status)
        {
            try
            {
                var transactions = (await _unitOfWork.PaymentTransactionRepository.GetAllAsync(includeProperties: "Ticket"))
                                    .Where(pt => pt.CreatedAt >= dateFrom && pt.CreatedAt <= dateTo && pt.Status == status &&
                                    pt.Ticket.SubscriptionTicket != null);
                var count = transactions.Select(tr => tr.TicketId).Distinct().Count();
                return new ResponseDTO
                {
                    Result = count,
                    Message = "Lấy số lượng thống kê vé định kỳ thành công.",
                    IsSuccess = true,
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO
                {
                    Result = null,
                    Message = $"Lỗi khi lấy số lượng thống kê vé định kỳ: {ex.Message}",
                    IsSuccess = false,
                    StatusCode = 500
                };
            }
        }

        public async Task<ResponseDTO> ViewCustomerStatisticsNumber()
        {
            try
            {
                var customerCount = (await _unitOfWork.CustomerRepository.GetAllAsync()).Count();
                return new ResponseDTO
                {
                    Result = customerCount,
                    Message = "Lấy số lượng người dùng thành công.",
                    IsSuccess = true,
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO
                {
                    Message = $"Lỗi khi lấy số lượng người dùng: {ex.Message}",
                    IsSuccess = false,
                    StatusCode = 500
                };
            }
        }
    }
}