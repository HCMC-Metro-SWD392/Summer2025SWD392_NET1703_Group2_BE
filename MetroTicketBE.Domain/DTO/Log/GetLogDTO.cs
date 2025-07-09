using MetroTicketBE.Domain.Enum;

namespace MetroTicketBE.Domain.DTO.Log;

public class GetLogDTO
{
    public string Description { get; set; } = string.Empty;
    public string UserFullname { get; set; } = string.Empty;
    public string LogType { get; set; }
    public DateTime CreatedAt { get; set; }
}