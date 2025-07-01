using AutoMapper;
using MetroTicketBE.Domain.DTO.TicketRoute;
using MetroTicketBE.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
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
using MetroTicketBE.Domain.Enums;
using MetroTicketBE.Domain.DTO.TicketProcess;

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
            .ForMember(dest => dest.FromStation, opt => opt.MapFrom(src =>
                src.TicketRoute != null ? src.TicketRoute.StartStation.Name :
                src.SubscriptionTicket != null ? src.SubscriptionTicket.StartStation.Name : null))
            .ForMember(dest => dest.ToStation, opt => opt.MapFrom(src =>
                src.TicketRoute != null ? src.TicketRoute.EndStation.Name :
                src.SubscriptionTicket != null ? src.SubscriptionTicket.EndStation.Name : null)).ReverseMap();

            CreateMap<ApplicationUser, UserDTO>();
            CreateMap<Promotion, GetPromotionDTO>();
            CreateMap<TrainSchedule, GetTrainScheduleDTO>()
                .ForMember(dest => dest.MetroLineName, opt => opt.MapFrom(src => src.MetroLine.MetroName))
                .ForMember(dest => dest.StationName, opt => opt.MapFrom(src => src.Station.Name)).ReverseMap();
            CreateMap<SubscriptionTicket, GetSubscriptionTicketDTO>();
            CreateMap<StaffShift, ShiftInfoDto>();
            CreateMap<Station, StationInfoDto>();
            CreateMap<Station, StationMetroLineDTO>();
            CreateMap<Staff, StaffInfoDto>()
                .ForMember(dest => dest.FullName,
                    opt => opt.MapFrom(src => src.User.FullName));
            CreateMap<StaffSchedule, GetScheduleDTO>();
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

            CreateMap<TicketProcess, GetTicketProcessDTO>().ReverseMap();
        }
    }
}
