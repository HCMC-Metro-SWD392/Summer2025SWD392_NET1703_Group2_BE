using MetroTicket.Domain.Entities;
using MetroTicketBE.Domain.Enum;

namespace MetroTicketBE.Domain.Entities;

public class FormRequest
{
    public Guid Id { get; set; }
    public required string SenderId { get; set; }
    public string Title { get; set; } = null!;
    public string? Content { get; set; }
    public required FormRequestType FormRequestType { get; set; }
    public string? ReviewerId { get; set; }
    public string? RejectionReason { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public FormStatus Status { get; set; }
    
    public ApplicationUser Reviewer { get; set; } = null!;
    public ApplicationUser Sender { get; set; } = null!;

    public ICollection<FormAttachment> FormAttachments { get; set; } = new List<FormAttachment>();
}