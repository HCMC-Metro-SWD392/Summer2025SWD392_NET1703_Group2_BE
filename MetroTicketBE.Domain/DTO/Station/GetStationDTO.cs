namespace MetroTicketBE.Domain.DTO.Station;

public class GetStationDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Address { get; set; }
    public string? Description { get; set; }
}