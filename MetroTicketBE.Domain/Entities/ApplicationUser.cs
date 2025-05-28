using MetroTicketBE.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace MetroTicket.Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public required string FullName { get; set; }
        public string? Address { get; set; }
        public string? Sex { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? IdentityId { get; set; }

        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
        public ICollection<Log> Logs { get; set; } = new List<Log>();
        public ICollection<FormRequest> FormRequestsAsSenders { get; set; } = new List<FormRequest>();
        public ICollection<FormRequest> FormRequestsAsReviewers { get; set; } = new List<FormRequest>();

        public ICollection<EmailTemplate> EmailTemplates { get; set; } = new List<EmailTemplate>();
    }
}