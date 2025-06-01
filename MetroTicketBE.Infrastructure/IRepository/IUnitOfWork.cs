using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetroTicketBE.Infrastructure.IRepository
{
    public interface IUnitOfWork
    {
        ICustomerRepository CustomerRepository { get; }
        IUserManagerRepository UserManagerRepository { get; }
        IEmailTemplateRepository EmailTemplateRepository { get; }
        IPaymentMethodRepository PaymentMethodRepository { get; }
        IPaymentTransactionRepository PaymentTransactionRepository { get; }
        Task<int> SaveAsync();
    }
}
