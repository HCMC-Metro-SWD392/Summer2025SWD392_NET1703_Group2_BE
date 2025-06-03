using MetroTicketBE.Domain.DTO.Auth;
using MetroTicketBE.Domain.DTO.Station;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetroTicketBE.Application.IService
{
    public interface IStationService
    {
        Task<ResponseDTO> CreateStation(CreateStationDTO createStationDTO);
        Task<ResponseDTO> GetAllStations();
    }
}
