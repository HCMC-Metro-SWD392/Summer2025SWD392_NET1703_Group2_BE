using MetroTicketBE.Application.IService;
using MetroTicketBE.Domain.DTO.Auth;
using MetroTicketBE.Domain.DTO.TrainSchedule;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MetroTicketBE.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrainScheduleController : ControllerBase
    {
        private readonly ITrainScheduleService _trainScheduleService;
        public TrainScheduleController(ITrainScheduleService trainScheduleService)
        {
            _trainScheduleService = trainScheduleService ?? throw new ArgumentNullException(nameof(trainScheduleService));
        }

        [HttpGet]
        [Route("get-train-schedules/{trainScheduleId}")]
        public async Task<ActionResult<ResponseDTO>> GetTrainSchedules(Guid trainScheduleId)
        {
            if (trainScheduleId == Guid.Empty)
            {
                return BadRequest("Invalid train schedule ID.");
            }
            var response = await _trainScheduleService.GetTrainSchedules(trainScheduleId);
            return StatusCode(response.StatusCode, response);
        }

        // [HttpPost]
        // [Route("create-train-schedule")]
        // public async Task<ActionResult<ResponseDTO>> CreateTrainSchedule([FromBody] Guid metroLineId)
        // {
        //     if (metroLineId == Guid.Empty)
        //     {
        //         return BadRequest("Invalid metro line ID.");
        //     }
        //     var response = await _trainScheduleService.GenerateTrainSchedules(metroLineId);
        //     return StatusCode(response.StatusCode, response);
        // }
        
        [HttpPut]
        [Route("update-train-schedule")]
        public async Task<ActionResult<ResponseDTO>> UpdateTrainSchedule([FromBody] UpdateTrainScheduleDTO updateTrainScheduleDTO)
        {
            if (updateTrainScheduleDTO is null || updateTrainScheduleDTO.Id == Guid.Empty)
            {
                return BadRequest("Invalid train schedule data.");
            }
            var response = await _trainScheduleService.UpdateTrainSchedule(updateTrainScheduleDTO);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPut]
        [Route("cancel-train-schedule/{trainScheduleId}")]
        public async Task<ActionResult<ResponseDTO>> CancelTrainSchedule(Guid trainScheduleId)
        {
            if (trainScheduleId == Guid.Empty)
            {
                return BadRequest("Invalid train schedule ID.");
            }
            var response = await _trainScheduleService.CancelTrainSchedule(trainScheduleId);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet]
        [Route("get-all-metro-schedule")]
        public async Task<ActionResult<ResponseDTO>> GetAll(
            [FromQuery] string? filterOn,
            [FromQuery] string? filterQuery,
            [FromQuery] string? sortBy,
            [FromQuery] bool? isAcsending,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var response = await _trainScheduleService.GetAll(filterOn, filterQuery, sortBy, isAcsending, pageNumber, pageSize);
            return StatusCode(response.StatusCode, response);
        }
    }
}
