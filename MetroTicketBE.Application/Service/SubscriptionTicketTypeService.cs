using MetroTicketBE.Application.IService;
using MetroTicketBE.Domain.DTO.Auth;
using MetroTicketBE.Domain.DTO.SubscriptionTicketType;
using MetroTicketBE.Domain.Entities;
using MetroTicketBE.Infrastructure.IRepository;

namespace MetroTicketBE.Application.Service;

public class SubscriptionTicketTypeService: ISubscriptionTicketTypeService
{
    private readonly IUnitOfWork _unitOfWork;
    public SubscriptionTicketTypeService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<ResponseDTO> GetByNameAsync(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Name cannot be null or empty.", nameof(name));
        }
        var subscriptionTicketType = await _unitOfWork.SubscriptionTicketTypeRepository.GetByNameAsync(name);
        if (subscriptionTicketType is null)
        {
            return new ResponseDTO
            {
                IsSuccess = false,
                Message = "Subscription ticket type not found.",
                StatusCode = 404
            };
        }
        return new ResponseDTO
        {
            IsSuccess = true,
            Message = "Subscription ticket type found.",
            StatusCode = 200,
            Result = subscriptionTicketType
        };
        
    }

    public async Task<ResponseDTO> GetByIdAsync(Guid id)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id cannot be empty.", nameof(id));
        }
        var subscriptionTicketType = await _unitOfWork.SubscriptionTicketTypeRepository.GetAsync(stt => stt.Id == id);
        return new ResponseDTO()
        {
            IsSuccess = true,
            Message = "Subscription ticket type found.",
            StatusCode = 200,
            Result = subscriptionTicketType
        };
    }
    
    public async Task<ResponseDTO> GetAllAsync()
    {
        var subscriptionTicketTypes = await _unitOfWork.SubscriptionTicketTypeRepository.GetAllAsync();
        if (subscriptionTicketTypes is null || !subscriptionTicketTypes.Any())
        {
            return new ResponseDTO
            {
                IsSuccess = false,
                Message = "No subscription ticket types found.",
                StatusCode = 404
            };
        }
        return new ResponseDTO
        {
            IsSuccess = true,
            Message = "Subscription ticket types retrieved successfully.",
            StatusCode = 200,
            Result = subscriptionTicketTypes
        };
    }
    
    public async Task<ResponseDTO> CreateAsync(CreateSubscriptionTicketTypeDTO createSubscriptionTicketTypeDTO)
    {
        if (createSubscriptionTicketTypeDTO is null)
        {
            throw new ArgumentNullException(nameof(createSubscriptionTicketTypeDTO), "CreateSubscriptionTicketTypeDTO cannot be null.");
        }
        
        var subscriptionTicketType = new SubscriptionTicketType
        {
            Name = createSubscriptionTicketTypeDTO.Name,
            DisplayName = createSubscriptionTicketTypeDTO.DisplayName,
            Expiration = createSubscriptionTicketTypeDTO.Expiration,
            FareCoefficient = createSubscriptionTicketTypeDTO.FareCoefficient
        };
        
        await _unitOfWork.SubscriptionTicketTypeRepository.AddAsync(subscriptionTicketType);
        await _unitOfWork.SaveAsync();
        
        
        return new ResponseDTO
        {
            IsSuccess = true,
            Message = "Subscription ticket type created successfully.",
            StatusCode = 201,
            Result = subscriptionTicketType
        };
    }

}