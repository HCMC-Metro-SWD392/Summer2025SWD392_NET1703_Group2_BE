﻿using MetroTicketBE.Domain.Enum;

namespace MetroTicketBE.Domain.DTO.SubscriptionTicket
{
    public class GetSubscriptionTicketDTO
    {
        public Guid Id { get; set; }
        public string TicketName { get; set; } = null!;
        public Guid TicketTypeId { get; set; }
        public int Price { get; set; }
        public int Expiration { get; set; }
    }
}
