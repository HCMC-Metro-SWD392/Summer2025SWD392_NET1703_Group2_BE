using AutoMapper;
using MetroTicketBE.Application.IService;
using MetroTicketBE.Domain.Constants;
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
        private readonly IMapper _mapper;
        private readonly IS3Service _s3Service;
        public FormRequestService(IUnitOfWork unitOfWork, IMapper mapper, IS3Service s3Service)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork), "UnitOfWork cannot be null.");
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper), "Mapper cannot be null.");
            _s3Service = s3Service ?? throw new ArgumentNullException(nameof(s3Service), "S3Service cannot be null.");
        }

        public async Task<ResponseDTO> ChangeFormRequestStatus(ClaimsPrincipal user, Guid formRequestId, ChangeFormStatusDTO changeFormStatusDTO)
        {
            try
            {
                var formRequest = await _unitOfWork.FormRequestRepository.GetByIdAsync(formRequestId);
                if (formRequest is null)
                {
                    return new ResponseDTO
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy yêu cầu",
                        StatusCode = 404
                    };
                }

                var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId is null)
                {
                    return new ResponseDTO
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy người dùng hiện đang truy cập",
                        StatusCode = 404
                    };
                }

                var sender = await _unitOfWork.CustomerRepository.GetByUserIdAsync(formRequest.SenderId);
                if (sender is null)
                {
                    return new ResponseDTO
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy người gửi yêu cầu",
                        StatusCode = 404
                    };
                }

                sender.CustomerType = changeFormStatusDTO.CustomerType;
                _unitOfWork.CustomerRepository.Update(sender);


                formRequest.Status = changeFormStatusDTO.FormStatus;
                formRequest.RejectionReason = changeFormStatusDTO.RejectionReason;
                formRequest.ReviewerId = userId;
                _unitOfWork.FormRequestRepository.Update(formRequest);

                await _unitOfWork.SaveAsync();

                return new ResponseDTO
                {
                    IsSuccess = true,
                    Message = "Cập nhật trạng thái yêu cầu thành công",
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO
                {
                    IsSuccess = false,
                    Message = "Đã xảy ra lỗi khi cập nhật trạng thái yêu cầu: " + ex.Message,
                    StatusCode = 500
                };
            }
        }
        public async Task<ResponseDTO> GetAll(string sortBy, FormStatus formStatus, bool? isAcsending, int pageNumber, int pageSize)
        {
            try
            {
                var formRequests = (await _unitOfWork.FormRequestRepository.GetAllAsync())
                    .Where(fr => fr.Status == formStatus);
                if (formRequests is null || !formRequests.Any())
                {
                    return new ResponseDTO
                    {
                        IsSuccess = false,
                        Message = "Không có yêu cầu nào",
                        StatusCode = 404
                    };
                }

                if (!string.IsNullOrEmpty(sortBy))
                {
                    formRequests = sortBy.ToLower() switch
                    {
                        "createdat" => isAcsending.HasValue && isAcsending.Value ?
                            formRequests.OrderBy(fr => fr.CreatedAt).ToList() :
                            formRequests.OrderByDescending(fr => fr.CreatedAt),

                        _ => formRequests
                    };
                }

                if (pageNumber < 1 || pageSize < 1)
                {
                    return new ResponseDTO
                    {
                        IsSuccess = false,
                        Message = "Số trang hoặc kích thước trang không hợp lệ",
                        StatusCode = 400
                    };
                }
                else
                {
                    formRequests.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                }

                var getFormRequests = _mapper.Map<List<GetFormRequestDTO>>(formRequests);

                return new ResponseDTO
                {
                    IsSuccess = true,
                    Message = "Lấy danh sách yêu cầu thành công",
                    StatusCode = 200,
                    Result = getFormRequests
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO
                {
                    IsSuccess = false,
                    Message = "Đã xảy ra lỗi khi lấy danh sách yêu cầu: " + ex.Message,
                    StatusCode = 500
                };
            }
        }

        public async Task<ResponseDTO> GetAllFormAttachment(Guid formRequestId)
        {
            try
            {
                var formRequest = await _unitOfWork.FormRequestRepository.GetByIdAsync(formRequestId);
                if (formRequest is null)
                {
                    return new ResponseDTO
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy yêu cầu",
                        StatusCode = 404
                    };
                }

                var urls = new List<string>();

                foreach (var attachment in formRequest.FormAttachments)
                {
                    var presignedUrl = _s3Service.GenerateDownloadUrl(attachment.Url);
                    urls.Add(presignedUrl);
                }

                if (!urls.Any())
                {
                    return new ResponseDTO
                    {
                        IsSuccess = false,
                        Message = "Không có tệp đính kèm nào",
                        StatusCode = 404
                    };
                }

                return new ResponseDTO
                {
                    IsSuccess = true,
                    Message = "Lấy danh sách tệp đính kèm thành công",
                    StatusCode = 200,
                    Result = urls
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO
                {
                    IsSuccess = false,
                    Message = "Đã xảy ra lỗi khi lấy danh sách tệp đính kèm: " + ex.Message,
                    StatusCode = 500
                };
            }
        }

        public async Task<ResponseDTO> GetFormRequest(ClaimsPrincipal user)
        {
            try
            {
                var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId is null)
                {
                    return new ResponseDTO
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy người dùng",
                        StatusCode = 400
                    };
                }

                var formRequests = (await _unitOfWork.FormRequestRepository.GetAllAsync())
                    .Where(fr => fr.SenderId == userId).OrderByDescending(fr => fr.CreatedAt).ToList();

                if (formRequests.Count == 0)
                {
                    return new ResponseDTO
                    {
                        IsSuccess = false,
                        Message = "Không có yêu cầu nào",
                        StatusCode = 404
                    };
                }

                var getFormRequest = _mapper.Map<List<GetFormRequestDTO>>(formRequests);

                return new ResponseDTO
                {
                    IsSuccess = true,
                    Message = "Lấy danh sách yêu cầu thành công",
                    StatusCode = 200,
                    Result = getFormRequest
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO
                {
                    IsSuccess = false,
                    Message = "Đã xảy ra lỗi khi lấy danh sách yêu cầu: " + ex.Message,
                    StatusCode = 500
                };
            }
        }

        public async Task<ResponseDTO> CreateFormRequest(ClaimsPrincipal user, CreateFormRequestDTO createFormRequestDTO)
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

                var isPendingFormRequest = await _unitOfWork.FormRequestRepository.IsPendingFormRequest(senderId);
                if (isPendingFormRequest)
                {
                    return new ResponseDTO
                    {
                        IsSuccess = false,
                        Message = "Bạn đã có một yêu cầu đang chờ duyệt. Chờ đợi là hạnh phúc",
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
