using MetroTicketBE.Domain.DTO.Auth;
using MetroTicketBE.Domain.Enum;

namespace MetroTicketBE.Application.IService
{
    public interface IDashBoardService
    {
        Task<ResponseDTO> ViewRevenueOverTime(DateTime dateFrom, DateTime dateTo);
        Task<ResponseDTO> ViewRevenueMonth(int monthAgo);
        Task<ResponseDTO> ViewRevenueYear(int year);
        Task<ResponseDTO> ViewTicketStatistics(DateTime dateFrom, DateTime dateTo, bool? isAccendingCreated, int pageNumber, int pageSize);
        Task<ResponseDTO> ViewTicketRouteStatisticsNumber(DateTime dateFrom, DateTime dateTo, PaymentStatus status);
        Task<ResponseDTO> ViewSubscriptionTicketStatisticsNumber(DateTime dateFrom, DateTime dateTo, PaymentStatus status);
        Task<ResponseDTO> ViewCustomerStatisticsNumber();

    }
}
