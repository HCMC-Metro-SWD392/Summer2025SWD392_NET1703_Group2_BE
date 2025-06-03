using MetroTicketBE.Application.IService;
using MetroTicketBE.Domain.DTO.Auth;
using MetroTicketBE.Domain.Entities;
using MetroTicketBE.Domain.Enum;
using MetroTicketBE.Domain.Enums;
using MetroTicketBE.Infrastructure.IRepository;

namespace MetroTicketBE.Application.Service;

public class SubscriptionService: ISubscriptionService
{
    private readonly IUnitOfWork _unitOfWork;
    
public SubscriptionService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }
    public async Task<ResponseDTO> AddSubscriptionAsync(Guid customerId)
    {
        if (customerId == Guid.Empty)
        {
            return new ResponseDTO()
            {
                IsSuccess = false,
                Message = "Mã khách hàng không hợp lệ",
                StatusCode = 400
            };
        }

        Customer? customer = await _unitOfWork.CustomerRepository.GetByIdAsync(customerId);

        if (customer is null)
        {
            return new ResponseDTO()
            {
                IsSuccess = false,
                Message = "Khách hàng không tồn tại",
                StatusCode = 404
            };
        }
        
        int price = customer.CustomerType switch
        {
            CustomerType.Student => 200000,
            CustomerType.OlderPerson => 150000,
            _ => 300000
        };

        try
        {
            var subscription = new SubscriptionTicket()
            {
                TicketName = "Vé Tháng",
                TicketType = TicketType.Monthly,
                Price = price,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddMonths(1)
            };

                await _unitOfWork.SubscriptionRepository.AddAsync(subscription);

            return new ResponseDTO()
            {
                IsSuccess = true,
                Message = "Thêm vé tháng thành công",
                StatusCode = 200,
                Result = subscription
            };
        }
        catch (Exception ex)
        {
            return new ResponseDTO()
            {
                IsSuccess = false,
                Message = $"Lỗi khi thêm vé tháng: {ex.Message}",
                StatusCode = 500
            };
        }
    }
}