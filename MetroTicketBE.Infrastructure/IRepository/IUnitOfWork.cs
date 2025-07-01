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
        IFareRuleRepository FareRuleRepository { get; }
        IPromotionRepository PromotionRepository { get; }
        IMetroLineRepository MetroLineRepository { get; }
        IStationRepository StationRepository { get; }
        ITicketRouteRepository TicketRouteRepository { get; }
        IMetroLineStationRepository MetroLineStationRepository { get; }
        ISubscriptionRepository SubscriptionRepository { get; }
        ITicketRepository TicketRepository { get; }
        ITrainScheduleRepository TrainScheduleRepository { get; }
        ISubscriptionTicketTypeRepository SubscriptionTicketTypeRepository { get; }
        IFormRequestRepository FormRequestRepository { get; }
        IStaffScheduleRepository StaffScheduleRepository { get; }
        IStaffShiftRepository StaffShiftRepository { get; }
        IStaffRepository StaffRepository { get; }
        ITicketProcessRepository TicketProcessRepository { get; }
        Task<int> SaveAsync();
    }
}
