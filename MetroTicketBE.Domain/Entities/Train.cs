namespace MetroTicketBE.Domain.Entities;

public class Train
{
    public Guid Id { get; set; }
    public string TrainCode { get; set; } = null!;
    public int TrainCarQuantity { get; set; }
    public double LoadCapacity { get; set; }
    public Guid TimeLineId { get; set; }
    public Guid StatusId { get; set; }
    
    public TimeLine TimeLine { get; set; } = null!;
    public Status Status { get; set; } = null!;
}