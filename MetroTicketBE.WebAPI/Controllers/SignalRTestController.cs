using MetroTicketBE.Domain.Constants;
using MetroTicketBE.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

[ApiController]
[Route("api/test-signalr")]
public class SignalRTestController : ControllerBase
{
    private readonly IHubContext<NotificationHub> _hubContext;

    public SignalRTestController(IHubContext<NotificationHub> hubContext)
    {
        _hubContext = hubContext;
    }

    [HttpPost]
    [Route("send-test-message")]
    [Authorize(Roles = StaticUserRole.ManagerAdmin)]
    public async Task<IActionResult> SendTestMessage()
    {
        await _hubContext.Clients.All.SendAsync("ReceiveNotification", "🚀 Thử SignalR thành công!");
        return Ok(new { message = "Đã gửi" });
    }
}
