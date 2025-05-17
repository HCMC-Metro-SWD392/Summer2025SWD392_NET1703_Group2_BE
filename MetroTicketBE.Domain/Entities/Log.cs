using System.ComponentModel.DataAnnotations.Schema;

namespace MetroTicket.Domain.Entities
{
    public class Log : BaseEntity<String>
    {
        public Guid Id { get; set; }
        public Guid LogTypeId { get; set; }
        [ForeignKey("LogTypeId")]
        public virtual required LogType LogType { get; set; }
        public string? Description { get; set; }
    }
}
