using Amazon;
using Amazon.SimpleEmailV2;
using Amazon.SimpleEmailV2.Model;
using AutoMapper;
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
        private readonly IMapper _mapper;
        private readonly IRedisService _redisService;

        public EmailService(IUnitOfWork unitOfWork, IConfiguration configuration, IRedisService redisService, IMapper mapper)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _redisService = redisService ?? throw new ArgumentNullException(nameof(redisService));
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
            return await SendEmailFromTemplate(toMail, "VerifyEmail", new Dictionary<string, string>
            {
                { "{Login}", confirmationLink }
            });
        }

        public async Task<bool> SendEmailFromTemplate(string toMail, string templateName, Dictionary<string, string> replacements)
        {
            var template = await _unitOfWork.EmailTemplateRepository.GetAsync(t => t.TemplateName == templateName);

            if (template is null)
            {
                throw new Exception($"Không tìm thấy template email với tên: {templateName}");
            }

            string subject = template.SubjectLine;
            string body = template.BodyContent;

            foreach (var replacement in replacements)
            {
                subject = subject.Replace(replacement.Key, replacement.Value);
                body = body.Replace(replacement.Key, replacement.Value);
            }

            return await SendEmailAsync(toMail, subject, body);
        }

        public async Task<bool> SendResetPasswordEmail(string toMail, string resetLink, string userName = "", int expirationHours = 24)
        {
            var expirationTime = $"{expirationHours} giờ";
            return await SendEmailFromTemplate(toMail, "ResetPasswordEmail", new Dictionary<string, string>
            {
                { "{ResetLink}", resetLink },
                { "{UserName}", !string.IsNullOrEmpty(userName) ? userName : "Quý khách" },
                { "{ExpirationTime}", expirationTime }
            });
        }

        //public async Task<bool> SendPasswordChangedNotification(string toMail, string userName)
        //{
        //    return await SendEmailFromTemplate(toMail, "PasswordChangedNotificationEmail", new Dictionary<string, string>
        //    {
        //        { "{UserName}", !string.IsNullOrEmpty(userName) ? userName : "Quý khách" },
        //        { "{ChangeDateTime}", DateTime.Now.ToString("dd/MM/yyyy HH:mm") }
        //    });
        //}

        //// Method để gửi email welcome sau khi verify thành công
        //public async Task<bool> SendWelcomeEmail(string toMail, string userName)
        //{
        //    return await SendEmailFromTemplate(toMail, "WelcomeEmail", new Dictionary<string, string>
        //    {
        //        { "{UserName}", !string.IsNullOrEmpty(userName) ? userName : "Quý khách" }
        //    });
        //}
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

        public async Task<ResponseDTO> UpdateEmailTemplate(ClaimsPrincipal user, Guid templateId, UpdateEmailTemplateDTO updateEmailTemplateDTO)
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

                var existingTemplate = await _unitOfWork.EmailTemplateRepository.GetByIdAsync(templateId);
                if (existingTemplate == null)
                {
                    return new ResponseDTO
                    {
                        IsSuccess = false,
                        Message = $"Không tìm thấy template với ID: {templateId}",
                        StatusCode = 404
                    };
                }

                if (!string.IsNullOrEmpty(updateEmailTemplateDTO.TemplateName) &&
                    !string.Equals(existingTemplate.TemplateName, updateEmailTemplateDTO.TemplateName, StringComparison.OrdinalIgnoreCase))
                {
                    var isDuplicate = await _unitOfWork.EmailTemplateRepository.IsExistByTemplateName(updateEmailTemplateDTO.TemplateName);
                    if (isDuplicate)
                    {
                        return new ResponseDTO
                        {
                            IsSuccess = false,
                            Message = $"Template [{updateEmailTemplateDTO.TemplateName}] đã tồn tại.",
                            StatusCode = 409
                        };
                    }
                    existingTemplate.TemplateName = updateEmailTemplateDTO.TemplateName;
                }

                if (updateEmailTemplateDTO.SubjectLine != null)
                    existingTemplate.SubjectLine = updateEmailTemplateDTO.SubjectLine;

                if (updateEmailTemplateDTO.BodyContent != null)
                    existingTemplate.BodyContent = updateEmailTemplateDTO.BodyContent;

                if (updateEmailTemplateDTO.SenderName != null)
                    existingTemplate.SenderName = updateEmailTemplateDTO.SenderName;

                if (updateEmailTemplateDTO.Category != null)
                    existingTemplate.Category = updateEmailTemplateDTO.Category;

                if (updateEmailTemplateDTO.PreHeaderText != null)
                    existingTemplate.PreHeaderText = updateEmailTemplateDTO.PreHeaderText;

                if (updateEmailTemplateDTO.PersonalizationTags != null)
                    existingTemplate.PersonalizationTags = updateEmailTemplateDTO.PersonalizationTags;

                if (updateEmailTemplateDTO.FooterContent != null)
                    existingTemplate.FooterContent = updateEmailTemplateDTO.FooterContent;

                if (updateEmailTemplateDTO.CallToAction != null)
                    existingTemplate.CallToAction = updateEmailTemplateDTO.CallToAction;

                if (updateEmailTemplateDTO.Language != null)
                    existingTemplate.Language = updateEmailTemplateDTO.Language;

                if (updateEmailTemplateDTO.RecipientType != null)
                    existingTemplate.RecipientType = updateEmailTemplateDTO.RecipientType;

                existingTemplate.UpdatedAt = DateTime.UtcNow;
                existingTemplate.UpdatedBy = userId;

                _unitOfWork.EmailTemplateRepository.Update(existingTemplate);
                await _unitOfWork.SaveAsync();

                return new ResponseDTO
                {
                    IsSuccess = true,
                    Message = "Cập nhật template email thành công.",
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO
                {
                    IsSuccess = false,
                    Message = $"Lỗi khi cập nhật template email: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<ResponseDTO> GetAllEmailTemplate(string? filterOn, string? filterQuery, string? sortBy, bool? isAcending, int pageNumber, int pageSize)
        {
            try
            {
                var emailTemples = (await _unitOfWork.EmailTemplateRepository.GetAllAsync())
                    .Where(e => e.Status == EmailStatus.Active);

                if (!string.IsNullOrEmpty(filterOn) && !string.IsNullOrEmpty(filterQuery))
                {
                    filterOn = filterOn.ToLower().Trim();
                    filterQuery = filterQuery.ToLower().Trim();

                    emailTemples = filterOn switch
                    {
                        "templatename" => emailTemples.Where(t => t.TemplateName.ToLower().Contains(filterQuery)),
                        "Language" => emailTemples.Where(t => t.Language.ToLower().Contains(filterQuery)),
                        "category" => emailTemples.Where(t => t.Category.ToLower().Contains(filterQuery)),

                        _ => emailTemples
                    };
                }

                if (!string.IsNullOrEmpty(sortBy))
                {
                    sortBy = sortBy.ToLower().Trim();
                    emailTemples = sortBy switch
                    {
                        "templatename" => isAcending == true ?
                            emailTemples.OrderBy(t => t.TemplateName) :
                            emailTemples.OrderByDescending(t => t.TemplateName),
                        "language" => isAcending == true ?
                            emailTemples.OrderBy(t => t.Language) :
                            emailTemples.OrderByDescending(t => t.Language),
                        "category" => isAcending == true ?
                            emailTemples.OrderBy(t => t.Category) :
                            emailTemples.OrderByDescending(t => t.Category),

                        _ => emailTemples
                    };
                }

                if (pageNumber <= 0 || pageSize <= 0)
                {
                    return new ResponseDTO
                    {
                        IsSuccess = false,
                        Message = "Số trang hoặc kích thước trang không hợp lệ.",
                        StatusCode = 400
                    };
                }
                else
                {
                    emailTemples = emailTemples.Skip(pageNumber - 1).Take(pageSize).ToList();
                }

                //var getAllEmailTemplate = _mapper.Map<List<GetEmailTemplateDTO>>(emailTemples);

                return new ResponseDTO
                {
                    IsSuccess = true,
                    Result = emailTemples,
                    Message = "Lấy danh sách template email thành công.",
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO
                {
                    IsSuccess = false,
                    Message = $"Lỗi khi lấy danh sách template email: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<ResponseDTO> GetEmailTemplateById(Guid templateId)
        {
            try
            {
                var emailTemplate = await _unitOfWork.EmailTemplateRepository.GetByIdAsync(templateId);
                if (emailTemplate == null)
                {
                    return new ResponseDTO
                    {
                        IsSuccess = false,
                        Message = $"Không tìm thấy template với ID: {templateId}",
                        StatusCode = 404
                    };
                }

                return new ResponseDTO
                {
                    IsSuccess = true,
                    Result = emailTemplate,
                    Message = "Lấy thông tin template email thành công.",
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO
                {
                    IsSuccess = false,
                    Message = $"Lỗi khi lấy thông tin template email: {ex.Message}",
                    StatusCode = 500
                };
            }
        }
        public async Task<bool> IsAllowToSendEmail(string email, string key)
        {
            var isAllowed = true;
            var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            await _redisService.RemoveRangeByScoreAsync(key, double.NegativeInfinity, now - 300);

            var count = await _redisService.SortedSetLengthAsync(key);
            if (count >= 5)
            {
                var last = await _redisService.GetSortedSetDescByScoreAsync(key, false, 1);
                if (last != null && last.Length > 0)
                {
                    var lastSentTime = last[0].Score;
                    if (now - lastSentTime < 300) // 5 phút
                    {
                        isAllowed = false;
                    }
                    else
                    {
                        await _redisService.DeleteKeyAsync(key);
                    }
                }
            }
            else
            {
                await _redisService.AddToSortedSetAsync(key, now.ToString(), now);
                await _redisService.ExpireKeyAsync(key, TimeSpan.FromMinutes(5));
            }

            return isAllowed;
        }
    }
}

