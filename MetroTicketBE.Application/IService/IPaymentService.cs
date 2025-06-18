using MetroTicketBE.Domain.DTO.Auth;
using MetroTicketBE.Domain.DTO.Payment;
using System.Security.Claims;

namespace MetroTicketBE.Application.IService
{
    public interface IPaymentService
    {
        Task<ResponseDTO> CreateLinkPaymentTicketPayOS(ClaimsPrincipal user, CreateLinkPaymentPayOSDTO createLinkDTO);
        Task<ResponseDTO> UpdatePaymentTickerStatusPayOS(ClaimsPrincipal user, string orderCode);
        Task<ResponseDTO> CreateLinkPaymentOverStationTicketRoutePayOS(ClaimsPrincipal user, CreateLinkPaymentOverStationDTO createLinkPaymentOverStationDTO);
        Task<ResponseDTO> UpdatePaymentOverStationTicketRoutePayOS(ClaimsPrincipal user, string orderCode);
    }
}
