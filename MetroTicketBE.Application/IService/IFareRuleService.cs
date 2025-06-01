using MetroTicketBE.Domain.DTO.Auth;
using MetroTicketBE.Domain.DTO.FareRule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetroTicketBE.Application.IService
{
    public interface IFareRuleService
    {
        Task<ResponseDTO> CreateFareRule(CreateFareRuleDTO createFareRuleDTO);
    }
}
