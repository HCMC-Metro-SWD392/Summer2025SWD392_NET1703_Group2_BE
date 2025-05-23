namespace MetroTicketBE.Domain.Entities;

public class CustomerType
{
   public Guid Id { get; set; }
   public required string TypeName { get; set; }
   
   public ICollection<Customer> Customers { get; set; } = new List<Customer>();
}