using AutoMapper;
using MetroTicket.Domain.Entities;
using MetroTicketBE.Application.IService;
using MetroTicketBE.Domain.Constants;
using MetroTicketBE.Domain.DTO.Auth;
using MetroTicketBE.Domain.DTO.Customer;
using MetroTicketBE.Domain.Entities;
using MetroTicketBE.Domain.Enum;
using MetroTicketBE.Infrastructure.IRepository;
using MetroTicketBE.Infrastructure.Repository;
using Microsoft.AspNetCore.Identity;

namespace MetroTicketBE.Application.Service;

public class UserService: IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;

    private readonly IMapper _mapper;

    public UserService(IUnitOfWork unitOfWork, RoleManager<IdentityRole> roleManager, IMapper mapper, UserManager<ApplicationUser> userManager)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
    }

    public async Task<ResponseDTO> GetUserByIdAsync(string id)
    {
        try
        {
            if (string.IsNullOrEmpty(id))
            {
                return new ResponseDTO
                {
                    Message = "ID người dùng không hợp lệ",
                    Result = null,
                    IsSuccess = false,
                    StatusCode = 400
                };
            }

            var user = await _unitOfWork.UserManagerRepository.GetByIdAsync(id);

            if (user == null)
            {
                return new ResponseDTO
                {
                    Message = "Người dùng không tồn tại",
                    Result = null,
                    IsSuccess = false,
                    StatusCode = 404
                };
            }

            return new ResponseDTO
            {
                Message = "Lấy thông tin người dùng thành công",
                Result = user,
                IsSuccess = true,
                StatusCode = 200
            };
        }
        catch (Exception ex)
        {
            return new ResponseDTO
            {
                Message = $"Đã xảy ra lỗi: {ex.Message}",
                Result = null,
                IsSuccess = false,
                StatusCode = 500
            };
        }
    }
    public async Task<ResponseDTO> UpdateUserAsync(string userId, UpdateUserDTO updateUserDto)
    {
        try
        {
            if (updateUserDto == null)
            {
                return new ResponseDTO
                {
                    Message = "Thông tin người dùng không hợp lệ",
                    Result = null,
                    IsSuccess = false,
                    StatusCode = 400
                };
            }
            var user = await _unitOfWork.UserManagerRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return new ResponseDTO
                {
                    Message = "Người dùng không tồn tại",
                    Result = null,
                    IsSuccess = false,
                    StatusCode = 404
                };
            }
            PatchWith(user, updateUserDto);
            var result = await _unitOfWork.UserManagerRepository.UpdateAsync(user);

            if (!result.Succeeded)
            {
                return new ResponseDTO
                {
                    Message = "Cập nhật thông tin người dùng thất bại",
                    Result = null,
                    IsSuccess = false,
                    StatusCode = 500
                };
            }

            await _unitOfWork.SaveAsync();
            return new ResponseDTO
            {
                Message = "Cập nhật thông tin người dùng thành công",
                Result = user,
                IsSuccess = true,
                StatusCode = 200
            };
        }
        catch (Exception ex)
        {
            return new ResponseDTO
            {
                Message = $"Đã xảy ra lỗi: {ex.Message}",
                Result = null,
                IsSuccess = false,
                StatusCode = 500
            };
        }
    }

    private static void PatchWith(ApplicationUser user, UpdateUserDTO dto)
    {
        if (dto.FullName != null) user.FullName = dto.FullName;
        if (dto.Address != null) user.Address = dto.Address;
        if (dto.PhoneNumber != null) user.PhoneNumber = dto.PhoneNumber;
        if (dto.Sex != null) user.Sex = dto.Sex;
        if (dto.Email != null) user.Email = dto.Email;
        if (dto.DateOfBirth.HasValue) user.DateOfBirth = dto.DateOfBirth;
        if (dto.IdentityId != null) user.IdentityId = dto.IdentityId;
    }
    
    
}