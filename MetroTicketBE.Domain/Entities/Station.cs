namespace MetroTicketBE.Domain.Entities;

public class Station
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public string Address { get; set; } = null!;
    public string Description { get; set; } = null!;

    public ICollection<TicketRoute> TicketRoutesAsFirstStation { get; set; } = new List<TicketRoute>();
    public ICollection<TicketRoute> TicketRoutesAsLastStation { get; set; } = new List<TicketRoute>();
    
    public ICollection<Process> CheckInProcesses { get; set; } = new List<Process>();
    public ICollection<Process> CheckOutProcesses { get; set; } = new List<Process>();

    public ICollection<MetroLine> StartStations { get; set; } = new List<MetroLine>();
    public ICollection<MetroLine> EndStations { get; set; } = new List<MetroLine>();

    public ICollection<TrainSchedule> StrainSchedules { get; set; } = new List<TrainSchedule>();

    public ICollection<MetroLineStation> MetroLineStations { get; set; } = new List<MetroLineStation>();

}