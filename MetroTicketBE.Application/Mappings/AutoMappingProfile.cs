using AutoMapper;
using MetroTicketBE.Domain.DTO.TicketRoute;
using MetroTicketBE.Domain.Entities;
using MetroTicket.Domain.Entities;
using MetroTicketBE.Domain.DTO.Auth;
using MetroTicketBE.Domain.DTO.MetroLine;
using MetroTicketBE.Domain.DTO.MetroLineStation;
using MetroTicketBE.Domain.DTO.Station;
using MetroTicketBE.Domain.DTO.Ticket;
using MetroTicketBE.Domain.DTO.Promotion;
using MetroTicketBE.Domain.DTO.StaffSchedule;
using MetroTicketBE.Domain.DTO.SubscriptionTicket;
using MetroTicketBE.Domain.DTO.TrainSchedule;
using MetroTicketBE.Domain.DTO.FormRequest;
using MetroTicketBE.Domain.DTO.Staff;
using MetroTicketBE.Domain.DTO.Customer;
using MetroTicketBE.Domain.DTO.TicketProcess;
using MetroTicketBE.Domain.DTO.DashBoard;
using System.Text.Json;
using MetroTicketBE.Domain.DTO.Log;
using Net.payOS.Types;
using MetroTicketBE.Domain.Enum;
using MetroTicketBE.Domain.DTO.Email;

namespace MetroTicketBE.Application.Mappings
{
    public class AutoMappingProfile : Profile
    {
        public AutoMappingProfile()
        {
            CreateMap<GetTicketRouteDTO, TicketRoute>().ReverseMap();
            CreateMap<MetroLine, GetMetroLineDTO>();
            CreateMap<Station, GetStationDTO>().ForMember(dest => dest.MetroLines, opt => opt.MapFrom(src => src.MetroLineStations.Select(mls => mls.MetroLine)));
            CreateMap<MetroLineStation, GetMetroLineStationDTO>();
            CreateMap<MetroLine, MetroLineDTO>();
            CreateMap<Ticket, GetTicketDTO>()
            .ForMember(dest => dest.FromStationRoute, opt => opt.MapFrom(src => src.TicketRoute.StartStation.Name))
            .ForMember(dest => dest.ToStationRoute, opt => opt.MapFrom(src => src.TicketRoute.EndStation.Name))
            .ForMember(dest => dest.FromStationSub, opt => opt.MapFrom(src => src.SubscriptionTicket.StartStation.Name))
            .ForMember(dest => dest.ToStationSub, opt => opt.MapFrom(src => src.SubscriptionTicket.EndStation.Name)).ReverseMap();

            CreateMap<ApplicationUser, UserDTO>();
            CreateMap<Promotion, GetPromotionDTO>();
            CreateMap<TrainSchedule, GetTrainScheduleDTO>()
                .ForMember(dest => dest.MetroLineName, opt => opt.MapFrom(src => src.MetroLine.MetroName))
                .ForMember(dest => dest.StationName, opt => opt.MapFrom(src => src.Station.Name)).ReverseMap();
            CreateMap<SubscriptionTicket, GetSubscriptionTicketDTO>();
            CreateMap<Station, StationMetroLineDTO>();
            CreateMap<StaffSchedule, GetScheduleDTO>()
                .ForMember(dest => dest.StaffFullName, opt => opt.MapFrom(src => src.Staff.User.FullName))
                .ForMember(dest => dest.ShiftName, opt => opt.MapFrom(src => src.Shift.ShiftName))
                .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.Shift.StartTime))
                .ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => src.Shift.EndTime))
                .ForMember(dest => dest.StationId, opt => opt.MapFrom(src => src.WorkingStationId))
                .ForMember(dest => dest.StationName, opt => opt.MapFrom(src => src.WorkingStation.Name))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));
            CreateMap<GetFormRequestDTO, FormRequest>().ReverseMap();
            CreateMap<Staff, GetStaffDTO>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.User.FullName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.User.PhoneNumber))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.User.Address))
                .ForMember(dest => dest.IdentityId, opt => opt.MapFrom(src => src.User.IdentityId))
                .ForMember(dest => dest.Sex, opt => opt.MapFrom(src => src.User.Sex))
                .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.User.DateOfBirth)).ReverseMap();

            CreateMap<Customer, CustomerResponseDTO>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.User.FullName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.User.PhoneNumber))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.User.Address))
                .ForMember(dest => dest.IdentityId, opt => opt.MapFrom(src => src.User.IdentityId))
                .ForMember(dest => dest.CustomerType, opt => opt.MapFrom(src => src.CustomerType))
                .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.User.DateOfBirth))
                .ForPath(dest => dest.Membership.Id, opt => opt.MapFrom(src => src.Membership.Id))
                .ForPath(dest => dest.Membership.MembershipType, opt => opt.MapFrom(src => src.Membership.MembershipType))
                .ForMember(dest => dest.Points, opt => opt.MapFrom(src => src.Points))
                .ForMember(dest => dest.StudentExpiration, opt => opt.MapFrom(src => src.StudentExpiration))
                .ForMember(dest => dest.Sex, opt => opt.MapFrom(src => src.User.Sex)).ReverseMap();

            CreateMap<TicketProcess, GetTicketProcessDTO>()
                .ForMember(dest => dest.StationName, opt => opt.MapFrom(src => src.Station.Name)).ReverseMap();

            CreateMap<PaymentTransaction, GetTicketStatisticDTO>()
                .ForMember(dest => dest.UserFullName, opt => opt.MapFrom(src => src.Customer.User.FullName))
                .ForMember(dest => dest.DetailTicket, opt => opt.MapFrom(src => ExtractNamesFromDataJson(src.DataJson)))
                .ForMember(dest => dest.PaymentStatus, opt => opt.MapFrom(src => GetPaymentStatusName(src.Status)))
                .ForMember(dest => dest.TimeOfPurchase, opt => opt.MapFrom(src => GetRelativeTime(src.CreatedAt))).ReverseMap();

            CreateMap<Log, GetLogDTO>()
                .ForMember(dest => dest.UserFullname,
                    opt => opt.MapFrom(src => src.User != null ? src.User.FullName : string.Empty))
                .ForMember(dest => dest.LogType,
                    opt => opt.MapFrom(src => src.LogType.ToString()));

            //CreateMap<EmailTemplate, GetEmailTemplateDTO>()
            //    .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.))

        }
        private static List<string> ExtractNamesFromDataJson(string json)
        {
            try
            {
                var items = JsonSerializer.Deserialize<List<ItemData>>(json);
                return items?.Select(i => i.name).ToList() ?? new List<string>();
            }
            catch
            {
                return new List<string>();
            }
        }

        private static string GetRelativeTime(DateTime createdAt)
        {
            var timespan = DateTime.UtcNow - createdAt;

            if (timespan.TotalSeconds < 60)
                return $"{(int)timespan.TotalSeconds} giây trước";
            if (timespan.TotalMinutes < 60)
                return $"{(int)timespan.TotalMinutes} phút trước";
            if (timespan.TotalHours < 24)
                return $"{(int)timespan.TotalHours} giờ trước";
            if (timespan.TotalDays < 30)
                return $"{(int)timespan.TotalDays} ngày trước";
            if (timespan.TotalDays < 365)
                return $"{(int)(timespan.TotalDays / 30)} tháng trước";

            return $"{(int)(timespan.TotalDays / 365)} năm trước";
        }

        private static string GetPaymentStatusName(PaymentStatus status)
        {
            return status switch
            {
                PaymentStatus.Unpaid => "Chưa thanh toán",
                PaymentStatus.Paid => "Đã thanh toán",
                PaymentStatus.Canceled => "Đã hủy",
                _ => "Không xác định"
            };
        }

    }
}
