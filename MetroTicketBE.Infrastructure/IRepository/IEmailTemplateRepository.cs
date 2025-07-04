using MetroTicketBE.Domain.Entities;

namespace MetroTicketBE.Infrastructure.IRepository
{
    public interface IEmailTemplateRepository : IRepository<EmailTemplate>
    {
        Task<bool> IsExistByTemplateName(string templateName);
    }
}
