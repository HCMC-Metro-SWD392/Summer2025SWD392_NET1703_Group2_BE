using MetroTicket.Domain.Entities;
using MetroTicketBE.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetroTicketBE.Domain.Enums;

namespace MetroTicketBE.Infrastructure.IRepository
{
    public interface ICustomerRepository : IRepository<Customer>
    {
        void Update(Customer customer);
        void UpdateRang(IEnumerable<Customer> customers);
        Task<Customer?> GetByIdAsync(Guid id);
        Task<Customer?> GetByEmailAsync(string email);
        Task<Customer> AddAsync(Customer customer);
        Task<Customer?> GetByUserIdAsync(string userId);
        Task<CustomerType> GetCustomerTypeByUserIdAsync(string userId);
    }
}
