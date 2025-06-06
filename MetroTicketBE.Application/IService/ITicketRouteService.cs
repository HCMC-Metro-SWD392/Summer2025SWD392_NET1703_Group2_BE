using MetroTicketBE.Domain.DTO.Auth;
using MetroTicketBE.Domain.DTO.TicketRoute;
using MetroTicketBE.Domain.Enums;
using System.Security.Claims;

namespace MetroTicketBE.Application.IService
{
    public interface ITicketRouteService
    {
        Task<ResponseDTO> CraeteTicketRoute(CreateTicketRouteDTO createTicketRouteDTO);
        Task<ResponseDTO> GetTicketRouteByFromToAsync(Guid StartStation, Guid EndStation);
        Task<ResponseDTO> GetAllTicketRoutesAsync
            (
                ClaimsPrincipal user,
                string? filterOn,
                string? filterQuery,
                double? fromPrice,
                double? toPrice,
                string? sortBy,
                bool? isAcsending,
                TicketRoutStatus ticketType,
                int pageNumber,
                int pageSize
            );

    }
}
