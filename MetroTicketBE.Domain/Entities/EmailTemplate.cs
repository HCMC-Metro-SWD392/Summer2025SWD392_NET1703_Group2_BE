using MetroTicket.Domain.Entities;
using MetroTicketBE.Domain.Enum;

namespace MetroTicketBE.Domain.Entities;

public class EmailTemplate
{
    public Guid Id { get; set; }
    public string TemplateName { get; set; } = null!;
    public string SubjectLine { get; set; } = null!;
    public string BodyContent { get; set; } = null!;
    public string SenderName { get; set; } = null!;
    public string SenderEmail { get; set; } = null!;
    public string Category { get; set; } = null!;
    public string PreheaderText { get; set; } = null!;
    public string PersonalizationTag { get; set; } = null!;
    public string FooterContent { get; set; } = null!;
    public string CallToActionText { get; set; } = null!;
    public string Language { get; set; } = null!;
    public string RecipientType { get; set; } = null!;
    public EmailStatus Status { get; set; }
}