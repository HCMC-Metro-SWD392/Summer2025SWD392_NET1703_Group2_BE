using MetroTicketBE.Domain.DTO.Auth;
using MetroTicketBE.Domain.DTO.Station;
using MetroTicketBE.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MetroTicketBE.Application.IService
{
    public interface IStationService
    {
        Task<ResponseDTO> CreateStation(ClaimsPrincipal user, CreateStationDTO createStationDTO);
        Task<ResponseDTO> UpdateStation(ClaimsPrincipal user, Guid stationId, UpdateStationDTO updateStationDTO);
        Task<ResponseDTO> GetAllStations(bool? isAscending, int pageNumber, int pageSize, bool? isActive);
        Task<ResponseDTO> GetStationById(Guid stationId);
        Task<ResponseDTO> SearchStationsByName(string? name, bool? isActive);
        Task<ResponseDTO> SetIsActiveStation(ClaimsPrincipal user,Guid stationId, bool isActive);
        Task<ResponseDTO> SearchTicketRoad(Guid stationStartId, Guid stationEndId);
        Task<ResponseDTO> SearchTicketRoadV2(Guid ticketId);
    }
}
