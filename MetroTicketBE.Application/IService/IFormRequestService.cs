using MetroTicketBE.Domain.DTO.Auth;
using MetroTicketBE.Domain.DTO.FormRequest;
using MetroTicketBE.Domain.Enum;
using System.Security.Claims;

namespace MetroTicketBE.Application.IService
{
    public interface IFormRequestService
    {
        Task<ResponseDTO> CreateFormRequest(ClaimsPrincipal user, CreateFormRequestDTO createFormRequestDTO);
        Task<ResponseDTO> GetFormRequest(ClaimsPrincipal user);
        Task<ResponseDTO> GetAll(string sortBy, FormStatus formStatus, bool? isAcsending, int pageNumber, int pageSize);
        Task<ResponseDTO> GetAllFormAttachment(Guid formRequestId);
        Task<ResponseDTO> ChangeFormRequestStatus(ClaimsPrincipal user, Guid formRequestId, ChangeFormStatusDTO changeFormStatusDTO);
    }
}
