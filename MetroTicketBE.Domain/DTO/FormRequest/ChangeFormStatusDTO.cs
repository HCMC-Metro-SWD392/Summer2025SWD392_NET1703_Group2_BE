using MetroTicketBE.Domain.Enum;
public class ChangeFormStatusDTO
{
    public FormStatus FormStatus { get; set; }
    public string? RejectionReason { get; set; }
}
