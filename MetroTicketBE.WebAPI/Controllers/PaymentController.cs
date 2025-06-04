using MetroTicketBE.Application.IService;
using MetroTicketBE.Domain.DTO.Auth;
using MetroTicketBE.Domain.DTO.Payment;
using Microsoft.AspNetCore.Mvc;

namespace MetroTicketBE.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService ?? throw new ArgumentNullException(nameof(paymentService));
        }
        [HttpPost]
        [Route("create-link-payment-ticket-route")]
        public async Task<ActionResult<ResponseDTO>> CreateLinkPaymentTicketRoute([FromBody] CreateLinkPaymentRoutePayOSDTO createLinkDTO)
        {
            var response = await _paymentService.CreateLinkPaymentTicketRoutePayOS(User, createLinkDTO);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPut]
        [Route("payment-transactions/{id}/update-status")]
        public async Task<ActionResult<ResponseDTO>> UpdatePaymentTickerStatusPayOS([FromRoute] Guid paymentTransactionId)
        {
            var response = await _paymentService.UpdatePaymentTickerStatusPayOS(User, paymentTransactionId);
            return StatusCode(response.StatusCode, response);
        }
    }
}
