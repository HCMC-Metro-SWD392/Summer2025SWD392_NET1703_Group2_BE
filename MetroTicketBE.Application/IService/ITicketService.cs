using MetroTicketBE.Domain.DTO.Auth;
using MetroTicketBE.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MetroTicketBE.Application.IService
{
    public interface ITicketService
    {
        Task<ResponseDTO> GetTicketBySerial(string serial);
        Task<ResponseDTO> ChangeTicketRouteStatus(Guid ticketId);
        Task<ResponseDTO> GetAllTicketRoutes
            (
                ClaimsPrincipal user,
                string? filterOn,
                string? filterQuery,
                double? fromPrice,
                double? toPrice,
                string? sortBy,
                bool? isAcsending,
                TicketRouteStatus ticketType,
                int pageNumber,
                int pageSize
            );
    }
}
