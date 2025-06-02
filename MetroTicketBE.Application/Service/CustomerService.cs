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
                Result = new CustomerResponseDTO
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
                },
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
}