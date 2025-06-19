using MetroTicketBE.Domain.DTO.Auth;
using MetroTicketBE.Domain.DTO.FormRequest;
using System.Security.Claims;

namespace MetroTicketBE.Application.IService
{
    public interface IFormRequestService
    {
        Task<ResponseDTO> SendFormRequest(ClaimsPrincipal user, CreateFormRequestDTO createFormRequestDTO);
    }
}
