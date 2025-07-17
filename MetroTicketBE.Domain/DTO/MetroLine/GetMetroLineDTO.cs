using MetroTicketBE.Domain.DTO.MetroLineStation;
using MetroTicketBE.Domain.DTO.Station;
using MetroTicketBE.Domain.Enums;

namespace MetroTicketBE.Domain.DTO.MetroLine;

public class GetMetroLineDTO
{
    public Guid Id { get; set; }
    public string MetroLineNumber { get; set; } = null!;
    public string? MetroName { get; set; }
    public DateTime CreatedAt { get; set; }
    public GetStationDTO? StartStation { get; set; }
    public GetStationDTO? EndStation { get; set; }
    public bool IsActive { get; set; }
    public MetroLineStatus Status { get; set; }

    public List<GetMetroLineStationDTO> MetroLineStations { get; set; } = new List<GetMetroLineStationDTO>();
}