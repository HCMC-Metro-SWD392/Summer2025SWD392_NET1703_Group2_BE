using MetroTicketBE.Domain.DTO.Auth;
using MetroTicketBE.Domain.DTO.News;
using MetroTicketBE.Domain.Enums;
using System.Security.Claims;

namespace MetroTicketBE.Application.IService
{
    public interface INewsService
    {
        public Task<ResponseDTO> CreateNews(ClaimsPrincipal user, CreateNewsDTO createNewsDTO);
        public Task<ResponseDTO> GetAllNewsListForManager
            (
            string? filterOn,
            string? filerQuery,
            NewsStatus status,
            int pageNumber,
            int pageSize
            );
        public Task<ResponseDTO> GetNewsById(Guid newsId);
        public Task<ResponseDTO> UpdateNews(ClaimsPrincipal user, Guid newsId, UpdateNewsDTO updateNewsDTO);
        public Task<ResponseDTO> DeleteNews(Guid newsId);

        public Task<ResponseDTO> GetAllNewsListForUser
            (
            string? filterOn,
            string? filerQuery,
            int pageNumber,
            int pageSize
            );

        public Task<ResponseDTO> GetAllNewsListForStaff
            (
            ClaimsPrincipal user,
            string? filterOn,
            string? filerQuery,
            NewsStatus status,
            int pageNumber,
            int pageSize
            );

        public Task<ResponseDTO> ChangeNewsStatus(Guid newsId, ChangeStatusDTO changeStatusDTO);
    }
}
