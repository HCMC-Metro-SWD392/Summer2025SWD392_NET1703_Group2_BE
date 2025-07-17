using MetroTicketBE.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace MetroTicketBE.Domain.DTO.Promotion
{
    public class GetPromotionDTO
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = null!;
        [Range(0, 100)]
        public decimal? Percentage { get; set; }
        public int? FixedAmount { get; set; }
        public PromotionType PromotionType { get; set; }
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Boolean IsActive { get; set; } 
    }
}
