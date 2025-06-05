using MetroTicketBE.Domain.DTO.MetroLineStation;
using MetroTicketBE.Domain.DTO.Station;

namespace MetroTicketBE.Domain.DTO.MetroLine;

public class GetMetroLineDTO
{
    public Guid Id { get; set; }
    public int MetroLineNumber { get; set; }
    public string? MetroName { get; set; }
    public GetStationDTO? StartStation { get; set; }
    public GetStationDTO? EndStation { get; set; }
    public List<GetMetroLineStationDTO> MetroLineStations { get; set; } = new List<GetMetroLineStationDTO>();
}