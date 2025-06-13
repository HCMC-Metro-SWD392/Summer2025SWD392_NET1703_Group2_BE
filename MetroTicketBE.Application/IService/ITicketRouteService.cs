using MetroTicketBE.Domain.DTO.Auth;
using MetroTicketBE.Domain.DTO.TicketRoute;


namespace MetroTicketBE.Application.IService
{
    public interface ITicketRouteService
    {
        Task<ResponseDTO> CraeteTicketRoute(CreateTicketRouteDTO createTicketRouteDTO);
        Task<ResponseDTO> GetTicketRouteByFromTo(Guid StartStation, Guid EndStation);
        //Task<ResponseDTO> ChangeTicketStartStation
    }
}