namespace MetroTicketBE.Domain.DTO.Auth;

public class UserDTO
{
    public string Id { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? IdentityId { get; set; }
    public string Sex { get; set; } = string.Empty;
    public DateTime? DateOfBirth { get; set; }
    public string UserName { get; set; } = string.Empty;
}