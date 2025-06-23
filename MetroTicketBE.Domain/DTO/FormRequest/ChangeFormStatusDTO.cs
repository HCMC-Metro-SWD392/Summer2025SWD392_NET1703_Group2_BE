using MetroTicketBE.Domain.Enum;
using MetroTicketBE.Domain.Enums;
public class ChangeFormStatusDTO
{
    public FormStatus FormStatus { get; set; }
    public string? RejectionReason { get; set; }
    public CustomerType CustomerType { get; set; }
}
