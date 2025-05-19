using MetroTicket.Domain.Entities;

namespace MetroTicketBE.Domain.Entities;

public class FormRequest : BaseEntity
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Title { get; set; } = null!;
    public string Content { get; set; } = null!;
    public Guid FormRequestTypeId { get; set; }
    public Guid ReviewerId { get; set; }
    public string RejectionReason { get; set; } = null!;
    public Guid StatusId { get; set; }
    
    public FormRequestType FormRequestType { get; set; } = null!;
    public Status Status { get; set; } = null!;
    public User User { get; set; } = null!;
}