using MetroTicketBE.Domain.Enum;

namespace MetroTicketBE.Domain.Entities;

public class Train
{
    public Guid Id { get; set; }
    public string TrainCode { get; set; } = null!;
    public int TrainCarQuantity { get; set; }
    public double LoadCapacity { get; set; }

    public ICollection<TrainSchedule> TrainSchedules { get; set; } = null!;
}