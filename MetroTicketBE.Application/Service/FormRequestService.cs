using MetroTicketBE.Application.IService;
using MetroTicketBE.Domain.DTO.Auth;
using MetroTicketBE.Domain.DTO.FormRequest;
using MetroTicketBE.Domain.Entities;
using MetroTicketBE.Domain.Enum;
using MetroTicketBE.Infrastructure.IRepository;
using System.Security.Claims;

namespace MetroTicketBE.Application.Service
{
    public class FormRequestService : IFormRequestService
    {
        private readonly IUnitOfWork _unitOfWork;
        public FormRequestService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork), "UnitOfWork cannot be null.");
        }
        public async Task<ResponseDTO> SendFormRequest(ClaimsPrincipal user, CreateFormRequestDTO createFormRequestDTO)
        {
            try
            {
                var senderId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (senderId is null)
                {
                    return new ResponseDTO
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy người dùng",
                        StatusCode = 400
                    };
                }

                var formRequest = new FormRequest
                {
                    SenderId = senderId,
                    Title = createFormRequestDTO.Title,
                    Content = createFormRequestDTO.Content,
                    FormRequestType = createFormRequestDTO.FormRequestType,
                    Status = FormStatus.Pending
                };

                // Generate the download URL for the attachment
                foreach (var key in createFormRequestDTO.AttachmentKeys)
                {
                    var fileName = Path.GetFileName(key);
                    formRequest.FormAttachments.Add(new FormAttachment
                    {
                        Url = key,
                        FileName = fileName
                    });
                }

                await _unitOfWork.FormRequestRepository.AddAsync(formRequest);
                await _unitOfWork.SaveAsync();

                return new ResponseDTO
                {
                    IsSuccess = true,
                    Message = "Yêu cầu đã được gửi thành công",
                    StatusCode = 201
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO
                {
                    IsSuccess = false,
                    Message = "Đã xảy ra lỗi khi gửi yêu cầu: " + ex.Message,
                    StatusCode = 500
                };
            }
        }
    }
}
