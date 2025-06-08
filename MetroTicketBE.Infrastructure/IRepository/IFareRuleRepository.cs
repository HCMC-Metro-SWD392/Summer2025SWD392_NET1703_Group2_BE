using MetroTicketBE.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetroTicketBE.Infrastructure.IRepository
{
    public interface IFareRuleRepository : IRepository<FareRule>
    {
        Task<bool> IsExistById(Guid fareRuleId);
        Task<FareRule?> GetByIdAsync(Guid fareRuleId);
        Task<int> CalculatePriceFromDistance(double? distance);
    }
}
