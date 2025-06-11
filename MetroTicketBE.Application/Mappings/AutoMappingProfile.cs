using AutoMapper;
using MetroTicketBE.Domain.DTO.TicketRoute;
using MetroTicketBE.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using MetroTicketBE.Domain.DTO.MetroLine;
using MetroTicketBE.Domain.DTO.MetroLineStation;
using MetroTicketBE.Domain.DTO.Station;
using MetroTicketBE.Domain.DTO.Ticket;
using MetroTicketBE.Domain.DTO.Promotion;
using MetroTicketBE.Domain.DTO.SubscriptionTicket;
using MetroTicketBE.Domain.DTO.TrainSchedule;

namespace MetroTicketBE.Application.Mappings
{
    public class AutoMappingProfile : Profile
    {
        public AutoMappingProfile()
        {
            CreateMap<GetTicketRouteDTO, TicketRoute>().ReverseMap();
            CreateMap<MetroLine, GetMetroLineDTO>();
            CreateMap<Station, GetStationDTO>();
            CreateMap<MetroLineStation, GetMetroLineStationDTO>();
            CreateMap<Ticket, GetTicketDTO>();

            CreateMap<Ticket, GetTicketDTO>()
                .ForMember(dest => dest.FromStation, opt => opt.MapFrom(src => src.TicketRoute.StartStation.Name))
                .ForMember(dest => dest.ToStation, opt => opt.MapFrom(src => src.TicketRoute.EndStation.Name));

            CreateMap<Promotion, GetPromotionDTO>();
            CreateMap<TrainSchedule, GetTrainScheduleDTO>()
                .ForMember(dest => dest.MetroLineName, opt => opt.MapFrom(src => src.MetroLine.MetroName))
                .ForMember(dest => dest.StationName, opt => opt.MapFrom(src => src.Station.Name)).ReverseMap();
            CreateMap<SubscriptionTicket, GetSubscriptionTicketDTO>();
        }
    }
}
