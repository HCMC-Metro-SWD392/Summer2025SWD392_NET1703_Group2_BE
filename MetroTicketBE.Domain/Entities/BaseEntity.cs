using System.ComponentModel.DataAnnotations.Schema;

namespace MetroTicket.Domain.Entities
{
    public class BaseEntity<CID>
    {
        public CID? CreatedBy { get; set; }
        [ForeignKey("CreatedBy")]
        public virtual User? CreatedByUser { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
