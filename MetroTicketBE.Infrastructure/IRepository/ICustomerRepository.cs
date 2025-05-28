using MetroTicketBE.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetroTicketBE.Infrastructure.IRepository
{
    public interface ICustomerRepository : IRepository<Customer>
    {
        void Update(Customer customer);
        void UpdateRang(IEnumerable<Customer> customers);
        Task<Customer?> GetByIdAsync(Guid id);
        Task<Customer> AddAsync(Customer customer);
        Task<Customer?> GetByUserIdAsync(string userId);
    }
}
