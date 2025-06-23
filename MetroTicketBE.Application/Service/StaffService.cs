using AutoMapper;
using MetroTicketBE.Application.IService;
using MetroTicketBE.Domain.DTO.Auth;
using MetroTicketBE.Domain.DTO.Staff;
using MetroTicketBE.Infrastructure.IRepository;

namespace MetroTicketBE.Application.Service
{
    public class StaffService : IStaffService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public StaffService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        public async Task<ResponseDTO> GetAllStaff()
        {
            try
            {
                var staffList = await _unitOfWork.StaffRepository.GetAllAsync(includeProperties: "User");
                if (staffList == null || !staffList.Any())
                {
                    return new ResponseDTO
                    {
                        IsSuccess = false,
                        StatusCode = 404,
                        Message = "Danh sách nhân viên rỗng."
                    };
                }

                var getStaffList = _mapper.Map<List<GetStaffDTO>>(staffList);

                return new ResponseDTO
                {
                    Result = getStaffList,
                    IsSuccess = true,
                    StatusCode = 200,
                    Message = "Lấy danh sách nhân viên thành công."
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO
                {
                    IsSuccess = false,
                    StatusCode = 500,
                    Message = $"Lỗi khi lấy danh sách nhân viên: {ex.Message}"
                };
            }
        }
    }
}
