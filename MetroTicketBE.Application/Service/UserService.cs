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

    public async Task<ResponseDTO> CreateStaffAsync(RegisterCustomerDTO dto, UserRole role)
    {
        try
        {
            
            var isEmailExist = await _unitOfWork.UserManagerRepository.IsEmailExist(dto.Email);

            if (isEmailExist is true)
            {
                return new ResponseDTO
                {
                    Message = "Email đã tồn tại",
                    Result = dto,
                    IsSuccess = false,
                    StatusCode = 409
                };
            }

            //Check if phone number already exists
            var isPhoneNumberExist = dto.PhoneNumber is not null &&
                                     await _unitOfWork.UserManagerRepository.IsPhoneNumberExist(dto.PhoneNumber);

            if (isPhoneNumberExist is true)
            {
                return new ResponseDTO
                {
                    Message = "Số điện thoại đã tồn tại",
                    Result = dto,
                    IsSuccess = false,
                    StatusCode = 409
                };
            }

            //Create new instance of user
            ApplicationUser newUser = new ApplicationUser
            {
                PhoneNumber = dto.PhoneNumber,
                Email = dto.Email,
                FullName = dto.FullName,
                UserName = dto.Email
            };
            var createUserResult = await _unitOfWork.UserManagerRepository.CreateAsync(newUser, dto.Password);

            if (createUserResult.Succeeded is false)
            {
                return new ResponseDTO
                {
                    Message = "Đăng ký không thành công",
                    Result = createUserResult,
                    IsSuccess = false,
                    StatusCode = 400
                };
            }

            var userRole = StaticUserRole.Staff;
            switch (role)
            {
                case UserRole.Manager:
                    userRole = StaticUserRole.Manager;
                    break;
                case UserRole.Admin:
                    userRole = StaticUserRole.Admin;
                    break;
            }

            var isRoleExist = await _roleManager.RoleExistsAsync(userRole);
            if (isRoleExist is false)
            {
                await _roleManager.CreateAsync(new IdentityRole(userRole));
            }

            var addToRoleResult = await _unitOfWork.UserManagerRepository.AddtoRoleAsync(newUser, userRole);
            if (addToRoleResult.Succeeded is false)
            {
                return new ResponseDTO
                {
                    Message = "Thêm vai trò không thành công",
                    Result = addToRoleResult,
                    IsSuccess = false,
                    StatusCode = 400
                };
            }

            var staff = new Staff()
            {
                UserId = newUser.Id,
            };
            await _unitOfWork.StaffRepository.AddAsync(staff);
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
            await _userManager.ConfirmEmailAsync(newUser, token);
            await _unitOfWork.SaveAsync();
            return new ResponseDTO()
            {
                Message = "Đăng ký thành công",
                Result = _mapper.Map<UserDTO>(newUser),
                IsSuccess = true,
                StatusCode = 201
            };

        }
        catch (Exception ex)
        {
            return new ResponseDTO()
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