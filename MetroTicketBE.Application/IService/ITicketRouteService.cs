using MetroTicketBE.Domain.DTO.Auth;
using MetroTicketBE.Domain.DTO.TicketRoute;
using MetroTicketBE.Domain.Enums;
using System.Security.Claims;

namespace MetroTicketBE.Application.IService
{
    public interface ITicketRouteService
    {
        Task<ResponseDTO> CraeteTicketRoute(CreateTicketRouteDTO createTicketRouteDTO);
        Task<ResponseDTO> GetTicketRouteByFromTo(Guid StartStation, Guid EndStation);
    }
}
