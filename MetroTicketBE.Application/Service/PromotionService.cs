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

        public async Task<ResponseDTO> DeletePromotion(Guid promotionId)
        {
            try
            {
                var promotion = await _unitOfWork.PromotionRepository.GetByIdAsync(promotionId);
                if (promotion is null)
                {
                    return new ResponseDTO
                    {
                        IsSuccess = false,
                        StatusCode = 404,
                        Message = "Mã giảm giá không tồn tại"
                    };
                }

                _unitOfWork.PromotionRepository.Remove(promotion);
                await _unitOfWork.SaveAsync();
                return new ResponseDTO
                {
                    IsSuccess = true,
                    StatusCode = 200,
                    Message = "Xóa mã giảm giá thành công"
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO
                {
                    IsSuccess = false,
                    StatusCode = 500,
                    Message = $"Lỗi khi xóa mã giảm giá: {ex.Message}"
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
                        "createdat" => isAcsending == true ? promotions.OrderBy(p => p.CreatedAt) : promotions.OrderByDescending(p => p.CreatedAt),
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

        public async Task<ResponseDTO> GetPromotionByCode(string code)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(code))
                {
                    return new ResponseDTO
                    {
                        IsSuccess = false,
                        StatusCode = 400,
                        Message = "Mã giảm giá không được để trống"
                    };
                }

                var promotion = await _unitOfWork.PromotionRepository.GetByCodeAsync(code);
                if (promotion is null)
                {
                    return new ResponseDTO
                    {
                        IsSuccess = false,
                        StatusCode = 404,
                        Message = "Mã giảm giá không tồn tại"
                    };
                }
                var getPromotion = _mapper.Map<GetPromotionDTO>(promotion);
                return new ResponseDTO
                {
                    IsSuccess = true,
                    StatusCode = 200,
                    Message = "Lấy mã giảm giá thành công",
                    Result = getPromotion
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO
                {
                    IsSuccess = false,
                    StatusCode = 500,
                    Message = $"Lỗi khi lấy mã giảm giá: {ex.Message}"
                };
            }
        }

        public async Task<ResponseDTO> GetPromotionById(Guid id)
        {
            try
            {
                var promotion = await _unitOfWork.PromotionRepository.GetByIdAsync(id);
                if (promotion is null)
                {
                    return new ResponseDTO
                    {
                        IsSuccess = false,
                        StatusCode = 404,
                        Message = "Mã giảm giá không tồn tại"
                    };
                }

                var getPromotion = _mapper.Map<GetPromotionDTO>(promotion);
                return new ResponseDTO
                {
                    IsSuccess = true,
                    StatusCode = 200,
                    Message = "Lấy mã giảm giá thành công",
                    Result = getPromotion
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO
                {
                    IsSuccess = false,
                    StatusCode = 500,
                    Message = $"Lỗi khi lấy mã giảm giá: {ex.Message}"
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

                // Validate ngày bắt đầu và kết thúc nếu cả hai được cung cấp
                if (updatePromotionDTO.StartDate.HasValue && updatePromotionDTO.EndDate.HasValue)
                {
                    if (updatePromotionDTO.StartDate >= updatePromotionDTO.EndDate)
                    {
                        return new ResponseDTO
                        {
                            IsSuccess = false,
                            StatusCode = 400,
                            Message = "Ngày bắt đầu phải trước ngày kết thúc"
                        };
                    }

                    if (updatePromotionDTO.StartDate < DateTime.UtcNow || updatePromotionDTO.EndDate < DateTime.UtcNow)
                    {
                        return new ResponseDTO
                        {
                            IsSuccess = false,
                            StatusCode = 400,
                            Message = "Ngày bắt đầu và kết thúc không thể là quá khứ"
                        };
                    }
                }

                // Validate StartDate nếu chỉ StartDate được gửi lên
                if (updatePromotionDTO.StartDate.HasValue && updatePromotionDTO.EndDate is null)
                {
                    if (updatePromotionDTO.StartDate > promotion.EndDate)
                    {
                        return new ResponseDTO
                        {
                            IsSuccess = false,
                            StatusCode = 400,
                            Message = "Ngày bắt đầu không thể sau ngày kết thúc"
                        };
                    }
                }

                // Validate theo loại khuyến mãi
                if (updatePromotionDTO.PromotionType == PromotionType.Percentage)
                {
                    if (updatePromotionDTO.Percentage is not null)
                    {
                        if (updatePromotionDTO.Percentage < 0 || updatePromotionDTO.Percentage > 100)
                        {
                            return new ResponseDTO
                            {
                                IsSuccess = false,
                                StatusCode = 400,
                                Message = "Tỷ lệ phần trăm phải nằm trong khoảng từ 0 đến 100"
                            };
                        }
                    }
                }
                else if (updatePromotionDTO.PromotionType == PromotionType.FixedAmount)
                {
                    if (updatePromotionDTO.FixedAmount is not null && updatePromotionDTO.FixedAmount <= 0)
                    {
                        return new ResponseDTO
                        {
                            IsSuccess = false,
                            StatusCode = 400,
                            Message = "Số tiền cố định phải lớn hơn 0"
                        };
                    }
                }

                // Validate mã giảm giá mới
                if (!string.IsNullOrWhiteSpace(updatePromotionDTO.Code))
                {
                    var newCode = updatePromotionDTO.Code.Trim().ToUpper();
                    if (newCode != promotion.Code)
                    {
                        var isExist = await _unitOfWork.PromotionRepository.IsExistByCodeExceptId(newCode, promotion.Id);
                        if (isExist)
                        {
                            return new ResponseDTO
                            {
                                IsSuccess = false,
                                StatusCode = 400,
                                Message = "Mã giảm giá đã tồn tại"
                            };
                        }
                        promotion.Code = newCode;
                    }
                }

                // Update các trường khác nếu có
                if (!string.IsNullOrWhiteSpace(updatePromotionDTO.Description))
                {
                    promotion.Description = updatePromotionDTO.Description.Trim();
                }

                if (updatePromotionDTO.StartDate.HasValue && updatePromotionDTO.StartDate > DateTime.MinValue)
                {
                    promotion.StartDate = updatePromotionDTO.StartDate.Value;
                }

                if (updatePromotionDTO.EndDate.HasValue && updatePromotionDTO.EndDate > DateTime.MinValue)
                {
                    promotion.EndDate = updatePromotionDTO.EndDate.Value;
                }

                if (updatePromotionDTO.PromotionType == PromotionType.Percentage)
                {
                    promotion.Percentage = updatePromotionDTO.Percentage;
                    promotion.FixedAmount = null; // Đặt FixedAmount thành null nếu PromotionType là Percentage
                } else
                {
                    promotion.FixedAmount = updatePromotionDTO.FixedAmount;
                    promotion.Percentage = null; // Đặt Percentage thành null nếu PromotionType là FixedAmount
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
