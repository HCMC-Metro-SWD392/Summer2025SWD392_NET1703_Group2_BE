using MetroTicketBE.Domain.DTO.Auth;
using MetroTicketBE.Domain.DTO.Payment;
using System.Security.Claims;

namespace MetroTicketBE.Application.IService
{
    public interface IPaymentService
    {
        Task<ResponseDTO> CreateLinkPaymentTicketRoutePayOS(ClaimsPrincipal user, CreateLinkPaymentRoutePayOSDTO createLinkDTO);
        Task<ResponseDTO> UpdatePaymentTickerStatusPayOS(ClaimsPrincipal user, long orderCode);
    }
}
