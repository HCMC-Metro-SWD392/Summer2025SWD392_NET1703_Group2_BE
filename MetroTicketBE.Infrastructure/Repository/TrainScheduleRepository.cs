using MetroTicketBE.Domain.Entities;
using MetroTicketBE.Domain.Enums;
using MetroTicketBE.Infrastructure.Context;
using MetroTicketBE.Infrastructure.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetroTicketBE.Infrastructure.Repository
{
    public class TrainScheduleRepository : Repository<TrainSchedule>, ITrainScheduleRepository
    {
        private readonly ApplicationDBContext _context;
        public TrainScheduleRepository(ApplicationDBContext context) : base(context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<TrainSchedule?> GetByIdAsync(Guid id)
        {
            return await _context.TrainSchedules
                .Include(ts => ts.MetroLine)
                .Include(ts => ts.Station)
                .FirstOrDefaultAsync(ts => ts.Id == id);
        }

        public async Task<bool> IsExistTrainSchedule(Guid? metroLineId, Guid? stationId, TimeSpan? startTime, TrainScheduleType? direction)
        {
            return await _context.TrainSchedules
                .AnyAsync(ts => ts.MetroLineId == metroLineId &&
                                ts.StationId == stationId &&
                                ts.StartTime == startTime &&
                                ts.Direction == direction);
        }
    }
}
