using MetroTicketBE.Domain.DTO.Auth;

namespace MetroTicketBE.Application.IService
{
    public interface IDashBoardService
    {
        Task<ResponseDTO> ViewRevenueOverTime(DateTime dateFrom, DateTime dateTo);
        Task<ResponseDTO> ViewRevenueMonth(int monthAgo);
        Task<ResponseDTO> ViewRevenueYear(int year);
        Task<ResponseDTO> ViewTicketStatistics(DateTime dateFrom, DateTime dateTo, bool? isAccendingCreated, int pageNumber, int pageSize);
    }
}
