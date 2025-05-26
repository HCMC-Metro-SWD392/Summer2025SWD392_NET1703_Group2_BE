using MetroTicket.Domain.Entities;
using MetroTicketBE.Domain.Enum;

namespace MetroTicketBE.Domain.Entities;

public class FormRequest
{
    public Guid Id { get; set; }
    public required string SenderId { get; set; }
    public string Title { get; set; } = null!;
    public string Content { get; set; } = null!;
    public required FormRequestType FormRequestType { get; set; }
    public string ReviewerId { get; set; } = null!;
    public string RejectionReason { get; set; } = null!;
    public string CreatedBy { get; set; } = null!; // UserId of the creator
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public FormStatus Status { get; set; }
    
    public User Reviewer { get; set; } = null!;
    public User Sender { get; set; } = null!;

    public ICollection<FormAttachment> FormAttachments { get; set; } = new List<FormAttachment>();
}