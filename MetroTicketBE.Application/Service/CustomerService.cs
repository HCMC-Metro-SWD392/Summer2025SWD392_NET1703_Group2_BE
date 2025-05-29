using MetroTicketBE.Application.IService;
using MetroTicketBE.Domain.DTO.Auth;
using MetroTicketBE.Domain.DTO.Customer;
using MetroTicketBE.Infrastructure.IRepository;

namespace MetroTicketBE.Application.Service;

public class CustomerService: ICustomerService
{
    private readonly ICustomerRepository _customerRepository;

    public CustomerService(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository)); 
    }
    
    public async Task<CustomerResponseDTO?> GetCustomerByIdAsync(Guid customerId)
    {
       var customer = await _customerRepository.GetByIdAsync(customerId);
       
         if (customer == null)
         {
             return null;
         }

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