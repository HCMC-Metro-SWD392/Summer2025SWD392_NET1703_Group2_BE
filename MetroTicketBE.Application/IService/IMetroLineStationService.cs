using MetroTicketBE.Domain.DTO.Auth;
using MetroTicketBE.Domain.DTO.MetroLineStation;

namespace MetroTicketBE.Application.IService
{
    public interface IMetroLineStationService
    {
        Task<ResponseDTO> CreateMetroLineStation(CreateMetroLineStationDTO createMetroLineStationDTO);
    }
}
