using MetroTicketBE.Domain.DTO.Auth;
using MetroTicketBE.Domain.DTO.Payos;
using System.Security.Claims;

namespace MetroTicketBE.Application.IService
{
    public interface IPayosService
    {
        Task<ResponseDTO> CreateLinkPaymentTicketRoute(ClaimsPrincipal user, CreateLinkPaymentRouteDTO createLinkDTO)
    }
}
