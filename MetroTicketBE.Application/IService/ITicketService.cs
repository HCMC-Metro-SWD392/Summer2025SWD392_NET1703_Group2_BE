using MetroTicketBE.Domain.DTO.Auth;
using MetroTicketBE.Domain.Entities;
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
        Task<ResponseDTO> GetAllTickets
            (
                ClaimsPrincipal user,
                string? filterOn,
                string? filterQuery,
                double? fromPrice,
                double? toPrice,
                string? sortBy,
                bool? isAcsending,
                TicketStatus ticketType,
                int pageNumber,
                int pageSize
            );
        Task<ResponseDTO> CreateTicketForSpecialCase(ClaimsPrincipal user, Guid subscriptionId);
        Task<ResponseDTO> CheckInTicketProcess(string qrCode, Guid stationId);
        Task<ResponseDTO> CheckOutTicketProcess(string qrCode, Guid stationId);
        Task<ResponseDTO> GetORCode(ClaimsPrincipal user, Guid ticketId);
        Task<ResponseDTO> CheckExistTicketRange(ClaimsPrincipal user, Guid startStaionId, Guid endStationId);
    }
}
