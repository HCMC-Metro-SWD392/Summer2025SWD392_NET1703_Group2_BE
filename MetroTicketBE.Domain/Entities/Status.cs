namespace MetroTicketBE.Domain.Entities;

public class Status
{
    public Guid Id { get; set; }
    public required string StatusCode { get; set; }
    public string StatusName { get; set; } = null!;
    public string Message { get; set; } = null!;
}