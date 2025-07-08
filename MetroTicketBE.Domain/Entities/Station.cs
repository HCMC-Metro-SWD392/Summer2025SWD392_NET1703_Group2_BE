namespace MetroTicketBE.Domain.Entities;

public class Station
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public string Address { get; set; } = null!;
    public string Description { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;
    public ICollection<TicketRoute> TicketRoutesAsFirstStation { get; set; } = new List<TicketRoute>();
    public ICollection<TicketRoute> TicketRoutesAsLastStation { get; set; } = new List<TicketRoute>();
    public ICollection<SubscriptionTicket> SubscriptionTicketsAsStartStation { get; set; } = new List<SubscriptionTicket>();
    public ICollection<SubscriptionTicket> SubscriptionTicketsAsEndStation { get; set; } = new List<SubscriptionTicket>();
    public ICollection<MetroLine> StartStations { get; set; } = new List<MetroLine>();
    public ICollection<MetroLine> EndStations { get; set; } = new List<MetroLine>();
    public ICollection<TrainSchedule> StrainSchedules { get; set; } = new List<TrainSchedule>();
    public ICollection<MetroLineStation> MetroLineStations { get; set; } = new List<MetroLineStation>();
    public ICollection<StaffSchedule> StaffSchedules { get; set; } = new List<StaffSchedule>();
    public ICollection<TicketProcess> TicketProcesses { get; set; } = new List<TicketProcess>();
}