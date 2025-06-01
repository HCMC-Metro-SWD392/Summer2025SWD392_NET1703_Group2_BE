using MetroTicketBE.Domain.DTO.Auth;
using MetroTicketBE.Domain.DTO.Payos;
using System.Security.Claims;

namespace MetroTicketBE.Application.IService
{
    public interface IPaymentService
    {
        Task<ResponseDTO> CreateLinkPaymentTicketRoute(ClaimsPrincipal user, CreateLinkPaymentRouteDTO createLinkDTO);
    }
}
