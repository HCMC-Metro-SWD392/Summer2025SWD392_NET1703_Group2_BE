using System.ComponentModel.DataAnnotations.Schema;

namespace MetroTicket.Domain.Entities
{
    public class LogType
    {
        public Guid Id { get; set; }
        public string? TypeName { get; set; }
        public virtual ICollection<Log> Logs { get; set; }
    }
}
