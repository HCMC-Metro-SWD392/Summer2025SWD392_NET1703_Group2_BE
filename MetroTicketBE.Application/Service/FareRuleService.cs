using MetroTicketBE.Application.IService;
using MetroTicketBE.Domain.DTO.Auth;
using MetroTicketBE.Domain.DTO.FareRule;
using MetroTicketBE.Domain.Entities;
using MetroTicketBE.Infrastructure.IRepository;

namespace MetroTicketBE.Application.Service
{
    public class FareRuleService : IFareRuleService
    {
        private readonly IUnitOfWork _unitOfWork;
        public FareRuleService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }
        public async Task<ResponseDTO> CreateFareRule(CreateFareRuleDTO createFareRuleDTO)
        {
            try
            {
                await CheckValidDistance(createFareRuleDTO.MinDistance, createFareRuleDTO.MaxDistance);

                FareRule fareRule = new FareRule
                {
                    MaxDistance = createFareRuleDTO.MaxDistance,
                    MinDistance = createFareRuleDTO.MinDistance,
                    Fare = createFareRuleDTO.Fare
                };

                if (fareRule is null)
                {
                    return new ResponseDTO
                    {
                        IsSuccess = false,
                        Message = "FareRule không hợp lệ",
                        StatusCode = 400
                    };
                }

                await _unitOfWork.FareRuleRepository.AddAsync(fareRule);
                await _unitOfWork.SaveAsync();

                return new ResponseDTO
                {
                    IsSuccess = true,
                    Message = "Tạo fare rule thành công",
                    StatusCode = 201,
                    Result = fareRule
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO
                {
                    Message = $"Đã xảy ra lỗi: {ex.Message}",
                    IsSuccess = false,
                    StatusCode = 500
                };
            }
        }

        private async Task<bool> CheckValidDistance(double minDistance, double maxDistance)
        {
            if (minDistance >= maxDistance)
            {
                throw new Exception("Khoảng cách tối thiểu phải nhỏ hơn khoảng cách tối đa");
            }

            var isExistMin = await _unitOfWork.FareRuleRepository
                 .GetAsync(f => f.MaxDistance >= minDistance);

            if (isExistMin is not null)
            {
                throw new Exception("Khoảng cách mới chồng lấn với khoảng cách đã tồn tại");
            }

            return true;
        }

    }
}
