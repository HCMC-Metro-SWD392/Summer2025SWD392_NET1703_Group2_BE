﻿using MetroTicketBE.Domain.DTO.Auth;
using MetroTicketBE.Domain.DTO.FareRule;
using System.Security.Claims;

namespace MetroTicketBE.Application.IService
{
    public interface IFareRuleService
    {
        Task<ResponseDTO> GetAll(ClaimsPrincipal user, string? sortBy, bool? isAcsending, int pageNumber, int pageSize);
        Task<ResponseDTO> CreateFareRule(CreateFareRuleDTO createFareRuleDTO);
        Task<ResponseDTO> UpdateFareRule(UpdateFareRuleDTO updareFareRuleDTO);
    }
}
