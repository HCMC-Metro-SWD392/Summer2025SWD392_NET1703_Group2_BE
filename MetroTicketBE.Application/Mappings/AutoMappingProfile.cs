using AutoMapper;
using MetroTicketBE.Domain.DTO.TicketRoute;
using MetroTicketBE.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetroTicketBE.Domain.DTO.MetroLine;
using MetroTicketBE.Domain.DTO.MetroLineStation;
using MetroTicketBE.Domain.DTO.Station;

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
        }
    }
}
