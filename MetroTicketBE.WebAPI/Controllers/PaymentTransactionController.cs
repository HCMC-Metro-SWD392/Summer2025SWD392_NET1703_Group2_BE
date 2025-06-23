using MetroTicketBE.Application.IService;
using MetroTicketBE.Domain.DTO.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MetroTicketBE.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentTransactionController : ControllerBase
    {
        private readonly IPaymentTransactionService _paymentTransactionService;
        public PaymentTransactionController(IPaymentTransactionService paymentTransactionService)
        {
            _paymentTransactionService = paymentTransactionService ?? throw new ArgumentNullException(nameof(paymentTransactionService));
        }
        [HttpGet]
        [Route("revenue-month")]
        public async Task<ActionResult<ResponseDTO>> ViewRevenueMonth([FromQuery] int month)
        {
            var response = await _paymentTransactionService.ViewRevenueMonth(month);
            return StatusCode(response.StatusCode, response);
        }
        [HttpGet]
        [Route("revenue/overtime")]
        public async Task<ActionResult<ResponseDTO>> ViewRevenueOverTime([FromQuery] DateTime dateFrom, [FromQuery] DateTime dateTo)
        {
            var response = await _paymentTransactionService.ViewRevenueOverTime(dateFrom, dateTo);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet]
        [Route("revenue-year")]
        public async Task<ActionResult<ResponseDTO>> ViewRevenueYear([FromQuery] int year)
        {
            var response = await _paymentTransactionService.ViewRevenueYear(year);
            return StatusCode(response.StatusCode, response);

        }
    }
}
