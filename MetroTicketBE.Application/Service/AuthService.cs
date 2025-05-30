using MetroTicket.Domain.Entities;
using MetroTicketBE.Application.IService;
using MetroTicketBE.Domain.Constants;
using MetroTicketBE.Domain.DTO.Auth;
using MetroTicketBE.Domain.Entities;
using MetroTicketBE.Domain.Enums;
using MetroTicketBE.Infrastructure.IRepository;
using Microsoft.AspNetCore.Identity;

namespace MetroTicketBE.Application.Service
{
    public class AuthService : IAuthService
    {
        private readonly IUserManagerRepository _userManagerRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenService _tokenService;

        public AuthService
        (
            IUserManagerRepository userManagerRepository,
            IUnitOfWork unitOfWork,
            RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager,
            ITokenService tokenService
        )
        {
            _userManagerRepository = userManagerRepository ?? throw new ArgumentNullException(nameof(userManagerRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        }

        public async Task<ResponseDTO> LoginUser(LoginDTO loginDTO)
        {
            try
            {
                // Check if phone number exists
                var user = await _userManagerRepository.FindByEmailAsync(loginDTO.Email);

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

                await _tokenService.StoreRefreshToken(user.Id, refreshToken, loginDTO.RememberMe);

                await _userManager.ResetAccessFailedCountAsync(user);

                return new ResponseDTO
                {
                    Message = "Đăng nhập thành công",
                    Result = new
                    {
                        AccessToken = accessToken,
                        RefreshToken = refreshToken,
                        User = user
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

        public async Task<ResponseDTO> RegisterCustomer(RegisterCustomerDTO registerCustomerDTO)
        {
            try
            {
                //Check if email already exists
                var isEmailExist = await _userManagerRepository.IsEmailExist(registerCustomerDTO.Email);
                
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
                var isPhoneNumberExist = registerCustomerDTO.PhoneNumber is not null && await _userManagerRepository.IsPhoneNumberExist(registerCustomerDTO.PhoneNumber);
                
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
                    UserName = registerCustomerDTO.PhoneNumber
                };

                // Create user in the database
                var createUserResult = await _userManagerRepository.CreateAsync(newUser, registerCustomerDTO.Password);

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
                var addToRoleResult = await _userManagerRepository.AddtoRoleAsync(newUser, StaticUserRole.Customer);

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

        public Task<ResponseDTO> SendVerifyEmail(string email, string confirmationLink)
        {
            throw new NotImplementedException();
        }
    }
}
