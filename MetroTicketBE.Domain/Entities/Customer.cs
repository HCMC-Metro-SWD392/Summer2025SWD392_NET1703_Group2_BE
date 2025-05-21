using MetroTicket.Domain.Entities;

namespace MetroTicketBE.Domain.Entities;

public class Customer
{
    public Guid Id { get; set; }
    public string UserId { get; set; }
    public Guid CustomerTypeId { get; set; }
    public Guid MembershipId { get; set; }
    public long Points { get; set; }
    
    public User User { get; set; } = null!;
    public CustomerType CustomerType { get; set; } = null!;
    public Membership Membership { get; set; } = null!;
}