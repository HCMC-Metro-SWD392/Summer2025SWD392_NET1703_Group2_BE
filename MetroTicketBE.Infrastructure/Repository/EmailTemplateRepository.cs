using MetroTicketBE.Domain.Entities;
using MetroTicketBE.Infrastructure.Context;
using MetroTicketBE.Infrastructure.IRepository;
using Microsoft.EntityFrameworkCore;

namespace MetroTicketBE.Infrastructure.Repository
{
    public class EmailTemplateRepository : Repository<EmailTemplate>, IEmailTemplateRepository
    {
        private readonly ApplicationDBContext _context;
        public EmailTemplateRepository(ApplicationDBContext context) : base(context)
        {
            _context = context;
        }

        public async Task<bool> IsExistByTemplateName(string templateName)
        {
            return await _context.EmailTemplates
                .AnyAsync(et => et.TemplateName.ToLower() == templateName.ToLower());
        }
    }
}
