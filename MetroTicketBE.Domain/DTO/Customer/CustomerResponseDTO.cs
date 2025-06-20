using MetroTicketBE.Domain.Enums;

namespace MetroTicketBE.Domain.DTO.Customer;

public class CustomerResponseDTO
{
    public Guid Id { get; set; }
    public CustomerType CustomerType { get; set; }
    public String? FullName { get; set; } = null!;
    public String? Address { get; set; } = null;
    public String? Sex { get; set; } = null;
    public DateOnly? DateOfBirth { get; set; }
    public String? UserName { get; set; } = null;
    public String? Email { get; set; } = null;
    public String? PhoneNumber { get; set; } = null;
    public MembershipDTO? Membership { get; set; }
    public String? IdentityId { get; set; } = null;
    public long? Points { get; set; } = 0;
    public DateTime? StudentExpiration { get; set; }
}
public class MembershipDTO
{
    public Guid? Id { get; set; } = null;
    public String? MembershipType { get; set; } = null;
}