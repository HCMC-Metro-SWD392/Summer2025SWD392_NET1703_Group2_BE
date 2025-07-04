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
        Task<bool> SendEmailFromTemplate(string toEmail, string templateName, Dictionary<string, string> replacements);
        Task<bool> SendResetPasswordEmail(string toMail, string resetLink, string userName = "", int expirationHours = 24);
        Task<ResponseDTO> CreateEmailTemplate(ClaimsPrincipal user, CreateEmailTemplateDTO createEmailTemplateDTO);
    }
}
