using MetroTicketBE.Domain.DTO.Auth;
using MetroTicketBE.Domain.DTO.Promotion;
using System.Security.Claims;

namespace MetroTicketBE.Application.IService
{
    public interface IPromotionService
    {
        Task<ResponseDTO> GetAll
            (
                ClaimsPrincipal user,
            string? filterOn,
            string? filterQuery,
            string? sortBy,
            bool? isAcsending,
            int pageNumber,
            int pageSize
            );
        Task<ResponseDTO> GetPromotionById(Guid id);
        Task<ResponseDTO> CreatePromotion(ClaimsPrincipal user, CreatePromotionDTO createPromotionDTO);
        Task<ResponseDTO> UpdatePromotion(ClaimsPrincipal user, Guid promotionId, UpdatePromotionDTO updatePromotionDTO);
        Task<ResponseDTO> DeletePromotion(ClaimsPrincipal user, Guid promotionId);
        Task<ResponseDTO> GetPromotionByCode(string code);
    }
}
