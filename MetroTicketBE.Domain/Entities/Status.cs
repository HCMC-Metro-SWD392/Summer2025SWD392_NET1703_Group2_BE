namespace MetroTicketBE.Domain.Entities;

public class Status
{
    public Guid Id { get; set; }
    public required string StatusCode { get; set; }
    public string StatusName { get; set; } = null!;
    public string Message { get; set; } = null!;

    public ICollection<Train> Trains { get; set; } = new List<Train>();
    public ICollection<SubscriptionTicket> SubscriptionTickets { get; set; } = new List<SubscriptionTicket>();
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    public ICollection<Process> Processes { get; set; } = new List<Process>();
    public ICollection<EmailTemplate> EmailTemplates { get; set; } = new List<EmailTemplate>();
    public ICollection<PayOSMethod> PayOSMethods { get; set; } = new List<PayOSMethod>();
    public ICollection<FormRequest> FormRequests { get; set; } = new List<FormRequest>();
}