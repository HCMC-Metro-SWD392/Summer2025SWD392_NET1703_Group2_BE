using MetroTicket.Domain.Entities;
using MetroTicketBE.Infrastructure.Context;
using MetroTicketBE.Infrastructure.IRepository;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetroTicketBE.Infrastructure.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDBContext _context;
        public ICustomerRepository CustomerRepository { get; private set; }
        public IUserManagerRepository UserManagerRepository { get; private set; }
        public IEmailTemplateRepository EmailTemplateRepository { get; private set; }
        public IPaymentMethodRepository PaymentMethodRepository { get; private set; }
        public IPaymentTransactionRepository PaymentTransactionRepository { get; private set; }
        public IFareRuleRepository FareRuleRepository { get; private set; }
        public IPromotionRepository PromotionRepository { get; private set; }
        public IMetroLineRepository MetroLineRepository { get; private set; }
        public IStationRepository StationRepository { get; private set; }
        public ITicketRouteRepository TicketRouteRepository { get; private set; }
        public IMetroLineStationRepository MetroLineStationRepository { get; private set; }

        public UnitOfWork(ApplicationDBContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));

            CustomerRepository = new CustomerRepository(_context);
            UserManagerRepository = new UserManagerRepository(userManager);
            EmailTemplateRepository = new EmailTemplateRepository(_context);
            PaymentMethodRepository = new PaymentMethodRepository(_context);
            PaymentTransactionRepository = new PaymentTransactionRepository(_context);
            FareRuleRepository = new FareRuleRepository(_context);
            PromotionRepository = new PromotionRepository(_context);
            MetroLineRepository = new MetroLineRepository(_context);
            StationRepository = new StationRepository(_context);
            TicketRouteRepository = new TicketRouteRepository(_context);
            MetroLineStationRepository = new MetroLineStationRepository(_context);
        }
        public async Task<int> SaveAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
