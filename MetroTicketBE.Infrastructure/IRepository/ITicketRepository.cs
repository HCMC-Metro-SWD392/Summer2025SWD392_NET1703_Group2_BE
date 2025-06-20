using MetroTicketBE.Domain.Entities;

namespace MetroTicketBE.Infrastructure.IRepository
{
    public interface ITicketRepository : IRepository<Ticket>
    {
        Task<Ticket?> GetTicketBySerialAsync(string serial);
        Task<Ticket?> GetByIdAsync(Guid ticketId);
        Task<Ticket?> GetByQrCodeAsync(string qrCode);
    }
}
