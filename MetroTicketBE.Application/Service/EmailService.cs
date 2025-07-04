using Amazon;
using Amazon.SimpleEmailV2;
using Amazon.SimpleEmailV2.Model;
using MetroTicketBE.Application.IService;
using MetroTicketBE.Domain.DTO.Auth;
using MetroTicketBE.Domain.DTO.Email;
using MetroTicketBE.Domain.Entities;
using MetroTicketBE.Domain.Enum;
using MetroTicketBE.Infrastructure.IRepository;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;

namespace MetroTicketBE.Application.Service
{
    public class EmailService : IEmailService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;

        public EmailService(IUnitOfWork unitOfWork, IConfiguration configuration)
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

                var sesClient = new AmazonSimpleEmailServiceV2Client(RegionEndpoint.APSoutheast1);

                var sendRequest = new SendEmailRequest
                {
                    FromEmailAddress = fromEmail,
                    Destination = new Destination
                    {
                        ToAddresses = new List<string> { toEmail }
                    },
                    Content = new EmailContent
                    {
                        Simple = new Message
                        {
                            Subject = new Content { Data = subject },
                            Body = new Body
                            {
                                Html = new Content { Data = body }
                            }
                        }
                    }
                };
                var response = await sesClient.SendEmailAsync(sendRequest);

                if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                {
                    isSuccess = true;
                }

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
            return await SendEmailFromTemplate(toMail, "VerifyEmail", confirmationLink);
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

        public async Task<ResponseDTO> CreateEmailTemplate(ClaimsPrincipal user, CreateEmailTemplateDTO createEmailTemplateDTO)
        {
            try
            {
                var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return new ResponseDTO
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy thông tin người dùng.",
                        StatusCode = 400
                    };
                }

                // Kiểm tra xem template đã tồn tại chưa
                var existingTemplate = await _unitOfWork.EmailTemplateRepository.IsExistByTemplateName(createEmailTemplateDTO.TemplateName);
                if (existingTemplate)
                {
                    return new ResponseDTO
                    {
                        IsSuccess = false,
                        Message = $"Template [{createEmailTemplateDTO.TemplateName}] đã tồn tại.",
                        StatusCode = 409
                    };
                }

                var emailTemplate = new EmailTemplate
                {
                    TemplateName = createEmailTemplateDTO.TemplateName,
                    SubjectLine = createEmailTemplateDTO.SubjectLine,
                    BodyContent = createEmailTemplateDTO.BodyContent,
                    SenderName = createEmailTemplateDTO.SenderName,
                    Category = createEmailTemplateDTO.Category,
                    PreHeaderText = createEmailTemplateDTO.PreHeaderText,
                    PersonalizationTags = createEmailTemplateDTO.PersonalizationTags,
                    FooterContent = createEmailTemplateDTO.FooterContent,
                    CallToAction = createEmailTemplateDTO.CallToAction,
                    Language = createEmailTemplateDTO.Language,
                    RecipientType = createEmailTemplateDTO.RecipientType,
                    Status = EmailStatus.Active,
                    CreatedBy = userId
                };

                await _unitOfWork.EmailTemplateRepository.AddAsync(emailTemplate);
                await _unitOfWork.SaveAsync();

                return new ResponseDTO
                {
                    IsSuccess = true,
                    Message = "Email template đã được tạo thành công",
                    StatusCode = 201
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO
                {
                    IsSuccess = false,
                    Message = $"Lỗi khi tạo template email: {ex.Message}",
                    StatusCode = 500
                };
            }
        }
    }
}
