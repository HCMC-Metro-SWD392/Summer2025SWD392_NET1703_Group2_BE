using MetroTicketBE.Domain.DTO.Auth;

namespace MetroTicketBE.Application.IService
{
    public interface IAuthService
    {
        Task<ResponseDTO> RegisterCustomer(RegisterCustomerDTO registerCustomerDTO);
    }
}
