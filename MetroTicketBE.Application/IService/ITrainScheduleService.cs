using MetroTicketBE.Domain.DTO.Auth;
using MetroTicketBE.Domain.DTO.TrainSchedule;
using MetroTicketBE.Domain.Enums;

namespace MetroTicketBE.Application.IService
{
    public interface ITrainScheduleService
    {
        Task<ResponseDTO> GetTrainSchedules(Guid trainScheduleId);
        Task<ResponseDTO> GetTrainSchedulesByStationId(Guid stationId,  TrainScheduleType? direction);
        Task<ResponseDTO> GenerateScheduleForMetroLine(CreateTrainScheduleDTO createTrainScheduleDTO);
        //Task<ResponseDTO> CreateMetroSchedule (Guid metroLineId);
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
