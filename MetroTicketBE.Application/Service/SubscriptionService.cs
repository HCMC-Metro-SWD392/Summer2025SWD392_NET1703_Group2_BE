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

    public async Task<ResponseDTO> CreateSubscriptionAsync(CreateSubscriptionDTO dto)
    {
        try
        {
            var isExistSubscription = await _unitOfWork.SubscriptionRepository.IsExistedByType(dto.TicketType);
            if (isExistSubscription)
            {
                return new ResponseDTO()
                {
                    IsSuccess = false,
                    StatusCode = 400,
                    Message = "Loại vé đã tồn tại"
                };
            }

            SubscriptionTicket subscriptionTicket = new SubscriptionTicket
            {
                TicketName = dto.TicketName,
                TicketType = dto.TicketType,
                Price = dto.Price, 
            };
            if (dto.TicketType == SubscriptionTicketType.Monthly)
            {
                subscriptionTicket.Expiration = 30;
            }
            else if (dto.TicketType == SubscriptionTicketType.Yearly)
            {
                subscriptionTicket.Expiration = 365;
            }else if (dto.TicketType == SubscriptionTicketType.Daily)
            {
                subscriptionTicket.Expiration = 1;
            }else if(dto.TicketType == SubscriptionTicketType.Weekly)
            {
                subscriptionTicket.Expiration = 7;
            }
            else
            {
                return new ResponseDTO()
                {
                    IsSuccess = false,
                    StatusCode = 400,
                    Message = "Loại vé không hợp lệ"
                };
            }
            await _unitOfWork.SubscriptionRepository.AddAsync(subscriptionTicket);
            await _unitOfWork.SaveAsync();
            return new ResponseDTO()
            {
                IsSuccess = true,
                StatusCode = 200,
                Message = "Tạo vé thành công",
                Result = subscriptionTicket
            };
            
        }
        catch (Exception ex)
        {
            return new ResponseDTO()
            {
                IsSuccess = false,
                StatusCode = 500,
                Message = "Lỗi khi tạo vé: " + ex.Message
            };
        }
    }

}