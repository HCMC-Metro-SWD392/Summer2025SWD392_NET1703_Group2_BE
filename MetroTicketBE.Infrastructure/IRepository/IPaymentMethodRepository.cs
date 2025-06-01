using MetroTicketBE.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetroTicketBE.Infrastructure.IRepository
{
    public interface IPaymentMethodRepository : IRepository<PaymentMethod>
    {
        Task<PaymentMethod?> GetByNameAsync(string methodName);
    }
}
