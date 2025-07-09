namespace MetroTicketBE.Domain.DTO.Station;

public class GetStationDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Address { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public List<MetroLineDTO> MetroLines { get; set; } = new List<MetroLineDTO>();
}

public class MetroLineDTO
{
    public Guid Id { get; set; }
    public string MetroName { get; set; } = null!;
    public StationMetroLineDTO StartStation { get; set; } = null!;
    public StationMetroLineDTO EndStation { get; set; } = null!;

}

public class StationMetroLineDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
}