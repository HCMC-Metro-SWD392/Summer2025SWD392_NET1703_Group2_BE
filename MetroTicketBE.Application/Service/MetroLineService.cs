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
using AutoMapper;

namespace MetroTicketBE.Application.Service
{
    public class MetroLineService : IMetroLineService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public MetroLineService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
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
                    await _unitOfWork.MetroLineRepository.IsExistByMetroLineNumber(createMetroLineDTO.MetroLineNumber, null);

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
                var metroLines = await _unitOfWork.MetroLineRepository.GetAllListAsync();
                if (metroLines is null || !metroLines.Any())
                {
                    return new ResponseDTO
                    {
                        IsSuccess = false,
                        StatusCode = 404,
                        Message = "Không tìm thấy tuyến Metro nào"
                    };
                }

                var getMetroLines = _mapper.Map<List<GetMetroLineDTO>>(metroLines);
                return new ResponseDTO
                {
                    Message = "Danh sách tuyến Metro",
                    IsSuccess = true,
                    StatusCode = 200,
                    Result = getMetroLines
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
                var metroLine = await _unitOfWork.MetroLineRepository.GetByIdAsync(metroLineId);
                if (metroLine is null)
                {
                    return new ResponseDTO
                    {
                        IsSuccess = false,
                        StatusCode = 404,
                        Message = "Không tìm thấy tuyến Metro"
                    };
                }

                var getMetroLine = _mapper.Map<GetMetroLineDTO>(metroLine);
                return new ResponseDTO
                {
                    IsSuccess = true,
                    StatusCode = 200,
                    Result = getMetroLine
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

        public async Task<ResponseDTO> UpdateMetroLine(Guid metroLineId, UpdateMetroLineDTO updateMetroLineDTO)
        {
            var metroLine = await _unitOfWork.MetroLineRepository.GetByIdAsync(metroLineId);
            if (metroLine is null)
            {
                return new ResponseDTO
                {
                    IsSuccess = false,
                    StatusCode = 404,
                    Message = "Không tìm thấy tuyến Metro"
                };
            }
            try
            {
                var startStation = await _unitOfWork.StationRepository.IsExistById(updateMetroLineDTO.StartStationId ?? metroLine.StartStationId);
                if (startStation is false)
                {
                    return new ResponseDTO
                    {
                        StatusCode = 404,
                        Message = "Không tìm thấy trạm bắt đầu",
                        IsSuccess = false
                    };
                }

                var endStation = await _unitOfWork.StationRepository.IsExistById(updateMetroLineDTO.EndStationId ?? metroLine.EndStationId);
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
                    await _unitOfWork.MetroLineRepository.IsExistByMetroLineNumber(updateMetroLineDTO.MetroLineNumber ?? metroLine.MetroLineNumber, metroLineId);
                if (isExistMetroLine)
                {
                    return new ResponseDTO
                    {
                        IsSuccess = false,
                        StatusCode = 400,
                        Message = "Số tuyến Metro đã tồn tại"
                    };
                }
                PatchMetroLine(metroLine, updateMetroLineDTO);
                _unitOfWork.MetroLineRepository.Update(metroLine);
                await _unitOfWork.SaveAsync();
                return new ResponseDTO()
                {
                    IsSuccess = true,
                    StatusCode = 200,
                    Message = "Cập nhật tuyến Metro thành công",
                    Result = _mapper.Map<GetMetroLineDTO>(metroLine)
                };

            }
            catch (Exception exception)
            {
                return new ResponseDTO()
                {
                    IsSuccess = false,
                    StatusCode = 500,
                    Message = "Lỗi khi cập nhật tuyến Metro: " + exception.Message
                };
            }
        }
        private static void  PatchMetroLine(MetroLine metroLine, UpdateMetroLineDTO updateMetroLineDTO)
        {
            if (!string.IsNullOrEmpty(updateMetroLineDTO.MetroLineNumber))
            {
                metroLine.MetroLineNumber = updateMetroLineDTO.MetroLineNumber;
            }
            if (!string.IsNullOrEmpty(updateMetroLineDTO.MetroName))
            {
                metroLine.MetroName = updateMetroLineDTO.MetroName;
            }
            if (updateMetroLineDTO.StartStationId.HasValue)
            {
                metroLine.StartStationId = updateMetroLineDTO.StartStationId.Value;
            }
            if (updateMetroLineDTO.EndStationId.HasValue)
            {
                metroLine.EndStationId = updateMetroLineDTO.EndStationId.Value;
            }
            if (updateMetroLineDTO.StartTime.HasValue)
            {
                metroLine.StartTime = updateMetroLineDTO.StartTime.Value;
            }

            if (updateMetroLineDTO.EndTime.HasValue)
            {
                metroLine.EndTime = updateMetroLineDTO.EndTime.Value;
            }
            metroLine.CreatedAt = DateTime.UtcNow;
        }
    }
    
}
