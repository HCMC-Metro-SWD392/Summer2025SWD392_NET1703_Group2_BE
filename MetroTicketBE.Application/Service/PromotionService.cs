using AutoMapper;
using MetroTicketBE.Application.IService;
using MetroTicketBE.Domain.DTO.Auth;
using MetroTicketBE.Domain.DTO.Promotion;
using MetroTicketBE.Domain.Entities;
using MetroTicketBE.Domain.Enums;
using MetroTicketBE.Infrastructure.IRepository;
using System.Security.Claims;

namespace MetroTicketBE.Application.Service
{
    public class PromotionService : IPromotionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public PromotionService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        public async Task<ResponseDTO> CreatePromotion(CreatePromotionDTO createPromotionDTO)
        {
            try
            {
                var isExistPromotion = await _unitOfWork.PromotionRepository.IsExistByCode(createPromotionDTO.Code);
                if (isExistPromotion is true)
                {
                    return new ResponseDTO
                    {
                        IsSuccess = false,
                        StatusCode = 404,
                        Message = "Mã giảm giá đã tồn tại"
                    };
                }

                if (createPromotionDTO.StartDate >= createPromotionDTO.EndDate)
                {
                    return new ResponseDTO
                    {
                        IsSuccess = false,
                        StatusCode = 400,
                        Message = "Ngày bắt đầu phải trước ngày kết thúc"
                    };
                }

                // Ngày bắt đầu và kết thúc không thể là quá khứ  
                if (createPromotionDTO.StartDate < DateTime.UtcNow || createPromotionDTO.EndDate < DateTime.UtcNow)
                {
                    return new ResponseDTO
                    {
                        IsSuccess = false,
                        StatusCode = 400,
                        Message = "Ngày bắt đầu và kết thúc không thể là quá khứ"
                    };
                }

                if (createPromotionDTO.PromotionType == PromotionType.Percentage)
                {
                    if (createPromotionDTO.Percentage is null || createPromotionDTO.Percentage < 0 || createPromotionDTO.Percentage > 100)
                    {
                        return new ResponseDTO
                        {
                            IsSuccess = false,
                            StatusCode = 400,
                            Message = "Tỷ lệ phần trăm phải nằm trong khoảng từ 0 đến 100"
                        };
                    }
                }
                else if (createPromotionDTO.PromotionType == PromotionType.FixedAmount)
                {
                    if (createPromotionDTO.FixedAmount <= 0)
                    {
                        return new ResponseDTO
                        {
                            IsSuccess = false,
                            StatusCode = 400,
                            Message = "Số tiền cố định phải lớn hơn 0"
                        };
                    }
                }

                Promotion promotion = new Promotion
                {
                    Code = createPromotionDTO.Code,
                    Percentage = createPromotionDTO?.Percentage,
                    FixedAmount = createPromotionDTO?.FixedAmount,
                    PromotionType = createPromotionDTO.PromotionType,
                    StartDate = createPromotionDTO.StartDate,
                    EndDate = createPromotionDTO.EndDate,
                    Description = createPromotionDTO.Description
                };

                if (promotion is null)
                {
                    return new ResponseDTO
                    {
                        IsSuccess = false,
                        StatusCode = 400,
                        Message = "Thông tin mã giảm giá không hợp lệ"
                    };
                }

                await _unitOfWork.PromotionRepository.AddAsync(promotion);
                await _unitOfWork.SaveAsync();

                return new ResponseDTO
                {
                    IsSuccess = true,
                    StatusCode = 201,
                    Message = "Tạo mã giảm giá thành công",
                    Result = promotion
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO
                {
                    IsSuccess = false,
                    StatusCode = 500,
                    Message = $"Lỗi khi tạo mã giảm giá: {ex.Message}"
                };
            }

        }

        public async Task<ResponseDTO> GetAll(ClaimsPrincipal user, string? filterOn, string? filterQuery, string? sortBy, bool? isAcsending, int pageNumber, int pageSize)
        {
            try
            {
                var promotions = await _unitOfWork.PromotionRepository.GetAllAsync();

                if (!string.IsNullOrEmpty(filterOn) && !string.IsNullOrEmpty(filterQuery))
                {
                    string filter = filterOn.ToLower();
                    string query = filterQuery.ToUpper();

                    promotions = filter switch
                    {
                        "code" => promotions.Where(p => p.Code.Contains(query, StringComparison.CurrentCulture)),

                        _ => promotions
                    };
                }

                if (!string.IsNullOrEmpty(sortBy))
                {
                    promotions = sortBy.Trim().ToLower() switch
                    {
                        "code" => isAcsending == true ? promotions.OrderBy(p => p.Code) : promotions.OrderByDescending(p => p.Code),
                        "startdate" => isAcsending == true ? promotions.OrderBy(p => p.StartDate) : promotions.OrderByDescending(p => p.StartDate),
                        "enddate" => isAcsending == true ? promotions.OrderBy(p => p.EndDate) : promotions.OrderByDescending(p => p.EndDate),
                        _ => promotions
                    };
                }

                if (pageNumber <= 0 || pageSize <= 0)
                {
                    return new ResponseDTO
                    {
                        IsSuccess = false,
                        StatusCode = 400,
                        Message = "Số trang và kích thước trang phải lớn hơn 0"
                    };
                }
                else
                {
                    promotions = promotions.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                }

                var getPromotions = _mapper.Map<List<GetPromotionDTO>>(promotions);

                if (promotions is null || !promotions.Any())
                {
                    return new ResponseDTO
                    {
                        IsSuccess = false,
                        StatusCode = 404,
                        Message = "Không tìm thấy mã giảm giá nào"
                    };
                }

                return new ResponseDTO
                {
                    IsSuccess = true,
                    StatusCode = 200,
                    Message = "Lấy danh sách mã giảm giá thành công",
                    Result = promotions
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO
                {
                    IsSuccess = false,
                    StatusCode = 500,
                    Message = $"Lỗi khi lấy danh sách mã giảm giá: {ex.Message}"
                };
            }
        }

        public async Task<ResponseDTO> UpdatePromotion(UpdatePromotionDTO updatePromotionDTO)
        {
            try
            {
                var promotion = await _unitOfWork.PromotionRepository.GetByIdAsync(updatePromotionDTO.Id);
                if (promotion is null)
                {
                    return new ResponseDTO
                    {
                        IsSuccess = false,
                        StatusCode = 404,
                        Message = "Mã giảm giá không tồn tại"
                    };
                }
                var isExistPromotion = await _unitOfWork.PromotionRepository.IsExistByCode(updatePromotionDTO.Code);
                if (isExistPromotion is true)
                {
                    return new ResponseDTO
                    {
                        IsSuccess = false,
                        StatusCode = 404,
                        Message = "Mã giảm giá đã tồn tại"
                    };
                }

                if (updatePromotionDTO.StartDate >= updatePromotionDTO.EndDate)
                {
                    return new ResponseDTO
                    {
                        IsSuccess = false,
                        StatusCode = 400,
                        Message = "Ngày bắt đầu phải trước ngày kết thúc"
                    };
                }
                // Ngày bắt đầu và kết thúc không thể là quá khứ  
                if (updatePromotionDTO.StartDate < DateTime.UtcNow || updatePromotionDTO.EndDate < DateTime.UtcNow)
                {
                    return new ResponseDTO
                    {
                        IsSuccess = false,
                        StatusCode = 400,
                        Message = "Ngày bắt đầu và kết thúc không thể là quá khứ"
                    };
                }
                if (updatePromotionDTO.PromotionType == PromotionType.Percentage)
                {
                    if (updatePromotionDTO.Percentage is null || updatePromotionDTO.Percentage < 0 || updatePromotionDTO.Percentage > 100)
                    {
                        return new ResponseDTO
                        {
                            IsSuccess = false,
                            StatusCode = 400,
                            Message = "Tỷ lệ phần trăm phải nằm trong khoảng từ 0 đến 100"
                        };
                    }
                }
                else if (updatePromotionDTO.PromotionType == PromotionType.FixedAmount)
                {
                    if (updatePromotionDTO.FixedAmount <= 0)
                    {
                        return new ResponseDTO
                        {
                            IsSuccess = false,
                            StatusCode = 400,
                            Message = "Số tiền cố định phải lớn hơn 0"
                        };
                    }
                }

                if (updatePromotionDTO.Code != default)
                {
                    promotion.Code = updatePromotionDTO.Code.Trim().ToUpper();
                }

                if (updatePromotionDTO.Description != default)
                {
                    promotion.Description = updatePromotionDTO.Description;
                }

                if (updatePromotionDTO.StartDate != default && updatePromotionDTO.StartDate > DateTime.MinValue)
                {
                    promotion.StartDate = updatePromotionDTO.StartDate;
                }

                if (updatePromotionDTO.EndDate != default && updatePromotionDTO.EndDate > DateTime.MinValue)
                {
                    promotion.EndDate = updatePromotionDTO.EndDate;
                }

                if (updatePromotionDTO.PromotionType != default)
                {
                    promotion.PromotionType = updatePromotionDTO.PromotionType;
                }

                if (updatePromotionDTO.Percentage != default)
                {
                    promotion.Percentage = updatePromotionDTO.Percentage;
                }

                if (updatePromotionDTO.FixedAmount != default)
                {
                    promotion.FixedAmount = updatePromotionDTO.FixedAmount;
                }

                _unitOfWork.PromotionRepository.Update(promotion);
                await _unitOfWork.SaveAsync();

                return new ResponseDTO
                {
                    IsSuccess = true,
                    StatusCode = 200,
                    Message = "Cập nhật mã giảm giá thành công",
                    Result = promotion
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO
                {
                    IsSuccess = false,
                    StatusCode = 500,
                    Message = $"Lỗi khi cập nhật mã giảm giá: {ex.Message}"
                };
            }
        }
    }


}
