using Amazon.SimpleEmailV2;
using AutoMapper;
using MetroTicketBE.Application.IService;
using MetroTicketBE.Domain.DTO.Auth;
using MetroTicketBE.Domain.DTO.News;
using MetroTicketBE.Domain.Entities;
using MetroTicketBE.Domain.Enums;
using MetroTicketBE.Infrastructure.IRepository;
using System.Security.Claims;
using MetroTicketBE.Domain.Enum;

namespace MetroTicketBE.Application.Service
{
    public class NewsService : INewsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogService _logService;
        private const string EntityName = "Tin tức";

        public NewsService(IUnitOfWork unitOfWork, IMapper mapper, ILogService logService)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logService = logService ?? throw new ArgumentNullException(nameof(logService));
        }

        public async Task<ResponseDTO> ChangeNewsStatus(ClaimsPrincipal user, Guid newsId, ChangeStatusDTO changeStatusDTO)
        {
            try
            {
                var news = await _unitOfWork.NewsRepository.GetByIdAsync(newsId);

                if (news is null)
                {
                    return new ResponseDTO
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy tin tức",
                        StatusCode = 404
                    };
                }

                news.Status = changeStatusDTO.Status;
                if (changeStatusDTO.Status == NewsStatus.Rejected)
                {
                    if (string.IsNullOrWhiteSpace(changeStatusDTO.RejectionReason))
                    {
                        return new ResponseDTO
                        {
                            IsSuccess = false,
                            Message = "Lý do từ chối không được để trống khi từ chối",
                            StatusCode = 400
                        };
                    }
                    news.RejectionReason = changeStatusDTO.RejectionReason;
                }
            
                await _logService.AddLogAsync(LogType.Update, user.FindFirstValue(ClaimTypes.NameIdentifier), EntityName, $"Thay đổi trạng thái tin tức: {news.Title} thành {changeStatusDTO.Status}");
                _unitOfWork.NewsRepository.Update(news);
                await _unitOfWork.SaveAsync();

                return new ResponseDTO
                {
                    IsSuccess = true,
                    Message = "Cập nhật trạng thái tin tức thành công",
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO
                {
                    IsSuccess = false,
                    Message = $"Lỗi khi cập nhật trạng thái tin tức: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<ResponseDTO> CreateNews(ClaimsPrincipal user, CreateNewsDTO createNewsDTO)
        {
            try
            {
                var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId is null)
                {
                    return new ResponseDTO
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy thông tin người dùng",
                        StatusCode = 404
                    };
                }

                var staff = await _unitOfWork.StaffRepository.GetByUserIdAsync(userId);
                if (staff is null)
                {
                    return new ResponseDTO
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy thông tin nhân viên",
                        StatusCode = 404
                    };
                }

                var news = new News
                {
                    Title = createNewsDTO.Title,
                    Content = createNewsDTO.Content,
                    Summary = createNewsDTO.Summary,
                    ImageUrl = createNewsDTO.ImageUrl,
                    Category = createNewsDTO.Category,
                    StaffId = staff.Id
                };

                await _logService.AddLogAsync(LogType.Create, user.FindFirstValue(ClaimTypes.NameIdentifier), EntityName, $"Tin tức: {news.Title}");
                await _unitOfWork.NewsRepository.AddAsync(news);
                await _unitOfWork.SaveAsync();

                return new ResponseDTO
                {
                    IsSuccess = true,
                    Message = "Tạo tin tức thành công",
                    StatusCode = 201,
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO
                {
                    IsSuccess = false,
                    Message = $"Lỗi khi tạo tin tức: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<ResponseDTO> DeleteNews(ClaimsPrincipal user, Guid newsId)
        {
            try
            {
                var news = await _unitOfWork.NewsRepository.GetByIdAsync(newsId);
                if (news is null)
                {
                    return new ResponseDTO
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy tin tức",
                        StatusCode = 404
                    };
                }
                
                await _logService.AddLogAsync(LogType.Delete, user.FindFirstValue(ClaimTypes.NameIdentifier), EntityName, $"Tin tức: {news.Title}");
                _unitOfWork.NewsRepository.Remove(news);
                await _unitOfWork.SaveAsync();

                return new ResponseDTO
                {
                    IsSuccess = true,
                    Message = "Xóa tin tức thành công",
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO
                {
                    IsSuccess = false,
                    Message = $"Lỗi khi xóa tin tức: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<ResponseDTO> GetAllNewsListForManager
            (
            string? filterOn,
            string? filerQuery,
            NewsStatus? status,
            int pageNumber,
            int pageSize
            )
        {
            try
            {
                var newsList = await _unitOfWork.NewsRepository.GetAllAsync();

                if (status.HasValue)
                {
                    newsList = newsList.Where(n => n.Status == status.Value);
                }

                if (!string.IsNullOrEmpty(filterOn) && !string.IsNullOrEmpty(filerQuery))
                {
                    filterOn = filterOn.ToLower().Trim();
                    filerQuery = filerQuery.ToLower().Trim();

                    newsList = filterOn switch
                    {
                        "title" => newsList.Where(n => n.Title.ToLower().Contains(filerQuery)),
                        "category" => newsList.Where(n => n.Category.ToLower().Contains(filerQuery)),
                        _ => newsList
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
                    newsList = newsList.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                }

                var getNewsList = _mapper.Map<List<GetNewsDTO>>(newsList);
                return new ResponseDTO
                {
                    IsSuccess = true,
                    Message = "Lấy danh sách tin tức thành công",
                    Result = getNewsList,
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO
                {
                    IsSuccess = false,
                    Message = $"Lỗi khi lấy danh sách tin tức: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<ResponseDTO> GetAllNewsListForStaff(ClaimsPrincipal user, string? filterOn, string? filerQuery, NewsStatus? status, int pageNumber, int pageSize)
        {
            try
            {
                var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (userId is null)
                {
                    return new ResponseDTO
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy thông tin người dùng",
                        StatusCode = 404
                    };
                }

                var staff = await _unitOfWork.StaffRepository.GetByUserIdAsync(userId);

                if (staff is null)
                {
                    return new ResponseDTO
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy thông tin nhân viên",
                        StatusCode = 404
                    };
                }

                var newsList = (await _unitOfWork.NewsRepository.GetAllAsync())
                    .Where(n => n.StaffId == staff.Id);

                if (status.HasValue)
                {
                    newsList = newsList.Where(n => n.Status == status.Value);
                }

                    if (!string.IsNullOrEmpty(filterOn) && !string.IsNullOrEmpty(filerQuery))
                {
                    filterOn = filterOn.ToLower().Trim();
                    filerQuery = filerQuery.ToLower().Trim();

                    newsList = filterOn switch
                    {
                        "title" => newsList.Where(n => n.Title.ToLower().Contains(filerQuery)),
                        "category" => newsList.Where(n => n.Category.ToLower().Contains(filerQuery)),
                        _ => newsList
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
                    newsList = newsList.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                }

                var getNewsList = _mapper.Map<List<GetNewsDTO>>(newsList);
                return new ResponseDTO
                {
                    IsSuccess = true,
                    Message = "Lấy danh sách tin tức thành công",
                    Result = getNewsList,
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO
                {
                    IsSuccess = false,
                    Message = $"Lỗi khi lấy danh sách tin tức: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<ResponseDTO> GetAllNewsListForUser(string? filterOn, string? filerQuery, int pageNumber, int pageSize)
        {
            try
            {
                var newsList = (await _unitOfWork.NewsRepository.GetAllAsync())
                    .Where(n => n.Status == NewsStatus.Published);

                if (!string.IsNullOrEmpty(filterOn) && !string.IsNullOrEmpty(filerQuery))
                {
                    filterOn = filterOn.ToLower().Trim();
                    filerQuery = filerQuery.ToLower().Trim();

                    newsList = filterOn switch
                    {
                        "title" => newsList.Where(n => n.Title.ToLower().Contains(filerQuery)),
                        "category" => newsList.Where(n => n.Category.ToLower().Contains(filerQuery)),
                        _ => newsList
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
                    newsList = newsList.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                }

                var getNewsList = _mapper.Map<List<GetNewsDTO>>(newsList);
                return new ResponseDTO
                {
                    IsSuccess = true,
                    Message = "Lấy danh sách tin tức thành công",
                    Result = getNewsList,
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO
                {
                    IsSuccess = false,
                    Message = $"Lỗi khi lấy danh sách tin tức: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<ResponseDTO> GetNewsById(Guid newsId)
        {
            try
            {
                var news = await _unitOfWork.NewsRepository.GetByIdAsync(newsId);
                if (news is null)
                {
                    return new ResponseDTO
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy tin tức",
                        StatusCode = 404
                    };
                }
                var getNews = _mapper.Map<GetNewsDTO>(news);
                return new ResponseDTO
                {
                    IsSuccess = true,
                    Result = getNews,
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO
                {
                    IsSuccess = false,
                    Message = $"Lỗi khi lấy tin tức: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<ResponseDTO> UpdateNews(ClaimsPrincipal user, Guid newsId, UpdateNewsDTO updateNewsDTO)
        {
            try
            {
                var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId is null)
                {
                    return new ResponseDTO
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy thông tin người dùng",
                        StatusCode = 404
                    };
                }
                var staff = await _unitOfWork.StaffRepository.GetByUserIdAsync(userId);
                if (staff is null)
                {
                    return new ResponseDTO
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy thông tin nhân viên",
                        StatusCode = 404
                    };
                }
                var news = await _unitOfWork.NewsRepository.GetByIdAsync(newsId);
                if (news is null)
                {
                    return new ResponseDTO
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy tin tức",
                        StatusCode = 404
                    };
                }

                if (news.StaffId != staff.Id)
                {
                    return new ResponseDTO
                    {
                        IsSuccess = false,
                        Message = "Bạn không có quyền sửa tin tức này",
                        StatusCode = 403
                    };
                }

                var isUpdated = false;

                if (!string.IsNullOrWhiteSpace(updateNewsDTO.Title))
                {
                    news.Title = updateNewsDTO.Title;
                    isUpdated = true;
                }

                if (!string.IsNullOrWhiteSpace(updateNewsDTO.Content))
                {
                    news.Content = updateNewsDTO.Content;
                    isUpdated = true;
                }


                if (!string.IsNullOrWhiteSpace(updateNewsDTO.Summary))
                {
                    news.Summary = updateNewsDTO.Summary;
                    isUpdated = true;
                }

                if (!string.IsNullOrWhiteSpace(updateNewsDTO.ImageUrl))
                {
                    news.ImageUrl = updateNewsDTO.ImageUrl;
                    isUpdated = true;
                }

                if (!string.IsNullOrWhiteSpace(updateNewsDTO.Category))
                {
                    news.Category = updateNewsDTO.Category;
                    isUpdated = true;
                }

                if (isUpdated)
                {
                    news.UpdatedAt = DateTime.UtcNow;
                    news.Status = NewsStatus.Updated;

                    _unitOfWork.NewsRepository.Update(news);
                    await _unitOfWork.SaveAsync();
                    return new ResponseDTO
                    {
                        IsSuccess = true,
                        Message = "Cập nhật tin tức thành công",
                        StatusCode = 200
                    };
                }
                else
                {
                    return new ResponseDTO
                    {
                        IsSuccess = false,
                        Message = "Không có thông tin nào để cập nhật",
                        StatusCode = 400
                    };
                }
            }
            catch (Exception ex)
            {
                return new ResponseDTO
                {
                    IsSuccess = false,
                    Message = $"Lỗi khi cập nhật tin tức: {ex.Message}",
                    StatusCode = 500
                };
            }
        }
    }
}
