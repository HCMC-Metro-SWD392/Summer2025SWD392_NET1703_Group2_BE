using MetroTicketBE.Domain.DTO.Auth;
using MetroTicketBE.Domain.DTO.MetroLineStation;

namespace MetroTicketBE.Application.IService
{
    public interface IMetroLineStationService
    {
        Task<ResponseDTO> CreateMetroLineStation(CreateMetroLineStationDTO createMetroLineStationDTO);
        Task<ResponseDTO> GetStationByMetroLineIdAsync(Guid metroLineId, bool? isActive = null);
        Task<ResponseDTO> UpdateMetroLineStation(Guid id, UpdateMetroLineStationDTO updateDTO);
        Task<ResponseDTO> RemoveMetroLineStation(Guid metroLineStationId);
    }
}
