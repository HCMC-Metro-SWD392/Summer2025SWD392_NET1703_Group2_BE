using MetroTicketBE.Application.IService;
using MetroTicketBE.Domain.DTO.Auth;
using MetroTicketBE.Domain.DTO.SubscriptionTicket;
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

    public Task<ResponseDTO> CreateSubscriptionAsync(CreateSubscriptionDTO dto)
    {
        try
        {


            SubscriptionTicket subscriptionTicket = new SubscriptionTicket
            {
            };

            _unitOfWork.SubscriptionRepository.AddAsync(subscriptionTicket);
            _unitOfWork.SaveAsync();

            return Task.FromResult(new ResponseDTO
            {
                IsSuccess = true,
                StatusCode = 200,
                Message = "Đăng ký vé tháng thành công",
                Result = subscriptionTicket
            });
        }
        catch (Exception ex)
        {
            return Task.FromResult(new ResponseDTO
            {
                IsSuccess = false,
                StatusCode = 500,
                Message = "Lỗi khi đăng ký vé tháng: " + ex.Message
            });
        }
    }

}