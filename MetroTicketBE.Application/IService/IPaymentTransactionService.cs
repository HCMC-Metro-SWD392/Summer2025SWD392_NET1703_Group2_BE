using MetroTicketBE.Domain.DTO.Auth;

namespace MetroTicketBE.Application.IService
{
    public interface IPaymentTransactionService
    {
        Task<ResponseDTO> ViewRevenueOverTime(DateTime dateFrom, DateTime dateTo);
        Task<ResponseDTO> ViewRevenueMonth(int monthAgo);
        Task<ResponseDTO> ViewRevenueYear(int year);
    }
}
