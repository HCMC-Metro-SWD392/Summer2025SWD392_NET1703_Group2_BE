using AutoMapper;
using MetroTicketBE.Application.IService;
using MetroTicketBE.Domain.DTO.Auth;
using MetroTicketBE.Domain.DTO.TicketProcess;
using MetroTicketBE.Infrastructure.IRepository;

namespace MetroTicketBE.Application.Service
{
    public class TicketProcessService : ITicketProcessService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public TicketProcessService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        public async Task<ResponseDTO> GetAllTicketProcessByTicketId(Guid ticketId)
        {
            try
            {
                var ticketProcesses = (await _unitOfWork.TicketProcessRepository.GetAllAsync(includeProperties: "Station,Ticket"))
                    .Where(tp => tp.TicketId == ticketId).ToList();
                if (ticketProcesses is null || !ticketProcesses.Any())
                {
                    return new ResponseDTO
                    {
                        Message = "Không tìm thấy quá trình nào.",
                        IsSuccess = false,
                        StatusCode = 404
                    };
                }
                var getTicketProcesses = _mapper.Map<List<GetTicketProcessDTO>>(ticketProcesses);
                return new ResponseDTO
                {
                    Message = "Lấy danh sách quá trình vé thành công",
                    IsSuccess = true,
                    StatusCode = 200,
                    Result = getTicketProcesses
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO
                {
                    Message = "Đã xảy ra lỗi khi lấy danh sách quá trình vé: " + ex.Message,
                    IsSuccess = false,
                    StatusCode = 500
                };
            }
        }
    }
}
