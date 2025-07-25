﻿using MetroTicketBE.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace MetroTicket.Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
        public string? Address { get; set; }
        public string? Sex { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public string? IdentityId { get; set; }

        public ICollection<Log> Logs { get; set; } = new List<Log>();
        public ICollection<FormRequest> FormRequestsAsSenders { get; set; } = new List<FormRequest>();
        public ICollection<FormRequest> FormRequestsAsReviewers { get; set; } = new List<FormRequest>();
    }
}