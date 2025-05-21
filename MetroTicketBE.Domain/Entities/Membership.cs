namespace MetroTicketBE.Domain.Entities;

public class Membership
{
    public Guid Id { get; set; }
    public required string MembershipType { get; set; }
    public long PointToRedeem { get; set; }
    
    public ICollection<Customer> Customers { get; set; } = new List<Customer>();
}