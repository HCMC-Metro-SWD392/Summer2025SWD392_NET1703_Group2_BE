using AutoMapper;
using MetroTicketBE.Application.IService;
using MetroTicketBE.Application.Mappings;
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
    private readonly IMapper _mapper;
    private readonly ITicketRouteService _ticketRouteService;
    private readonly IFareRuleRepository _fareRuleRepository;
    
public SubscriptionService(IUnitOfWork unitOfWork, IMapper mapper, ITicketRouteService ticketRouteService, IFareRuleRepository fareRuleRepository)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _ticketRouteService = ticketRouteService ?? throw new ArgumentNullException(nameof(ticketRouteService));
        _fareRuleRepository = fareRuleRepository ?? throw new ArgumentNullException(nameof(fareRuleRepository));
    }

    // public async Task<ResponseDTO> CreateSubscriptionAsync(CreateSubscriptionDTO dto)
    // {
    //     try
    //     {
    //         var isExistSubscription = await _unitOfWork.SubscriptionRepository.IsExistedByType(dto.TicketType);
    //         if (isExistSubscription)
    //         {
    //             return new ResponseDTO()
    //             {
    //                 IsSuccess = false,
    //                 StatusCode = 400,
    //                 Message = "Loại vé đã tồn tại"
    //             };
    //         }
    //             
    //         SubscriptionTicket subscriptionTicket = new SubscriptionTicket
    //         {
    //             TicketName = dto.TicketName,
    //             TicketType = dto.TicketType,
    //             Price = dto.Price, 
    //         };
    //         if (dto.TicketType == SubscriptionTicketType.Monthly)
    //         {
    //             subscriptionTicket.Expiration = 30;
    //         }
    //         else if (dto.TicketType == SubscriptionTicketType.Yearly)
    //         {
    //             subscriptionTicket.Expiration = 365;
    //         }else if (dto.TicketType == SubscriptionTicketType.Daily)
    //         {
    //             subscriptionTicket.Expiration = 1;
    //         }else if(dto.TicketType == SubscriptionTicketType.Weekly)
    //         {
    //             subscriptionTicket.Expiration = 7;
    //         }
    //         else if (dto.TicketType is SubscriptionTicketType.Student)
    //         {
    //             subscriptionTicket.Expiration = 30;
    //         }
    //         else if (dto.TicketType is SubscriptionTicketType.Elder or SubscriptionTicketType.Military)
    //         {
    //             subscriptionTicket.Expiration = int.MaxValue; // Vé dành cho người cao tuổi hoặc quân đội có thời hạn sử dụng vô hạn
    //         }
    //         else
    //         {
    //             return new ResponseDTO()
    //             {
    //                 IsSuccess = false,
    //                 StatusCode = 400,
    //                 Message = "Loại vé không hợp lệ"
    //             };
    //         }
    //         await _unitOfWork.SubscriptionRepository.AddAsync(subscriptionTicket);
    //         await _unitOfWork.SaveAsync();
    //         return new ResponseDTO()
    //         {
    //             IsSuccess = true,
    //             StatusCode = 200,
    //             Message = "Tạo vé thành công",
    //             Result = subscriptionTicket
    //         };
    //         
    //     }
    //     catch (Exception ex)
    //     {
    //         return new ResponseDTO()
    //         {
    //             IsSuccess = false,
    //             StatusCode = 500,
    //             Message = "Lỗi khi tạo vé: " + ex.Message
    //         };
    //     }
    // }

    public async Task<ResponseDTO> CreateSubscriptionTicketAsync(CreateSubscriptionDTO createSubscriptionDTO)
    {
        var existSubscription = await _unitOfWork.SubscriptionRepository.GetByStartAndEndStationAsync(createSubscriptionDTO.StartStationId, createSubscriptionDTO.EndStationId);
        var existByTicketType = await _unitOfWork.SubscriptionRepository.GetByTicketTypeIdAsync(createSubscriptionDTO.TicketTypeId);
        var ticketType = await _unitOfWork.SubscriptionTicketTypeRepository.GetAsync(t => t.Id == createSubscriptionDTO.TicketTypeId);

        if (existSubscription is not null && existByTicketType is not null)
        {
            return new ResponseDTO()
            {
                IsSuccess = false,
                StatusCode = 409,
                Message = "Vé đã tồn tại cho tuyến đường này và loại vé này"
            };
        }
        double distance = await _ticketRouteService.CalculateDistanceOfTwoStation(createSubscriptionDTO.StartStationId, createSubscriptionDTO.EndStationId);

        var startStation = await _unitOfWork.StationRepository.GetNameById(createSubscriptionDTO.StartStationId);
        if (string.IsNullOrEmpty(startStation))
        {
            return new ResponseDTO
            {
                Message = "Trạm bắt đầu không hợp lệ.",
                IsSuccess = false,
                StatusCode = 400
            };
        }

        var endStation = await _unitOfWork.StationRepository.GetNameById(createSubscriptionDTO.EndStationId);

        if (string.IsNullOrEmpty(endStation))
        {
            return new ResponseDTO
            {
                Message = "Trạm kết thúc không hợp lệ.",
                IsSuccess = false,
                StatusCode = 400
            };
        }
        if (ticketType == null)
        {
            return new ResponseDTO()
            {
                IsSuccess = false,
                StatusCode = 404,
                Message = "Loại vé không hợp lệ"
            };
        }
        var price = await _fareRuleRepository.CalculatePriceFromDistance(distance) * ticketType.FareCoefficient;
        var saveSubscriptionTicket = new SubscriptionTicket()
        {
            TicketName = $"Vé {ticketType.DisplayName} từ {startStation} đến {endStation}",
            StartStationId = createSubscriptionDTO.StartStationId,
            EndStationId = createSubscriptionDTO.EndStationId,
            Expiration = ticketType.Expiration,
            Price = price,
            TicketTypeId = createSubscriptionDTO.TicketTypeId,
        };
        
        await _unitOfWork.SubscriptionRepository.AddAsync(saveSubscriptionTicket);
        await _unitOfWork.SaveAsync();
        return new ResponseDTO()
        {
            IsSuccess = true,
            StatusCode = 201,
            Message = "Tạo vé thành công",
            Result = _mapper.Map<GetSubscriptionTicketDTO>(saveSubscriptionTicket)
        };
    }

    public async Task<ResponseDTO> GetAllSubscriptionsAsync()
    {
        try
        {
            var subscriptions = await _unitOfWork.SubscriptionRepository.GetAllAsync();
            if (subscriptions == null || !subscriptions.Any())
            {
                return new ResponseDTO()
                {
                    IsSuccess = false,
                    StatusCode = 404,
                    Message = "Không tìm thấy vé nào"
                };
            }

            var subscriptionDTO = _mapper.Map<List<GetSubscriptionTicketDTO>>(subscriptions);
            return new ResponseDTO()
            {
                IsSuccess = true,
                StatusCode = 200,
                Message = "Lấy danh sách vé thành công",
                Result = subscriptionDTO
            };
            
        }catch (Exception ex)
        {
            return new ResponseDTO()
            {
                IsSuccess = false,
                StatusCode = 500,
                Message = "Lỗi khi lấy danh sách vé: " + ex.Message
            };
        }
    }
    
    public async Task<ResponseDTO> UpdateSubscriptionAsync(Guid id, UpdateSubscriptionDTO dto)
    {
        try
        {
            var subscription = await _unitOfWork.SubscriptionRepository.GetAsync(s => s.Id == id);
            if (subscription == null)
            {
                return new ResponseDTO()
                {
                    IsSuccess = false,
                    StatusCode = 404,
                    Message = "Không tìm thấy vé"
                };
            }
            if (dto.TicketName != null && dto.TicketName.Trim().Length > 0) 
            {
                var isExistSubscription = await _unitOfWork.SubscriptionRepository.IsExistedByName(dto.TicketName);
                if (isExistSubscription)
                {
                    return new ResponseDTO()
                    {
                        IsSuccess = false,
                        StatusCode = 400,
                        Message = "Tên vé đã tồn tại"
                    };
                }
                subscription.TicketName = dto.TicketName;
            }
            if (dto.Price is > 0)
            {
                subscription.Price = dto.Price.Value;
            }
            
            _unitOfWork.SubscriptionRepository.Update(subscription);
            await _unitOfWork.SaveAsync();
            
            return new ResponseDTO()
            {
                IsSuccess = true,
                StatusCode = 200,
                Message = "Cập nhật vé thành công",
                Result = subscription
            };
        }
        catch (Exception ex)
        {
            return new ResponseDTO()
            {
                IsSuccess = false,
                StatusCode = 500,
                Message = "Lỗi khi cập nhật vé: " + ex.Message
            };
        }
    }
    
    public async Task<ResponseDTO> GetSubscriptionAsync(Guid id)
    {
        try
        {
            var subscription = await _unitOfWork.SubscriptionRepository.GetAsync(s => s.Id == id);
            if (subscription == null)
            {
                return new ResponseDTO()
                {
                    IsSuccess = false,
                    StatusCode = 404,
                    Message = "Không tìm thấy vé"
                };
            }
            return new ResponseDTO()
            {
                IsSuccess = true,
                StatusCode = 200,
                Message = "Lấy vé thành công",
                Result = subscription
            };
        }
        catch (Exception ex)
        {
            return new ResponseDTO()
            {
                IsSuccess = false,
                StatusCode = 500,
                Message = "Lỗi khi lấy vé: " + ex.Message
            };
        }
    }

}