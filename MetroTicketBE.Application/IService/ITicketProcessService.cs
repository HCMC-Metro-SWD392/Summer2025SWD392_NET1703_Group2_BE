using MetroTicketBE.Domain.DTO.Auth;

namespace MetroTicketBE.Application.IService
{
    public interface ITicketProcessService
    {
        Task<ResponseDTO> GetAllTicketProcessByTicketId(Guid ticketId);
    }
}
