﻿using MetroTicketBE.Domain.Entities;
using MetroTicketBE.Domain.Enums;

namespace MetroTicketBE.Infrastructure.IRepository
{
    public interface ITrainScheduleRepository : IRepository<TrainSchedule>
    {
        Task<bool> IsExistTrainSchedule(
            Guid? metroLineId,
            Guid? stationId,
            TimeSpan? startTime,
            TrainScheduleType? direction);

        Task<TrainSchedule?> GetByIdAsync(Guid id);
        Task<List<TrainSchedule>> GetByStationIdSortedAsync(Guid stationId,  TrainScheduleType? direction);
        Task<List<TrainSchedule>> GetByMetroLineIdSortedAsync(Guid metroLineId);

    }
}
