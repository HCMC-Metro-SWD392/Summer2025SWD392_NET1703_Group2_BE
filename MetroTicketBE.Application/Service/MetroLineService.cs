using MetroTicketBE.Application.IService;
using MetroTicketBE.Domain.DTO.Auth;
using MetroTicketBE.Domain.DTO.MetroLine;
using MetroTicketBE.Domain.Entities;
using MetroTicketBE.Infrastructure.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetroTicketBE.Application.Service
{
    public class MetroLineService : IMetroLineService
    {
        private readonly IUnitOfWork _unitOfWork;

        public MetroLineService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<ResponseDTO> CreateMetroLine(CreateMetroLineDTO createMetroLineDTO)
        {
            try
            {
                var startStation = await _unitOfWork.StationRepository.IsExistById(createMetroLineDTO.StartStationId);
                if (startStation is false)
                {
                    return new ResponseDTO
                    {
                        StatusCode = 404,
                        Message = "Không tìm thấy trạm bắt đầu",
                        IsSuccess = false
                    };
                }

                var endStation = await _unitOfWork.StationRepository.IsExistById(createMetroLineDTO.EndStationId);
                if (endStation is false)
                {
                    return new ResponseDTO
                    {
                        StatusCode = 404,
                        Message = "Không tìm thấy trạm kết thúc ",
                        IsSuccess = false
                    };
                }

                var isExistMetroLine =
                    await _unitOfWork.MetroLineRepository.IsExistByMetroLineNumber(createMetroLineDTO.MetroLineNumber);

                if (isExistMetroLine is true)
                {
                    return new ResponseDTO
                    {
                        IsSuccess = false,
                        StatusCode = 400,
                        Message = "Số tuyến Metro đã tồn tại"
                    };
                }

                MetroLine metroLine = new MetroLine
                {
                    MetroLineNumber = createMetroLineDTO.MetroLineNumber,
                    MetroName = createMetroLineDTO.MetroName,
                    StartStationId = createMetroLineDTO.StartStationId,
                    EndStationId = createMetroLineDTO.EndStationId
                };

                await _unitOfWork.MetroLineRepository.AddAsync(metroLine);
                await _unitOfWork.SaveAsync();

                return new ResponseDTO
                {
                    IsSuccess = true,
                    StatusCode = 200,
                    Message = "Tạo tuyến Metro thành công",
                    Result = metroLine
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO
                {
                    IsSuccess = false,
                    StatusCode = 500,
                    Message = "Lỗi khi tạo tuyến Metro: " + ex.InnerException?.Message ?? ex.Message
                };
            }
        }

        public async Task<ResponseDTO> GetAllMetroLines()
        {
            try
            {
                var metroLines = await _unitOfWork.MetroLineRepository.GetAllAsync();
                if (metroLines is null || !metroLines.Any())
                {
                    return new ResponseDTO
                    {
                        IsSuccess = false,
                        StatusCode = 404,
                        Message = "Không tìm thấy tuyến Metro nào"
                    };
                }

                return new ResponseDTO
                {
                    IsSuccess = true,
                    StatusCode = 200,
                    Result = metroLines
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO
                {
                    IsSuccess = false,
                    StatusCode = 500,
                    Message = "Lỗi khi lấy danh sách tuyến Metro: " + ex.InnerException?.Message ?? ex.Message
                };
            }
        }
        
        public async Task<ResponseDTO> GetMetroLineById(Guid metroLineId)
        {
            try
            {
                var metroLine = await _unitOfWork.MetroLineRepository.GetAsync(ml => ml.Id == metroLineId);
                if (metroLine is null)
                {
                    return new ResponseDTO
                    {
                        IsSuccess = false,
                        StatusCode = 404,
                        Message = "Không tìm thấy tuyến Metro"
                    };
                }

                return new ResponseDTO
                {
                    IsSuccess = true,
                    StatusCode = 200,
                    Result = metroLine
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO
                {
                    IsSuccess = false,
                    StatusCode = 500,
                    Message = "Lỗi khi lấy thông tin tuyến Metro: " + ex.InnerException?.Message ?? ex.Message
                };
            }
        }
    }
    
    
}
