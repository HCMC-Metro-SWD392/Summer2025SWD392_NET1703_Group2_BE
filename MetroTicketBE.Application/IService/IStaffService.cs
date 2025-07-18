using MetroTicketBE.Domain.DTO.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetroTicketBE.Application.IService
{
    public interface IStaffService
    {
        Task<ResponseDTO> GetAllStaff(bool? isActive);
        Task<ResponseDTO> GetStaffByStaffCode(string staffCode);
    }
}
