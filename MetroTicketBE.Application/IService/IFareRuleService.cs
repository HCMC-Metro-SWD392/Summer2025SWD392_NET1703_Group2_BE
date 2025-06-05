using MetroTicketBE.Domain.DTO.Auth;
using MetroTicketBE.Domain.DTO.FareRule;

namespace MetroTicketBE.Application.IService
{
    public interface IFareRuleService
    {
        Task<ResponseDTO> GetAll();
        Task<ResponseDTO> CreateFareRule(CreateFareRuleDTO createFareRuleDTO);
        Task<ResponseDTO> UpdateFareRule(UpdateFareRuleDTO updareFareRuleDTO);
    }
}
