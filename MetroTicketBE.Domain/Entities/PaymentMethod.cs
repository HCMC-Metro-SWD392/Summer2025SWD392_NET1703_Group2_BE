
namespace MetroTicketBE.Domain.Entities
{
    public class PaymentMethod
    {
        public Guid Id { get; set; }
        public required string MethodName { get; set; }
        public string? Description { get; set; }

        public ICollection<PayOSMethod> PayOSMethods { get; set; } = new List<PayOSMethod>();
        public ICollection<PaymentTransaction> Transactions { get; set; } = new List<PaymentTransaction>();
    }
}
