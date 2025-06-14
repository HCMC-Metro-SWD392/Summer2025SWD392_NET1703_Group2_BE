using MetroTicketBE.Application.IService;
using MetroTicketBE.Domain.DTO.Auth;
using MetroTicketBE.Domain.DTO.Customer;
using MetroTicketBE.Domain.Entities;
using MetroTicketBE.Infrastructure.IRepository;

namespace MetroTicketBE.Application.Service;

public class CustomerService: ICustomerService
{
    private readonly IUnitOfWork _unitOfWork;

    public CustomerService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
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
    
    public async Task<ResponseDTO> UpdateCustomerAsync(Guid customerId, UpdateCustomerDTO updateCustomerDTO)
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

            // customer.User.FullName = updateCustomerDTO.FullName;
            // customer.User.Address = updateCustomerDTO.Address;
            // customer.User.PhoneNumber = updateCustomerDTO.PhoneNumber;
            // customer.User.Email = updateCustomerDTO.Email;
            // customer.User.DateOfBirth = updateCustomerDTO.DateOfBirth;
            // customer.User.IdentityId = updateCustomerDTO.IdentityId;
            // customer.CustomerType = updateCustomerDTO.CustomerType ?? customer.CustomerType;
            
            PatchWith(customer, updateCustomerDTO);
            
            _unitOfWork.CustomerRepository.Update(customer);
            await _unitOfWork.SaveAsync();

            return new ResponseDTO()
            {
                Message = "Cập nhật thông tin khách hàng thành công",
                Result = MapToCustomerResponseDTO(customer),
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

    private static void PatchWith(Customer customer, UpdateCustomerDTO dto)
    {
        if (dto.FullName != null) customer.User.FullName = dto.FullName;
        if (dto.Address != null) customer.User.Address = dto.Address;
        if (dto.PhoneNumber != null) customer.User.PhoneNumber = dto.PhoneNumber;
        if (dto.Sex != null) customer.User.Sex = dto.Sex;
        if (dto.Email != null) customer.User.Email = dto.Email;
        if (dto.DateOfBirth.HasValue) customer.User.DateOfBirth = DateTime.SpecifyKind(dto.DateOfBirth.Value, DateTimeKind.Local);
        if (dto.IdentityId != null) customer.User.IdentityId = dto.IdentityId;
        if (dto.CustomerType.HasValue) customer.CustomerType = dto.CustomerType.Value;
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
}