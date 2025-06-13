using AutoMapper;
using MetroTicketBE.Application.IService;
using MetroTicketBE.Domain.DTO.Auth;
using MetroTicketBE.Domain.DTO.TicketRoute;
using MetroTicketBE.Domain.Entities;
using MetroTicketBE.Infrastructure.IRepository;
using MetroTicketBE.WebAPI.Extentions;

namespace MetroTicketBE.Application.Service
{
    public class TicketRouteService : ITicketRouteService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public TicketRouteService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<ResponseDTO> CraeteTicketRoute(CreateTicketRouteDTO createTicketRouteDTO)
        {
            try
            {
                var exists = await _unitOfWork.TicketRouteRepository.GetTicketRouteByStartAndEndStation(createTicketRouteDTO.StartStationId, createTicketRouteDTO.EndStationId);

                if (exists is not null)
                {
                    return new ResponseDTO
                    {
                        Message = "Vé lượt đã tồn tại.",
                        IsSuccess = false,
                        StatusCode = 409
                    };
                }

                double distance = await CalculateDistanceOfTwoStation(createTicketRouteDTO.StartStationId, createTicketRouteDTO.EndStationId);

                var startStation = await _unitOfWork.StationRepository.GetNameById(createTicketRouteDTO.StartStationId);
                if (string.IsNullOrEmpty(startStation))
                {
                    return new ResponseDTO
                    {
                        Message = "Trạm bắt đầu không hợp lệ.",
                        IsSuccess = false,
                        StatusCode = 400
                    };
                }

                var endStation = await _unitOfWork.StationRepository.GetNameById(createTicketRouteDTO.EndStationId);

                if (string.IsNullOrEmpty(endStation))
                {
                    return new ResponseDTO
                    {
                        Message = "Trạm kết thúc không hợp lệ.",
                        IsSuccess = false,
                        StatusCode = 400
                    };
                }

                TicketRoute saveTicketRoute = new TicketRoute
                {
                    TicketName = $"Vé lượt từ {startStation} đến {endStation}",
                    StartStationId = createTicketRouteDTO.StartStationId,
                    EndStationId = createTicketRouteDTO.EndStationId,
                    Distance = distance,
                };

                var getSaveTicketRoute = _mapper.Map<GetTicketRouteDTO>(saveTicketRoute);

                await _unitOfWork.TicketRouteRepository.AddAsync(saveTicketRoute);
                await _unitOfWork.SaveAsync();

                return new ResponseDTO
                {
                    Message = "Tạo vé lượt thành công",
                    IsSuccess = true,
                    StatusCode = 200,
                    Result = getSaveTicketRoute
                };
            }
            catch (Exception ex)
            {
                throw new Exception("Đã xảy ra lỗi khi tạo vé lượt: ", ex);
            }
        }

        public async Task<ResponseDTO> GetTicketRouteByFromTo(Guid StartStation, Guid EndStation)
        {
            try
            {
                var ticketRoute = await _unitOfWork.TicketRouteRepository.GetTicketRouteByStartAndEndStation(StartStation, EndStation);

                if (ticketRoute is not null)
                {
                    int ticketRoutePrice = await _unitOfWork.FareRuleRepository.CalculatePriceFromDistance(ticketRoute.Distance);
                    return new ResponseDTO
                    {
                        Message = "Lấy vé lượt thành công",
                        IsSuccess = true,
                        StatusCode = 200,
                        Result = new
                        {
                            TicketRouteId = ticketRoute.Id,
                            Price = ticketRoutePrice
                        }
                    };
                }

                return new ResponseDTO
                {
                    Message = "Không tìm thấy vé lượt cần tìm.",
                    IsSuccess = false,
                    StatusCode = 404
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO
                {
                    Message = "Đã xảy ra lỗi khi lấy vé lượt: " + ex.Message,
                    IsSuccess = false,
                    StatusCode = 500
                };
            }
        }

        private async Task<double> CalculateDistanceOfTwoStation(Guid startStationId, Guid endStationId)
        {
            try
            {
                var allMetroLines = await _unitOfWork.MetroLineRepository.GetAllListAsync();

                var _graph = new StationGraph(allMetroLines);

                var stationPath = _graph.FindShortestPath(startStationId, endStationId);

                // Log đường đi để kiểm tra
                //var stationNames = stationPath.Select(id =>
                //    allMetroLines.SelectMany(l => l.MetroLineStations)
                //        .FirstOrDefault(s => s.StationId == id)?.Station.Name ?? id.ToString()).ToList();
                //Console.WriteLine("Đường đi: " + string.Join(" -> ", stationNames));

                // Nếu không tìm thấy đường đi, ném ra ngoại lệ
                if (stationPath == null || !stationPath.Any())
                {
                    throw new Exception("Không tìm được đường đi giữa hai trạm.");
                }

                double distance = _graph.GetPathDistance(stationPath);

                // Kiểm tra khoảng cách tính được có hợp lệ hay không
                if (distance <= 0)
                {
                    throw new Exception("Khoảng cách tính được không hợp lệ.");
                }

                return distance;
            }
            catch (Exception ex)
            {
                throw new Exception("Đã xảy ra lỗi tính khoảng cách: ", ex);
            }
        }
    }
}
