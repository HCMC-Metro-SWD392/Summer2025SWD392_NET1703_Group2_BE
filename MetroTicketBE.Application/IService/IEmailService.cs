using MetroTicketBE.Domain.DTO.Auth;
using MetroTicketBE.Domain.DTO.Email;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MetroTicketBE.Application.IService
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(string to, string subject, string body);
        Task<bool> SendVerifyEmail(string toMail, string confirmationLink);
        Task<bool> SendEmailFromTemplate(string toEmail, string templateName, string confirmationLink);
        Task<ResponseDTO> CreateEmailTemplate(ClaimsPrincipal user, CreateEmailTemplateDTO createEmailTemplateDTO);
    }
}
