using MetroTicket.Domain.Entities;
using MetroTicketBE.Application.IService;
using MetroTicketBE.Domain.Constants;
using MetroTicketBE.Domain.DTO.Auth;
using MetroTicketBE.Domain.Entities;
using MetroTicketBE.Domain.Enums;
using MetroTicketBE.Infrastructure.IRepository;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using AutoMapper;
using MetroTicketBE.Domain.DTO.Customer;
using Microsoft.AspNetCore.SignalR;

namespace MetroTicketBE.Application.Service
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly IEmailService _emailService;
        private readonly IMapper _mapper;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly IRedisService _redisService;

        public AuthService
        (
            IUnitOfWork unitOfWork,
            RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager,
            ITokenService tokenService,
            IEmailService emailService,
            IMapper mapper,
            IHubContext<NotificationHub> hubContext,
            IRedisService redisService
        )
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
            _redisService = redisService ?? throw new ArgumentNullException(nameof(redisService));
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
                        Message =
                            $"Mật khẩu không chính xác. Nếu nhập sai {5 - user.AccessFailedCount} lần nữa, tài khoản sẽ bị khóa 5 phút",
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

                var connectionId = await _redisService.RetrieveString($"checkLogin:{user.Id}");
                if(!string.IsNullOrEmpty(connectionId))
                {
                    await _hubContext.Clients.Client(connectionId).SendAsync("ForceLogout");
                }

                var accessToken = await _tokenService.GenerateJwtAccessTokenAsync(user);
                var refreshToken = await _tokenService.GenerateJwtRefreshTokenAsync(user, loginDTO.RememberMe);

                await CheckAndResetStudentExpiration(user.Id);
                var isStudent = await IsStudent(user.Id);
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
                    UserName = user.UserName,
                    IsStudent = isStudent
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

        public async Task<ResponseDTO> LoginUserByGoogle(LoginByGoogleDTO loginByGoogleDTO)
        {
            try
            {
                var isEmailExist = await _unitOfWork.UserManagerRepository.IsEmailExist(loginByGoogleDTO.Email);

                if (!isEmailExist)
                {
                    var registerByGoogleDTO = new RegisterCustomerByGoogleDTO
                    {
                        Email = loginByGoogleDTO.Email,
                        FullName = loginByGoogleDTO.FullName,
                    };

                    await RegisterCustomerByGoogle(registerByGoogleDTO);
                }

                var user = await _unitOfWork.UserManagerRepository.GetByEmailAsync(loginByGoogleDTO.Email);

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

                //var isPasswordValid = await _userManager.CheckPasswordAsync(user, loginDTO.Password);

                //if (isPasswordValid is false)
                //{
                //    await _userManager.AccessFailedAsync(user);
                //    return new ResponseDTO
                //    {
                //        Message =
                //            $"Mật khẩu không chính xác. Nếu nhập sai {5 - user.AccessFailedCount} lần nữa, tài khoản sẽ bị khóa 5 phút",
                //        Result = null,
                //        IsSuccess = false,
                //        StatusCode = 401
                //    };
                //}

                //if (user.EmailConfirmed is false)
                //{
                //    return new ResponseDTO
                //    {
                //        Message = "Email chưa được xác nhận",
                //        Result = null,
                //        IsSuccess = false,
                //        StatusCode = 403
                //    };
                //}
                await CheckAndResetStudentExpiration(user.Id);
                var accessToken = await _tokenService.GenerateJwtAccessTokenAsync(user);
                var refreshToken = await _tokenService.GenerateJwtRefreshTokenAsync(user, loginByGoogleDTO.RememberMe);
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
                    UserName = user.UserName,
                    IsStudent = await IsStudent(user.Id)
                };
                await _tokenService.StoreRefreshToken(user.Id, refreshToken, loginByGoogleDTO.RememberMe);

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
                var isPhoneNumberExist = registerCustomerDTO.PhoneNumber is not null &&
                                         await _unitOfWork.UserManagerRepository.IsPhoneNumberExist(registerCustomerDTO
                                             .PhoneNumber);

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
                var createUserResult =
                    await _unitOfWork.UserManagerRepository.CreateAsync(newUser, registerCustomerDTO.Password);

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
                var addToRoleResult =
                    await _unitOfWork.UserManagerRepository.AddtoRoleAsync(newUser, StaticUserRole.Customer);

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
                    //Result = newCustomer,
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

        public async Task<ResponseDTO> RegisterCustomerByGoogle(RegisterCustomerByGoogleDTO registerCustomerByGoogleDTO)
        {
            try
            {
                //Check if email already exists
                //var isEmailExist = await _unitOfWork.UserManagerRepository.IsEmailExist(registerCustomerByGoogleDTO.Email);

                //if (isEmailExist is true)
                //{
                //    return new ResponseDTO
                //    {
                //        Message = "Email đã tồn tại",
                //        Result = registerCustomerDTO,
                //        IsSuccess = false,
                //        StatusCode = 409
                //    };
                //}

                //Check if phone number already exists
                //var isPhoneNumberExist = registerCustomerByGoogleDTO.PhoneNumber is not null &&
                //                         await _unitOfWork.UserManagerRepository.IsPhoneNumberExist(registerCustomerDTO
                //                             .PhoneNumber);

                //if (isPhoneNumberExist is true)
                //{
                //    return new ResponseDTO
                //    {
                //        Message = "Số điện thoại đã tồn tại",
                //        Result = registerCustomerDTO,
                //        IsSuccess = false,
                //        StatusCode = 409
                //    };
                //}

                //Create new instance of user
                ApplicationUser newUser = new ApplicationUser
                {
                    Email = registerCustomerByGoogleDTO.Email,
                    FullName = registerCustomerByGoogleDTO.FullName,
                    UserName = registerCustomerByGoogleDTO.Email
                };

                // Create user in the database
                var randomPassword = "Mt@" + new Guid(Guid.NewGuid().ToString());

                var createUserResult =
                    await _unitOfWork.UserManagerRepository.CreateAsync(newUser, randomPassword.ToString());

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
                var addToRoleResult =
                    await _unitOfWork.UserManagerRepository.AddtoRoleAsync(newUser, StaticUserRole.Customer);

                if (addToRoleResult.Succeeded is false)
                {
                    return new ResponseDTO
                    {
                        Message = "Thêm vai trò không thành công",
                        Result = registerCustomerByGoogleDTO,
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
                        Result = registerCustomerByGoogleDTO,
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
                        Result = registerCustomerByGoogleDTO,
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
                var confirmationLink =
                    $"{StaticURL.Frontend_Url_Verify_Email}?email={email}&token={Uri.EscapeDataString(token)}";

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

                var isCustomer = await _userManager.IsInRoleAsync(user, StaticUserRole.Customer);
                if (isCustomer)
                {
                    await _userManager.RemoveFromRoleAsync(user, StaticUserRole.Customer);
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
                var existedStaff = await _unitOfWork.StaffRepository.GetByUserIdAsync(user.Id);
                if (existedStaff is not null)
                {
                    existedStaff.IsActive = true;
                    _unitOfWork.StaffRepository.Update(existedStaff);
                }
                else
                {
                    var lastStaff = await _unitOfWork.StaffRepository.GetLastStaffAsync();
                    var staff = new Staff()
                    {
                        UserId = user.Id,
                        StaffCode = lastStaff is null
                            ? "S00001"
                            : GenerateStaffCode(lastStaff.StaffCode),
                    };
                    await _unitOfWork.StaffRepository.AddAsync(staff);
                }
                
                await _unitOfWork.SaveAsync();
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

        public async Task<ResponseDTO> CreateStaffAsync(RegisterCustomerDTO dto)
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
                    UserName = dto.Email,
                    EmailConfirmed = true
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

                var addToRoleResult =
                    await _unitOfWork.UserManagerRepository.AddtoRoleAsync(newUser, StaticUserRole.Staff);
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
                var lastStaff = await _unitOfWork.StaffRepository.GetLastStaffAsync();
                var staff = new Staff()
                {
                    UserId = newUser.Id,
                    StaffCode = lastStaff is null
                        ? "S00001"
                        : GenerateStaffCode(lastStaff.StaffCode),
                };
                await _unitOfWork.StaffRepository.AddAsync(staff);
                await _unitOfWork.SaveAsync();
                return new ResponseDTO()
                {
                    Message = "Đăng ký thành công",
                    //Result = _mapper.Map<UserDTO>(newUser),
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
        private string GenerateStaffCode(string lastStaffCode)
        {
            string prefix = new string(lastStaffCode.TakeWhile(char.IsLetter).ToArray());
            int numberPart = int.Parse(new string(lastStaffCode.SkipWhile(char.IsLetter).ToArray()));
            numberPart++;
            string newNumberPart = numberPart.ToString().PadLeft(5, '0');
            return prefix + newNumberPart;
        }
        public async Task<ResponseDTO> ChangPassword(ClaimsPrincipal user, ChangePasswordDTO changePasswordDTO)
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
                var applicationUser = await _unitOfWork.UserManagerRepository.GetByIdAsync(userId);
                if (applicationUser is null)
                {
                    return new ResponseDTO
                    {
                        Message = "Người dùng không tồn tại",
                        IsSuccess = false,
                        StatusCode = 404
                    };
                }

                var isPasswordValid = await _userManager.CheckPasswordAsync(applicationUser, changePasswordDTO.CurrentPassword);
                if (isPasswordValid is false)
                {
                    return new ResponseDTO
                    {
                        Message = "Mật khẩu hiện tại không chính xác",
                        IsSuccess = false,
                        StatusCode = 401
                    };
                }

                var changePasswordResult = await _userManager.ChangePasswordAsync(applicationUser, changePasswordDTO.CurrentPassword, changePasswordDTO.NewPassword);

                if (changePasswordResult.Succeeded is true)
                {
                    return new ResponseDTO
                    {
                        Message = "Đổi mật khẩu thành công",
                        Result = null,
                        IsSuccess = true,
                        StatusCode = 200
                    };
                }
                else
                {
                    return new ResponseDTO
                    {
                        Message = "Đổi mật khẩu không thành công",
                        Result = changePasswordResult.Errors,
                        IsSuccess = false,
                        StatusCode = 400
                    };
                }
            }
            catch (Exception ex)
            {
                return new ResponseDTO
                {
                    Message = $"Đã xảy ra lỗi khi thay đổi mật khẩu: {ex.Message}",
                    Result = null,
                    IsSuccess = false,
                    StatusCode = 500
                };
            }
        }

        public async Task<ResponseDTO> SendResetPasswordEmail(SendResetPasswordDTO resetPasswordDTO)
        {
            try
            {
                var user = await _unitOfWork.UserManagerRepository.GetByEmailAsync(resetPasswordDTO.Email);
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

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var resetLink = $"{StaticURL.Frontend_Url_Reset_Password}?email={resetPasswordDTO.Email}&token={Uri.EscapeDataString(token)}";

                var key = $"reset-password-email:{resetPasswordDTO.Email}";
                var isAllowToSendEmail = await _emailService.IsAllowToSendEmail(resetPasswordDTO.Email, key);
                if (!isAllowToSendEmail)
                {
                    return new ResponseDTO
                    {
                        Message = "Bạn đã gửi email đặt lại mật khẩu quá nhiều lần. Vui lòng thử lại sau 5 phút nữa.",
                        IsSuccess = false,
                        StatusCode = 429
                    };
                }

                await _emailService.SendResetPasswordEmail(resetPasswordDTO.Email, resetLink);

                return new ResponseDTO
                {
                    Message = "Email đặt lại mật khẩu đã được gửi",
                    IsSuccess = true,
                    StatusCode = 200
                };
            }
            catch (Exception e)
            {
                return new ResponseDTO
                {
                    Message = $"Đã xảy ra lỗi khi gửi email đặt lại mật khẩu: {e.Message}",
                    IsSuccess = false,
                    StatusCode = 500
                };
            }
        }

        public async Task<ResponseDTO> ResetPassword(ResetPasswordDTO resetPasswordDTO)
        {
            try
            {
                var user = await _unitOfWork.UserManagerRepository.GetByEmailAsync(resetPasswordDTO.Email);
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

                var decodedToken = Uri.UnescapeDataString(resetPasswordDTO.Token);
                var resetPasswordResult = await _userManager.ResetPasswordAsync(user, decodedToken, resetPasswordDTO.NewPassword);
                if (resetPasswordResult.Succeeded is true)
                {
                    return new ResponseDTO
                    {
                        Message = "Đặt lại mật khẩu thành công",
                        IsSuccess = true,
                        StatusCode = 200
                    };
                }
                else
                {
                    return new ResponseDTO
                    {
                        Message = "Đặt lại mật khẩu không thành công",
                        Result = resetPasswordResult.Errors,
                        IsSuccess = false,
                        StatusCode = 400
                    };
                }
            }
            catch (Exception ex)
            {
                return new ResponseDTO
                {
                    Message = $"Đã xảy ra lỗi khi đặt lại mật khẩu: {ex.Message}",
                    Result = null,
                    IsSuccess = false,
                    StatusCode = 500
                };
            }
        }
        private async Task CheckAndResetStudentExpiration(string userId)
        {
            try
            {
                var customer = await _unitOfWork.CustomerRepository.GetByUserIdAsync(userId);
                var isStudentExpired = customer?.StudentExpiration != null && customer.StudentExpiration < DateTime.UtcNow;
                if (customer is null || !isStudentExpired) return;
                customer.StudentExpiration = null;
                customer.CustomerType = CustomerType.Normal;
                _unitOfWork.CustomerRepository.Update(customer);
                await _unitOfWork.SaveAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }
        }

        private async Task<bool> IsStudent(string userId)
        {
            var customer = await _unitOfWork.CustomerRepository.GetByUserIdAsync(userId);
            if (customer is null)
            {
                return false;
            }
            return customer.CustomerType == CustomerType.Student &&
                   customer.StudentExpiration != null &&
                   customer.StudentExpiration > DateTime.UtcNow;
        }

        public async Task<ResponseDTO> SetManagerRole(string email)
        {
            try
            {
                var user = await _unitOfWork.UserManagerRepository.GetByEmailAsync(email);
                if (user is null)
                {
                    return new ResponseDTO
                    {
                        Message = "Người dùng không tồn tại",
                        IsSuccess = false,
                        StatusCode = 404
                    };
                }

                var isCustomer = await _userManager.IsInRoleAsync(user, StaticUserRole.Customer);
                if (isCustomer)
                {
                    await _userManager.RemoveFromRoleAsync(user, StaticUserRole.Customer);
                }

                var isStaff = await _userManager.IsInRoleAsync(user, StaticUserRole.Staff);
                if (isCustomer)
                {
                    await _userManager.RemoveFromRoleAsync(user, StaticUserRole.Staff);
                }

                var isRoleExist = await _roleManager.RoleExistsAsync(StaticUserRole.Manager);
                if (isRoleExist is false)
                {
                    await _roleManager.CreateAsync(new IdentityRole(StaticUserRole.Manager));
                }

                await _userManager.AddToRoleAsync(user, StaticUserRole.Manager);

                await _unitOfWork.SaveAsync();

                return new ResponseDTO
                {
                    Message = "Nâng người dùng lên quản lý thành công",
                    IsSuccess = true,
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO
                {
                    Message = "Đã xảy ra lỗi khi nâng vai trò quản lí " + ex.Message,
                    IsSuccess = false,
                    StatusCode = 500
                };
            }
        }

        public async Task<ResponseDTO> SetAdminRole(string email)
        {
            try
            {
                var user = await _unitOfWork.UserManagerRepository.GetByEmailAsync(email);
                if (user is null)
                {
                    return new ResponseDTO
                    {
                        Message = "Người dùng không tồn tại",
                        IsSuccess = false,
                        StatusCode = 404
                    };
                }

                var isCustomer = await _userManager.IsInRoleAsync(user, StaticUserRole.Customer);
                if (isCustomer)
                {
                    await _userManager.RemoveFromRoleAsync(user, StaticUserRole.Customer);
                }

                var isStaff = await _userManager.IsInRoleAsync(user, StaticUserRole.Staff);
                if (isCustomer)
                {
                    await _userManager.RemoveFromRoleAsync(user, StaticUserRole.Staff);
                }

                var isManager = await _userManager.IsInRoleAsync(user, StaticUserRole.Manager);
                if (isManager)
                {
                    await _userManager.RemoveFromRoleAsync(user, StaticUserRole.Manager);
                }

                var isRoleExist = await _roleManager.RoleExistsAsync(StaticUserRole.Admin);
                if (isRoleExist is false)
                {
                    await _roleManager.CreateAsync(new IdentityRole(StaticUserRole.Admin));
                }

                await _userManager.AddToRoleAsync(user, StaticUserRole.Admin);

                await _unitOfWork.SaveAsync();

                return new ResponseDTO
                {
                    Message = "Nâng người dùng lên admin thành công",
                    IsSuccess = true,
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO
                {
                    Message = "Đã xảy ra lỗi khi nâng vai trò admin " + ex.Message,
                    IsSuccess = false,
                    StatusCode = 500
                };
            }
        }

        public async Task<ResponseDTO> CreateManagerAsync(RegisterCustomerDTO dto)
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
                    UserName = dto.Email,
                    EmailConfirmed = true
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

                var addToRoleResult =
                    await _unitOfWork.UserManagerRepository.AddtoRoleAsync(newUser, StaticUserRole.Manager);
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

                await _unitOfWork.SaveAsync();
                return new ResponseDTO()
                {
                    Message = "Đăng ký thành công",
                    //Result = _mapper.Map<UserDTO>(newUser),
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

        public async Task<ResponseDTO> CreateAdminAsync(RegisterCustomerDTO dto)
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
                    UserName = dto.Email,
                    EmailConfirmed = true
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

                var addToRoleResult =
                    await _unitOfWork.UserManagerRepository.AddtoRoleAsync(newUser, StaticUserRole.Admin);
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

                await _unitOfWork.SaveAsync();
                return new ResponseDTO()
                {
                    Message = "Đăng ký thành công",
                    //Result = _mapper.Map<UserDTO>(newUser),
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

        public async Task<ResponseDTO> DemoteRoleToUser(string email)
        {
            try
            {
                var user = await _unitOfWork.UserManagerRepository.GetByEmailAsync(email);
                if (user is null)
                {
                    return new ResponseDTO
                    {
                        Message = "Người dùng không tồn tại",
                        IsSuccess = false,
                        StatusCode = 404
                    };
                }

                var isAdmin = await _userManager.IsInRoleAsync(user, StaticUserRole.Admin);
                if (isAdmin)
                {
                    await _userManager.RemoveFromRoleAsync(user, StaticUserRole.Admin);
                }

                var isManager = await _userManager.IsInRoleAsync(user, StaticUserRole.Manager);
                if (isManager)
                {
                    await _userManager.RemoveFromRoleAsync(user, StaticUserRole.Manager);
                }

                return new ResponseDTO()
                {
                    Message = "Hạ vai trò người dùng thành công",
                    IsSuccess = true,
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO()
                {
                    Message = "Đã xảy ra lỗi khi hạ vai trò người dùng: " + ex.Message,
                    IsSuccess = false,
                    StatusCode = 500
                };
            }
        }

        public async Task<ResponseDTO> GetAllManager(string? filterOn, string? filterQuery, string? sortBy, bool? isAcensding, int pageNumber, int pageSize)
        {
            try
            {
                var managerList = await _unitOfWork.UserManagerRepository.GetAllManagerAsync();

                if (!string.IsNullOrEmpty(filterOn) && !string.IsNullOrEmpty(filterQuery))
                {
                    filterOn = filterOn.ToLower().Trim();
                    filterQuery = filterQuery.ToLower().Trim();

                    managerList = filterOn switch
                    {
                        "email" => managerList.Where(u => u.Email.ToLower().Contains(filterQuery)).ToList(),
                        "fullname" => managerList.Where(u => u.Email.ToLower().Contains(filterQuery)).ToList(),
                        "phonenumber" => managerList.Where(u => u.PhoneNumber != null && u.PhoneNumber.ToLower().Contains(filterQuery)).ToList(),

                        _ => managerList
                    };
                }

                if (!string.IsNullOrEmpty(sortBy))
                {
                    sortBy = sortBy.ToLower().Trim();
                    managerList = sortBy switch
                    {
                        "email" => isAcensding.HasValue && isAcensding.Value ? managerList.OrderBy(u => u.Email).ToList() : managerList.OrderByDescending(u => u.Email).ToList(),
                        "fullname" => isAcensding.HasValue && isAcensding.Value ? managerList.OrderBy(u => u.FullName).ToList() : managerList.OrderByDescending(u => u.FullName).ToList(),
                        "phonenumber" => isAcensding.HasValue && isAcensding.Value ? managerList.OrderBy(u => u.PhoneNumber).ToList() : managerList.OrderByDescending(u => u.PhoneNumber).ToList(),

                        _ => managerList
                    };
                }

                if (pageNumber < 1 || pageSize < 1)
                {
                    return new ResponseDTO
                    {
                        Message = "Số trang hoặc kích thước trang không hợp lệ",
                        Result = null,
                        IsSuccess = false,
                        StatusCode = 400
                    };
                }
                else
                {
                    managerList = managerList.Skip(pageSize * (pageNumber - 1))
                                              .Take(pageSize).ToList();
                }
                if (managerList.Count == 0)
                {
                    return new ResponseDTO
                    {
                        Message = "Không tìm thấy người quản lý nào",
                        IsSuccess = true,
                        StatusCode = 404
                    };
                }

                var getAllManager = _mapper.Map<List<GetAdminMangerDTO>>(managerList);

                return new ResponseDTO
                {
                    Message = "Lấy danh sách người quản lý thành công",
                    Result = getAllManager,
                    IsSuccess = true,
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO
                {
                    Message = $"Đã xảy ra lỗi khi lấy danh sách người quản lý: {ex.Message}",
                    Result = null,
                    IsSuccess = false,
                    StatusCode = 500
                };
            }
        }

        public async Task<ResponseDTO> GetAllAdmin(string? filterOn, string? filterQuery, string? sortBy, bool? isAcensding, int pageNumber, int pageSize)
        {
            try
            {
                var adminList = await _unitOfWork.UserManagerRepository.GetAllAdminAsync();

                if (!string.IsNullOrEmpty(filterOn) && !string.IsNullOrEmpty(filterQuery))
                {
                    filterOn = filterOn.ToLower().Trim();
                    filterQuery = filterQuery.ToLower().Trim();

                    adminList = filterOn switch
                    {
                        "email" => adminList.Where(u => u.Email.ToLower().Contains(filterQuery)).ToList(),
                        "fullname" => adminList.Where(u => u.Email.ToLower().Contains(filterQuery)).ToList(),
                        "phonenumber" => adminList.Where(u => u.PhoneNumber != null && u.PhoneNumber.ToLower().Contains(filterQuery)).ToList(),

                        _ => adminList
                    };
                }

                if (!string.IsNullOrEmpty(sortBy))
                {
                    sortBy = sortBy.ToLower().Trim();
                    adminList = sortBy switch
                    {
                        "email" => isAcensding.HasValue && isAcensding.Value ? adminList.OrderBy(u => u.Email).ToList() : adminList.OrderByDescending(u => u.Email).ToList(),
                        "fullname" => isAcensding.HasValue && isAcensding.Value ? adminList.OrderBy(u => u.FullName).ToList() : adminList.OrderByDescending(u => u.FullName).ToList(),
                        "phonenumber" => isAcensding.HasValue && isAcensding.Value ? adminList.OrderBy(u => u.PhoneNumber).ToList() : adminList.OrderByDescending(u => u.PhoneNumber).ToList(),

                        _ => adminList
                    };
                }

                if (pageNumber < 1 || pageSize < 1)
                {
                    return new ResponseDTO
                    {
                        Message = "Số trang hoặc kích thước trang không hợp lệ",
                        Result = null,
                        IsSuccess = false,
                        StatusCode = 400
                    };
                }
                else
                {
                    adminList = adminList.Skip(pageSize * (pageNumber - 1))
                                              .Take(pageSize).ToList();
                }
                if (adminList.Count == 0)
                {
                    return new ResponseDTO
                    {
                        Message = "Không tìm thấy người admin nào",
                        IsSuccess = true,
                        StatusCode = 404
                    };
                }

                var getAllManager = _mapper.Map<List<GetAdminMangerDTO>>(adminList);

                return new ResponseDTO
                {
                    Message = "Lấy danh sách người admin thành công",
                    Result = getAllManager,
                    IsSuccess = true,
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO
                {
                    Message = $"Đã xảy ra lỗi khi lấy danh sách admin: {ex.Message}",
                    IsSuccess = false,
                    StatusCode = 500
                };
            }
        }

        public async Task<ResponseDTO> DemoteStaff(string email)
        {
            try
            {
                var user = await _unitOfWork.UserManagerRepository.GetByEmailAsync(email);
                if (user is null)
                {
                    return new ResponseDTO
                    {
                        Message = "Người dùng không tồn tại",
                        IsSuccess = false,
                        StatusCode = 404
                    };
                }
                var existStaff = await _unitOfWork.StaffRepository.GetByUserIdAsync(user.Id);
                if (existStaff is null)
                {
                    return new ResponseDTO
                    {
                        Message = "Nhân viên không tồn tại",
                        IsSuccess = false,
                        StatusCode = 404
                    };
                }
                await _userManager.RemoveFromRoleAsync(await _unitOfWork.UserManagerRepository.GetByIdAsync(user.Id),
                        StaticUserRole.Staff);
                await _userManager.AddToRoleAsync(await _unitOfWork.UserManagerRepository.GetByIdAsync(user.Id),
                        StaticUserRole.Customer);
                existStaff.IsActive = false;
                _unitOfWork.StaffRepository.Update(existStaff);
                await _unitOfWork.SaveAsync();
                return new ResponseDTO
                {
                    Message = "Nhân viên đã bị vô hiệu hóa",
                    Result = null,
                    IsSuccess = true,
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO
                {
                    Message = $"Đã xảy ra lỗi khi cập nhật trạng thái nhân viên: {ex.Message}",
                    IsSuccess = false,
                    StatusCode = 500
                };
            }
        }
    }
}

