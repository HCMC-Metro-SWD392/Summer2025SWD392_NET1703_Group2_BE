using MetroTicketBE.Domain.DTO.Auth;
using MetroTicketBE.Domain.DTO.MetroLine;
using MetroTicketBE.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MetroTicketBE.Application.IService
{
    public interface IMetroLineService
    {
        Task<ResponseDTO> CreateMetroLine(ClaimsPrincipal user, CreateMetroLineDTO createMetroLineDTO);
        Task<ResponseDTO> GetAllMetroLines(bool? isActive);
        Task<ResponseDTO> GetMetroLineById(Guid metroLineId);
        Task<ResponseDTO> UpdateMetroLine(ClaimsPrincipal user, Guid metroLineId, UpdateMetroLineDTO updateMetroLineDTO);
        Task<ResponseDTO> SetIsActiveMetroLine(ClaimsPrincipal user, Guid metroLineId, bool isActive);
        Task<ResponseDTO> ChangeMetroLineStatus(Guid metroLineId, MetroLineStatus metroLineStatus);
        Task<ResponseDTO> CheckMetroLineErrorInPath(Guid stationStartId, Guid stationEndId);
        Task<ResponseDTO> CheckMetroLineErrorInPathV2(Guid ticketId);
    }
}
