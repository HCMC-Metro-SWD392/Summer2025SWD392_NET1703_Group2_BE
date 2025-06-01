using MetroTicketBE.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetroTicketBE.Infrastructure.IRepository
{
    public interface IPromotionRepository : IRepository<Promotion>
    {
        Task<Promotion?> GetByCodeAsync(string code);
        Task<Promotion?> GetByIdAsync(Guid promotionId);
    }
}
