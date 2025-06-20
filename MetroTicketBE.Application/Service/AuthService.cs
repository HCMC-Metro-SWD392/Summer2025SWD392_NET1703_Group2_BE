using MetroTicket.Domain.Entities;
using MetroTicketBE.Application.IService;
using MetroTicketBE.Domain.Constants;
using MetroTicketBE.Domain.DTO.Auth;
using MetroTicketBE.Domain.Entities;
using MetroTicketBE.Domain.Enums;
using MetroTicketBE.Infrastructure.IRepository;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace MetroTicketBE.Application.Service
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly IEmailService _emailService;

        public AuthService
        (
            IUnitOfWork unitOfWork,
            RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager,
            ITokenService tokenService,
            IEmailService emailService
        )
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        }

        public async Task<ResponseDTO> LoginUser(LoginDTO loginDTO)
        {
            try
            {
                // Check if phone number exists
                var user = await _unitOfWork.UserManagerRepository.GetByEmailAsync(loginDTO.Email);

                if (user is null)
                {
                    return new ResponseDTO
                    {
                        Message = "Email không tồn tại",
                        Result = null,
                        IsSuccess = false,
                        StatusCode = 404
                    };
                }

                if (user.LockoutEnd > DateTimeOffset.UtcNow)
                {
                    var remainingMinutes = (user.LockoutEnd.Value - DateTimeOffset.UtcNow).TotalMinutes;
                    return new ResponseDTO
                    {
                        Message = $"Tài khoản đang bị tạm khóa {Math.Ceiling(remainingMinutes)} phút",
                        Result = null,
                        IsSuccess = false,
                        StatusCode = 403
                    };
                }

                var isPasswordValid = await _userManager.CheckPasswordAsync(user, loginDTO.Password);

                if (isPasswordValid is false)
                {
                    await _userManager.AccessFailedAsync(user);
                    return new ResponseDTO
                    {
                        Message = $"Mật khẩu không chính xác. Nếu nhập sai {5 - user.AccessFailedCount} lần nữa, tài khoản sẽ bị khóa 5 phút",
                        Result = null,
                        IsSuccess = false,
                        StatusCode = 401
                    };
                }

                if (user.EmailConfirmed is false)
                {
                    return new ResponseDTO
                    {
                        Message = "Email chưa được xác nhận",
                        Result = null,
                        IsSuccess = false,
                        StatusCode = 403
                    };
                }

                var accessToken = await _tokenService.GenerateJwtAccessTokenAsync(user);
                var refreshToken = await _tokenService.GenerateJwtRefreshTokenAsync(user, loginDTO.RememberMe);
                var responeUser = new UserDTO()
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    Address = user.Address,
                    IdentityId = user.IdentityId,
                    Sex = user.Sex,
                    DateOfBirth = user.DateOfBirth,
                    UserName = user.UserName
                };
                await _tokenService.StoreRefreshToken(user.Id, refreshToken, loginDTO.RememberMe);

                await _userManager.ResetAccessFailedCountAsync(user);

                return new ResponseDTO
                {
                    Message = "Đăng nhập thành công",
                    Result = new
                    {
                        AccessToken = accessToken,
                        RefreshToken = refreshToken,
                        User = responeUser
                    },
                    IsSuccess = true,
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO
                {
                    // Return all exception details in the message
                    Message = $"Đã xảy ra lỗi: {ex.Message}",
                    Result = null,
                    IsSuccess = false,
                    StatusCode = 500
                };
            }
        }

        public async Task<ResponseDTO> Logout(ClaimsPrincipal user)
        {
            try
            {
                var userId = user?.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId is null)
                {
                    return new ResponseDTO
                    {
                        Message = "Chưa đăng nhập",
                        IsSuccess = false,
                        StatusCode = 400
                    };
                }

                // Find the user by ID
                var getUser = await _userManager.FindByIdAsync(userId);
                if (getUser is null)
                {
                    return new ResponseDTO
                    {
                        Message = "Người dùng không tồn tại",
                        IsSuccess = false,
                        StatusCode = 404
                    };
                }

                await _userManager.UpdateSecurityStampAsync(getUser);
                // Delete the refresh token associated with the user
                var isDeleted = await _tokenService.DeleteRefreshToken(userId);
                if (isDeleted is false)
                {
                    return new ResponseDTO
                    {
                        Message = "Không thể xóa refresh token",
                        IsSuccess = false,
                        StatusCode = 500
                    };
                }

                return new ResponseDTO
                {
                    Message = "Đăng xuất thành công",
                    IsSuccess = true,
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO
                {
                    // Return all exception details in the message
                    Message = $"Đã xảy ra lỗi: {ex.Message}",
                    IsSuccess = false,
                    StatusCode = 500
                };
            }
        }

        public async Task<ResponseDTO> RegisterCustomer(RegisterCustomerDTO registerCustomerDTO)
        {
            try
            {
                //Check if email already exists
                var isEmailExist = await _unitOfWork.UserManagerRepository.IsEmailExist(registerCustomerDTO.Email);

                if (isEmailExist is true)
                {
                    return new ResponseDTO
                    {
                        Message = "Email đã tồn tại",
                        Result = registerCustomerDTO,
                        IsSuccess = false,
                        StatusCode = 409
                    };
                }

                //Check if phone number already exists
                var isPhoneNumberExist = registerCustomerDTO.PhoneNumber is not null && await _unitOfWork.UserManagerRepository.IsPhoneNumberExist(registerCustomerDTO.PhoneNumber);

                if (isPhoneNumberExist is true)
                {
                    return new ResponseDTO
                    {
                        Message = "Số điện thoại đã tồn tại",
                        Result = registerCustomerDTO,
                        IsSuccess = false,
                        StatusCode = 409
                    };
                }

                //Create new instance of user
                ApplicationUser newUser = new ApplicationUser
                {
                    PhoneNumber = registerCustomerDTO.PhoneNumber,
                    Email = registerCustomerDTO.Email,
                    FullName = registerCustomerDTO.FullName,
                    UserName = registerCustomerDTO.Email
                };

                // Create user in the database
                var createUserResult = await _unitOfWork.UserManagerRepository.CreateAsync(newUser, registerCustomerDTO.Password);

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

                Customer newCustomer = new Customer
                {
                    UserId = newUser.Id,
                    CustomerType = CustomerType.Normal,
                    Points = 0,
                };

                var isRoleExist = await _roleManager.RoleExistsAsync(StaticUserRole.Customer);

                if (isRoleExist is false)
                {
                    await _roleManager.CreateAsync(new IdentityRole(StaticUserRole.Customer));
                }

                // Add user to role
                var addToRoleResult = await _unitOfWork.UserManagerRepository.AddtoRoleAsync(newUser, StaticUserRole.Customer);

                if (addToRoleResult.Succeeded is false)
                {
                    return new ResponseDTO
                    {
                        Message = "Thêm vai trò không thành công",
                        Result = registerCustomerDTO,
                        IsSuccess = false,
                        StatusCode = 400
                    };
                }

                // Add customer to the database
                var addCustomerResult = await _unitOfWork.CustomerRepository.AddAsync(newCustomer);
                if (addCustomerResult is null)
                {
                    return new ResponseDTO
                    {
                        Message = "Thêm khách hàng không thành công",
                        Result = registerCustomerDTO,
                        IsSuccess = false,
                        StatusCode = 400
                    };
                }

                // Save changes to the database
                var saveResult = await _unitOfWork.SaveAsync();
                if (saveResult <= 0)
                {
                    return new ResponseDTO
                    {
                        Message = "Lưu thông tin không thành công",
                        Result = registerCustomerDTO,
                        IsSuccess = false,
                        StatusCode = 500
                    };
                }
                await SendVerifyEmail(newUser.Email);
                return new ResponseDTO
                {
                    Message = "Đăng ký thành công",
                    Result = newCustomer,
                    IsSuccess = true,
                    StatusCode = 201
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO
                {
                    // Return all exception details in the message
                    Message = $"Đã xảy ra lỗi: {ex.Message}",
                    Result = null,
                    IsSuccess = false,
                    StatusCode = 500
                };
            }
        }

        public async Task<ResponseDTO> SendVerifyEmail(string email)
        {
            try
            {
                var user = await _unitOfWork.UserManagerRepository.GetByEmailAsync(email);
                if (user.EmailConfirmed is true)
                {
                    return new ResponseDTO
                    {
                        Message = "Email đã được xác nhận",
                        Result = null,
                        IsSuccess = false,
                        StatusCode = 400
                    };
                }

                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var confirmationLink = $"{StaticURL.Frontend_Url_Verify_Email}?email={email}&token={Uri.EscapeDataString(token)}";

                await _emailService.SendVerifyEmail(email, confirmationLink);
                return new ResponseDTO
                {
                    Message = "Email xác nhận đã được gửi",
                    Result = null,
                    IsSuccess = true,
                    StatusCode = 200
                };
            }
            catch (Exception e)
            {
                return new ResponseDTO
                {
                    // Return all exception details in the message
                    Message = $"Đã xảy ra lỗi khi gửi email xác nhận: {e.Message}",
                    Result = null,
                    IsSuccess = false,
                    StatusCode = 500
                };
            }
        }

        public async Task<ResponseDTO> SetStaffRole(string email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user is null)
                {
                    return new ResponseDTO
                    {
                        Message = "Người dùng không tồn tại",
                        IsSuccess = false,
                        StatusCode = 404
                    };
                }

                var isRoleExist = await _roleManager.RoleExistsAsync(StaticUserRole.Staff);
                if (isRoleExist is false)
                {
                    await _roleManager.CreateAsync(new IdentityRole(StaticUserRole.Staff));
                }
                var addToRoleResult = await _userManager.AddToRoleAsync(user, StaticUserRole.Staff);
                if (addToRoleResult.Succeeded is false)
                {
                    return new ResponseDTO
                    {
                        Message = "Thêm vai trò nhân viên không thành công",
                        IsSuccess = false,
                        StatusCode = 400
                    };
                }

                return new ResponseDTO
                {
                    Message = "Thêm vai trò nhân viên thành công",
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

        public async Task<ResponseDTO> VerifyEmail(string email, string token)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);

                if (user is null)
                {
                    return new ResponseDTO
                    {
                        Message = "Người dùng không tồn tại",
                        Result = null,
                        IsSuccess = false,
                        StatusCode = 404
                    };
                }

                if (user.EmailConfirmed is true)
                {
                    return new ResponseDTO
                    {
                        Message = "Email đã được xác nhận",
                        Result = null,
                        IsSuccess = false,
                        StatusCode = 400
                    };
                }

                var decodedToken = Uri.UnescapeDataString(token);
                var result = await _userManager.ConfirmEmailAsync(user, decodedToken);

                if (result.Succeeded is false)
                {
                    return new ResponseDTO
                    {
                        Message = "Xác nhận email không thành công",
                        Result = null,
                        IsSuccess = false,
                        StatusCode = 400
                    };
                }

                return new ResponseDTO
                {
                    Message = "Xác nhận email thành công",
                    Result = null,
                    IsSuccess = true,
                    StatusCode = 200
                };
            }
            catch (Exception e)
            {
                return new ResponseDTO
                {
                    // Return all exception details in the message
                    Message = $"Đã xảy ra lỗi khi xác nhận email: {e.Message}",
                    Result = null,
                    IsSuccess = false,
                    StatusCode = 500
                };
            }
        }
    }
}
