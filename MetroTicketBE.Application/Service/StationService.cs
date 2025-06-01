using MetroTicketBE.Application.IService;
using MetroTicketBE.Domain.DTO.Auth;
using MetroTicketBE.Domain.DTO.Station;
using MetroTicketBE.Domain.Entities;
using MetroTicketBE.Infrastructure.IRepository;

namespace MetroTicketBE.Application.Service
{
    public class StationService : IStationService
    {
        private readonly IUnitOfWork _unitOfWork;
        public StationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<ResponseDTO> CreateStation(CreateStationDTO createStationDTO)
        {
            try
            {
                var isExistStation = await _unitOfWork.StationRepository.IsExistByName(createStationDTO.Name);

                if (isExistStation is true)
                {
                    return new ResponseDTO
                    {
                        IsSuccess = false,
                        StatusCode = 400,
                        Message = "Trạm Metro đã tồn tại"
                    };
                }

                Station station = new Station
                {
                    Name = createStationDTO.Name,
                    Address = createStationDTO.Address,
                    Description = createStationDTO.Description
                };

                await _unitOfWork.StationRepository.AddAsync(station);
                await _unitOfWork.SaveAsync();

                return new ResponseDTO
                {
                    IsSuccess = true,
                    StatusCode = 200,
                    Message = "Tạo trạm Metro thành công",
                    Result = station
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO
                {
                    IsSuccess = false,
                    StatusCode = 500,
                    Message = "Lỗi khi tạo trạm Metro: " + ex.Message
                };
            }
        }
    }
}
