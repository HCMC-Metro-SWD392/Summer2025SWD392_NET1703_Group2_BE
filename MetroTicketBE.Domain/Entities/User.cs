using Microsoft.AspNetCore.Identity;

namespace MetroTicket.Domain.Entities
{
    public class User : IdentityUser
    {
        public required string FullName { get; set; }
        public string? Address { get; set; }
        public string? Sex { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? IdentityId { get; set; }
    }
}
