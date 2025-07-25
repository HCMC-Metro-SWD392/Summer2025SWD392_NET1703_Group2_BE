﻿using MetroTicketBE.Application.IService;
using MetroTicketBE.Domain.DTO.Auth;
using MetroTicketBE.Domain.DTO.FareRule;
using MetroTicketBE.Domain.DTO.Promotion;
using MetroTicketBE.Domain.Entities;
using MetroTicketBE.Infrastructure.IRepository;
using System.Globalization;
using System.Security.Claims;

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
                        Message = "Quy tắc vé không hợp lệ",
                        StatusCode = 400
                    };
                }

                await _unitOfWork.FareRuleRepository.AddAsync(fareRule);
                await _unitOfWork.SaveAsync();

                return new ResponseDTO
                {
                    IsSuccess = true,
                    Message = "Tạo quy tắc vé thành công",
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

        public async Task<ResponseDTO> GetAll(ClaimsPrincipal user, string? sortBy, bool? isAcsending, int pageNumber, int pageSize)
        {
            try
            {
                var fareRule = await _unitOfWork.FareRuleRepository.GetAllAsync();

                if (!string.IsNullOrEmpty(sortBy))
                {
                    fareRule = sortBy.Trim().ToLower() switch
                    {
                        "price" => isAcsending == true ? fareRule.OrderBy(f => f.Fare) : fareRule.OrderByDescending(p => p.Fare),
                        "mindistance" => isAcsending == true ? fareRule.OrderBy(p => p.MinDistance) : fareRule.OrderByDescending(p => p.MinDistance),
                        "createdat" => isAcsending == true ? fareRule.OrderBy(p => p.CreatedAt) : fareRule.OrderByDescending(p => p.CreatedAt),
                        _ => fareRule
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
                    fareRule = fareRule.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                }

                if (fareRule is null || !fareRule.Any())
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
                    Message = "Lấy danh sách quy tắc vé thành công",
                    StatusCode = 200,
                    Result = fareRule
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO
                {
                    Message = $"Đã xảy ra lỗi khi lấy danh sách quy tắc vé: {ex.Message}",
                    IsSuccess = false,
                    StatusCode = 500
                };
            }
        }

        public async Task<ResponseDTO> UpdateFareRule(UpdateFareRuleDTO updareFareRuleDTO)
        {
            try
            {
                var fareRule = await _unitOfWork.FareRuleRepository.GetByIdAsync(updareFareRuleDTO.Id);
                if (fareRule is null)
                {
                    return new ResponseDTO
                    {
                        IsSuccess = false,
                        Message = "Quy tắc vé không tồn tại",
                        StatusCode = 404
                    };
                }

                if (updareFareRuleDTO.MaxDistance is not null && updareFareRuleDTO.MinDistance is not null)
                {
                    fareRule.MinDistance = updareFareRuleDTO.MinDistance.Value;
                    fareRule.MaxDistance = updareFareRuleDTO.MaxDistance.Value;
                }

                else if (updareFareRuleDTO.MaxDistance is not null)
                {
                    fareRule.MaxDistance = updareFareRuleDTO.MaxDistance.Value;
                }

                else if (updareFareRuleDTO.MinDistance is not null)
                {
                    fareRule.MinDistance = updareFareRuleDTO.MinDistance.Value;
                }

                if (updareFareRuleDTO.Fare is not null)
                {
                    fareRule.Fare = updareFareRuleDTO.Fare.Value;
                }

                await CheckValidDistance(fareRule.MinDistance, fareRule.MaxDistance, fareRule.Id);
                await _unitOfWork.SaveAsync();

                return new ResponseDTO
                {
                    IsSuccess = true,
                    Message = "Cập nhật quy tắc vé thành công",
                    StatusCode = 200,
                    Result = fareRule
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO
                {
                    Message = $"Đã xảy ra lỗi khi cập nhật quy tắc vé: {ex.Message}",
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

            var isOverlap = await _unitOfWork.FareRuleRepository
                 .GetAsync(f => f.MaxDistance > minDistance);

            if (isOverlap is not null)
            {
                throw new Exception("Khoảng cách mới chồng lấn với khoảng cách đã tồn tại");
            }

            return true;
        }

        private async Task<bool> CheckValidDistance(double minDistance, double maxDistance, Guid currentFareRuleId)
        {
            if (minDistance >= maxDistance)
            {
                throw new Exception("Khoảng cách tối thiểu phải nhỏ hơn khoảng cách tối đa");
            }

            var isOverlap = await _unitOfWork.FareRuleRepository.GetAsync(f => f.Id != currentFareRuleId && f.MinDistance < maxDistance && minDistance < f.MaxDistance);

            if (isOverlap is not null)
            {
                throw new Exception("Khoảng cách mới chồng lấn với khoảng cách đã tồn tại");
            }

            return true;
        }
    }
}
