using MetroTicketBE.Domain.DTO.Auth;
using MetroTicketBE.Domain.DTO.TrainSchedule;

namespace MetroTicketBE.Application.IService
{
    public interface ITrainScheduleService
    {
        Task<ResponseDTO> GetTrainSchedules(Guid trainScheduleId);
        Task<ResponseDTO> CreateTrainSchedule(CreateTrainScheduleDTO createTrainScheduleDTO);
        Task<ResponseDTO> GenerateTrainSchedules(Guid metroLineId);
        Task<ResponseDTO> UpdateTrainSchedule(UpdateTrainScheduleDTO updateTrainScheduleDTO);
        Task<ResponseDTO> CancelTrainSchedule(Guid trainScheduleId);
        Task<ResponseDTO> GetAll
            (
                string? filterOn,
                string? filterQuery,
                string? sortBy,
                bool? isAcsending,
                int pageNumber,
                int pageSize
            );
    }
}
