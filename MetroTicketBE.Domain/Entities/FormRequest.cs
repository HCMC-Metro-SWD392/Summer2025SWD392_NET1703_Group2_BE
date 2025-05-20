using MetroTicket.Domain.Entities;

namespace MetroTicketBE.Domain.Entities;

public class FormRequest : BaseEntity<Guid, string, string>
{
    public string SenderId { get; set; }
    public string Title { get; set; } = null!;
    public string Content { get; set; } = null!;
    public Guid FormRequestTypeId { get; set; }
    public string ReviewerId { get; set; }
    public string RejectionReason { get; set; } = null!;
    public Guid StatusId { get; set; }
    
    public FormRequestType FormRequestType { get; set; } = null!;
    public Status Status { get; set; } = null!;
    public User Reviewer { get; set; } = null!;
    public User Sender { get; set; } = null!;
}