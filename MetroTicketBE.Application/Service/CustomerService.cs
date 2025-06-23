using AutoMapper;
using MetroTicket.Domain.Entities;
using MetroTicketBE.Application.IService;
using MetroTicketBE.Domain.DTO.Auth;
using MetroTicketBE.Domain.DTO.Customer;
using MetroTicketBE.Domain.Entities;
using MetroTicketBE.Infrastructure.IRepository;

namespace MetroTicketBE.Application.Service;

public class CustomerService : ICustomerService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CustomerService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<ResponseDTO> GetCustomerByIdAsync(Guid customerId)
    {
        try
        {
            if (customerId == Guid.Empty)
            {
                return new ResponseDTO()
                {
                    Message = "Mã khách hàng không hợp lệ",
                    Result = null,
                    IsSuccess = false,
                    StatusCode = 400
                };
            }

            Customer? customer = await _unitOfWork.CustomerRepository.GetByIdAsync(customerId);

            if (customer is null)
            {
                return new ResponseDTO()
                {
                    Message = "Khách hàng không tồn tại",
                    Result = null,
                    IsSuccess = false,
                    StatusCode = 404
                };
            }

            return new ResponseDTO()
            {
                Message = "Lấy thông tin khách hàng thành công",
                Result = MapToCustomerResponseDTO(customer),
                IsSuccess = true,
                StatusCode = 200
            };
        }
        catch (Exception exception)
        {
            return new ResponseDTO()
            {
                Message = $"Đã xảy ra lỗi: {exception.Message}",
                Result = null,
                IsSuccess = false,
                StatusCode = 500
            };
        }
    }

    public Task<ResponseDTO> GetCustomerByUserIdAsync(string userId)
    {
        try
        {
            if (string.IsNullOrEmpty(userId))
            {
                return Task.FromResult(new ResponseDTO()
                {
                    Message = "Mã người dùng không hợp lệ",
                    Result = null,
                    IsSuccess = false,
                    StatusCode = 400
                });
            }

            return _unitOfWork.CustomerRepository.GetByUserIdAsync(userId)
                .ContinueWith(task =>
                {
                    Customer? customer = task.Result;

                    if (customer is null)
                    {
                        return new ResponseDTO()
                        {
                            Message = "Khách hàng không tồn tại",
                            Result = null,
                            IsSuccess = false,
                            StatusCode = 404
                        };
                    }

                    return new ResponseDTO()
                    {
                        Message = "Lấy thông tin khách hàng thành công",
                        Result = MapToCustomerResponseDTO(customer),
                        IsSuccess = true,
                        StatusCode = 200
                    };
                });
        }
        catch (Exception exception)
        {
            return Task.FromResult(new ResponseDTO()
            {
                Message = $"Đã xảy ra lỗi: {exception.Message}",
                Result = null,
                IsSuccess = false,
                StatusCode = 500
            });
        }
    }

    public Task<ResponseDTO> GetCustomerByEmailAsync(string email)
    {
        try
        {
            if (string.IsNullOrEmpty(email))
            {
                return Task.FromResult(new ResponseDTO()
                {
                    Message = "Email không hợp lệ",
                    Result = null,
                    IsSuccess = false,
                    StatusCode = 400
                });
            }

            return _unitOfWork.CustomerRepository.GetByEmailAsync(email)
                .ContinueWith(task =>
                {
                    Customer? customer = task.Result;

                    if (customer is null)
                    {
                        return new ResponseDTO()
                        {
                            Message = "Khách hàng không tồn tại",
                            Result = null,
                            IsSuccess = false,
                            StatusCode = 404
                        };
                    }

                    return new ResponseDTO()
                    {
                        Message = "Lấy thông tin khách hàng thành công",
                        Result = MapToCustomerResponseDTO(customer),
                        IsSuccess = true,
                        StatusCode = 200
                    };
                });
        }
        catch (Exception exception)
        {
            return Task.FromResult(new ResponseDTO()
            {
                Message = $"Đã xảy ra lỗi: {exception.Message}",
                Result = null,
                IsSuccess = false,
                StatusCode = 500
            });
        }
    }

    private CustomerResponseDTO MapToCustomerResponseDTO(Customer customer)
    {
        return new CustomerResponseDTO
        {
            Id = customer.Id,
            CustomerType = customer.CustomerType,
            FullName = customer.User.FullName,
            Address = customer.User.Address,
            Sex = customer.User.Sex,
            PhoneNumber = customer.User.PhoneNumber,
            Email = customer.User.Email,
            DateOfBirth = customer.User.DateOfBirth,
            UserName = customer.User.UserName,
            IdentityId = customer.User.IdentityId,
            Membership = new MembershipDTO()
            {
                Id = customer.Membership?.Id,
                MembershipType = customer.Membership?.MembershipType
            },
            Points = customer.Points,
            StudentExpiration = customer.StudentExpiration
        };
    }

    public async Task<ResponseDTO> GetAllCustomersAsync(string? filterOn, string? filterQuery, string? sortBy, bool? isAscending, int pageNumber, int pageSize)
    {
        try
        {
            var customers = await _unitOfWork.CustomerRepository.GetAllAsync(includeProperties: "User");

            if (!string.IsNullOrEmpty(filterOn) && !string.IsNullOrEmpty(filterQuery))
            {
                string filter = filterOn.Trim().ToLower();
                string query = filterQuery.Trim().ToLower();

                customers = filter switch
                {
                    "fullname" => customers.Where(c => c.User.FullName.ToLower().Contains(query, StringComparison.CurrentCultureIgnoreCase)),
                    "email" => customers.Where(c => c.User.Email.ToLower().Contains(query, StringComparison.CurrentCultureIgnoreCase)),
                    "phonenumber" => customers.Where(c => c.User.PhoneNumber.ToLower().Contains(query, StringComparison.CurrentCultureIgnoreCase)),

                    _ => customers
                };
            }

            if (!string.IsNullOrEmpty(sortBy))
            {
                customers = sortBy.Trim().ToLower() switch
                {
                    "fullname" => isAscending == true ? customers.OrderBy(c => c.User.FullName) : customers.OrderByDescending(c => c.User.FullName),
                    "email" => isAscending == true ? customers.OrderBy(c => c.User.Email) : customers.OrderByDescending(c => c.User.Email),
                    "phonenumber" => isAscending == true ? customers.OrderBy(c => c.User.PhoneNumber) : customers.OrderByDescending(c => c.User.PhoneNumber),

                    _ => customers
                };
            }

            if (pageNumber <= 0 || pageSize <= 0)
            {
                return new ResponseDTO()
                {
                    Message = "Số trang hoặc kích thước trang không hợp lệ",
                    Result = null,
                    IsSuccess = false,
                    StatusCode = 400
                };
            }
            else
            {
                customers = customers
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize).ToList();
            }

            var getCustomer = _mapper.Map<List<CustomerResponseDTO>>(customers);
            return new ResponseDTO()
            {
                Message = "Lấy danh sách khách hàng thành công",
                Result = getCustomer,
                IsSuccess = true,
                StatusCode = 200
            };
        }
        catch (Exception exception)
        {
            return new ResponseDTO()
            {
                Message = $"Đã xảy ra lỗi khi lấy danh sách khách hàng: {exception.Message}",
                Result = null,
                IsSuccess = false,
                StatusCode = 500
            };
        }
    }
}