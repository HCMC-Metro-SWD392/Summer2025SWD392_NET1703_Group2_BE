using MetroTicketBE.Domain.DTO.Auth;
using MetroTicketBE.Domain.DTO.TicketRoute;

namespace MetroTicketBE.Application.IService
{
    public interface ITicketRouteService
    {
        Task<ResponseDTO> CraeteTicketRoute(CreateTicketRouteDTO createTicketRouteDTO);
        Task<ResponseDTO> GetTicketRouteByFromToAsync(Guid StartStation, Guid EndStation);
    }
}
