﻿using MetroTicketBE.Application.IService;
using MetroTicketBE.Domain.DTO.Auth;
using MetroTicketBE.Domain.DTO.Payment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MetroTicketBE.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService ?? throw new ArgumentNullException(nameof(paymentService));
        }

        [HttpPost]
        [Route("create-link-payment-ticket-route")]
        public async Task<ActionResult<ResponseDTO>> CreateLinkPaymentTicketRoute([FromBody] CreateLinkPaymentPayOSDTO createLinkDTO)
        {
            var response = await _paymentService.CreateLinkPaymentTicketPayOS(User, createLinkDTO);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPut]
        [Route("payment-transactions/update-status/{orderCode}")]
        public async Task<ActionResult<ResponseDTO>> UpdatePaymentTickerStatusPayOS([FromRoute] string orderCode)
        {
            var response = await _paymentService.UpdatePaymentTickerStatusPayOS(User, orderCode);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost]
        [Route("create-link-payment-over-station-ticket-route")]
        public async Task<ActionResult<ResponseDTO>> CreateLinkPaymentOverStationForTicketRoute([FromBody] CreateLinkPaymentOverStationDTO createLinkPaymentOverStationDTO)
        {
            var response = await _paymentService.CreateLinkPaymentOverStationTicketRoutePayOS(User, createLinkPaymentOverStationDTO);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPut]
        [Route("payment-over-station-ticket-route/update-status/{orderCode}")]
        public async Task<ActionResult<ResponseDTO>> UpdatePaymentOverStationTicketRoutePayOS([FromRoute] string orderCode)
        {
            var response = await _paymentService.UpdatePaymentOverStationTicketRoutePayOS(User, orderCode);
            return StatusCode(response.StatusCode, response);
        }
    }
}
