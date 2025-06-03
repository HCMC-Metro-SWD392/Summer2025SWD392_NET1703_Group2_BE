using MetroTicketBE.Domain.Enums;

namespace MetroTicketBE.Domain.DTO.Customer;

public class UpdateCustomerDTO
{
    public string? FullName { get; set; }
    public string? Address { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? IdentityId { get; set; }
    
    public string? Sex { get; set; }
    
    public CustomerType? CustomerType { get; set; }
}