
using MetroTicketBE.Application.IService;
using MetroTicketBE.Infrastructure.IRepository;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace MetroTicketBE.Application.Service
{
    public class EmailSerivce : IEmailService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;

        public EmailSerivce(IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task<bool> SendEmailAsync(string toEmail, string subject, string body)
        {
            var isSuccess = false;
            try
            {
                var fromEmail = _configuration["EmailSettings:FromEmail"];
                var fromPassword = _configuration["EmailSettings:FromPassword"];
                var smtpHost = _configuration["EmailSettings:SmtpHost"];
                var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"]);

                var message = new MailMessage(fromEmail, toEmail, subject, body);
                message.IsBodyHtml = true;

                using var smtpClient = new SmtpClient(smtpHost, smtpPort)
                {
                    Credentials = new NetworkCredential(fromEmail, fromPassword),
                    EnableSsl = true
                };

                await smtpClient.SendMailAsync(message);

                isSuccess = true;
                return isSuccess;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Lỗi khi gửi email: {e.Message}");
                return isSuccess;
            }
        }

        public async Task<bool> SendVerifyEmail(string toMail, string confirmationLink)
        {
            return await SendEmailFromTemplate(toMail, "SendVerifyEmail", confirmationLink);
        }

        public async Task<bool> SendEmailFromTemplate(string toMail, string templateName, string replacementValue)
        {
            var template = await _unitOfWork.EmailTemplateRepository.GetAsync(t => t.TemplateName == templateName);

            if (template is null)
            {
                throw new Exception($"Không tìm thấy template email với tên: {templateName}");
            }

            string subject = template.SubjectLine;
            string body = template.BodyContent.Replace("{Login}", replacementValue);

            return await SendEmailAsync(toMail, subject, body);

        }
    }
}
