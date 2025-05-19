namespace MetroTicketBE.Domain.Entities;

public class FormRequestType
{
    public Guid Id { get; set; }
    public required string Name { get; set; }

    public ICollection<FormRequest> FormRequests { get; set; } = new List<FormRequest>();
}