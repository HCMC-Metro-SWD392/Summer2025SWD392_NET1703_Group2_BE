﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetroTicketBE.Domain.DTO.MetroLine
{
    public class CreateMetroLineDTO
    {
        public string? MetroName { get; set; }
        public Guid StartStationId { get; set; }
        public Guid EndStationId { get; set; }
        
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }
}
