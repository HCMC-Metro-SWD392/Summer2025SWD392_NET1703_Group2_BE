using MetroTicketBE.Domain.Enums;

namespace MetroTicketBE.Domain.Entities
{
    public class News
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string Content { get; set; } = null!;
        public string? Summary { get; set; }
        public string? ImageUrl { get; set; }
        public string Category { get; set; } = null!;
        public Guid StaffId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; }
        public NewsStatus Status { get; set; } = NewsStatus.Pending;
        public string? RejectionReason { get; set; }

        public Staff Staff { get; set; } = null!;
    }
}
