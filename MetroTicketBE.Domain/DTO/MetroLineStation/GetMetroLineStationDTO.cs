using MetroTicketBE.Domain.DTO.Station;

namespace MetroTicketBE.Domain.DTO.MetroLineStation;

public class GetMetroLineStationDTO
{
    public Guid Id { get; set; }
    public double DistanceFromStart { get; set; }
    public int StationOrder { get; set; }
    public GetStationDTO Station { get; set; } = null!;
}
