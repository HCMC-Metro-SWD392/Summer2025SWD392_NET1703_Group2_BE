using MetroTicket.Domain.Entities;
using MetroTicketBE.Domain.Enums;

namespace MetroTicketBE.Domain.Entities;

public class Customer
{
    public Guid Id { get; set; }
    public required string UserId { get; set; }
    public CustomerType CustomerType { get; set; }
    public Guid? MembershipId { get; set; }
    public long Points { get; set; }
    public DateTime? StudentExpiration { get; set; }

    public ApplicationUser User { get; set; } = null!;
    public Membership Membership { get; set; } = null!;
    public ICollection<PaymentTransaction> Transactions { get; set; } = new List<PaymentTransaction>();
}