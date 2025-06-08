using MetroTicketBE.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetroTicketBE.Domain.DTO.Promotion
{
    public class UpdatePromotionDTO
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
    }
}
