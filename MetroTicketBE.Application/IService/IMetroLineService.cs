using MetroTicketBE.Domain.DTO.Auth;
using MetroTicketBE.Domain.DTO.MetroLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetroTicketBE.Application.IService
{
    public interface IMetroLineService
    {
        Task<ResponseDTO> CreateMetroLine(CreateMetroLineDTO createMetroLineDTO);
        Task<ResponseDTO> GetAllMetroLines(bool? isActive);
        Task<ResponseDTO> GetMetroLineById(Guid metroLineId);
        Task<ResponseDTO> UpdateMetroLine(Guid metroLineId, UpdateMetroLineDTO updateMetroLineDTO);
        Task<ResponseDTO> SetIsActiveMetroLine(Guid metroLineId, bool isActive);
    }
}
