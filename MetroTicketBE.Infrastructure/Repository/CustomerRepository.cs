﻿using MetroTicketBE.Domain.Entities;
using MetroTicketBE.Domain.Enums;
using MetroTicketBE.Infrastructure.Context;
using MetroTicketBE.Infrastructure.IRepository;
using Microsoft.EntityFrameworkCore;

namespace MetroTicketBE.Infrastructure.Repository
{
    public class CustomerRepository : Repository<Customer>, ICustomerRepository
    {
        private readonly ApplicationDBContext _context;

        public CustomerRepository(ApplicationDBContext context) : base(context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Customer?> GetByIdAsync(Guid id)
        {
            return await _context.Customers
                .Include(c => c.User)
                .Include(m => m.Membership)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Customer?> GetByUserIdAsync(string userId)
        {
            return await _context.Customers
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }

        public async Task<Customer?> GetByEmailAsync(string email)
        {
            return await _context.Customers
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.User.Email == email);
        }

        public async Task<CustomerType> GetCustomerTypeByUserIdAsync(string userId)
        {
            return await _context.Customers.Include(c => c.User)
                .Where(c => c.UserId == userId)
                .Select(c => c.CustomerType)
                .FirstOrDefaultAsync();
        }

        public void Update(Customer customer)
        {
            _context.Customers.Update(customer);
        }

        public void UpdateRang(IEnumerable<Customer> customers)
        {
            _context.Customers.UpdateRange(customers);
        }

        public new async Task<Customer> AddAsync(Customer customer)
        {
            var entityEntry = await _context.Customers.AddAsync(customer);
            return entityEntry.Entity;
        }

    }
}
