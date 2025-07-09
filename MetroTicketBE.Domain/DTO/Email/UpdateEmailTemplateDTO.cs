

namespace MetroTicketBE.Domain.DTO.Email
{
    public class UpdateEmailTemplateDTO
    {
        public string? TemplateName { get; set; }
        public string? SubjectLine { get; set; }
        public string? BodyContent { get; set; }
        public string? SenderName { get; set; }
        public string? Category { get; set; }
        public string? PreHeaderText { get; set; }
        public string? PersonalizationTags { get; set; }
        public string? FooterContent { get; set; }
        public string? CallToAction { get; set; }
        public string? Language { get; set; }
        public string? RecipientType { get; set; }
    }
}
