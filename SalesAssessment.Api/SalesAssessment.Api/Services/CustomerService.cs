using SalesAssessment.Api.Models;
using SalesAssessment.Api.Repositories;

namespace SalesAssessment.Api.Services;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepository;

    public CustomerService(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public Task<List<Customer>> GetAllAsync() => _customerRepository.GetAllAsync();

    public Task<Customer?> GetByIdAsync(int id) => _customerRepository.GetByIdAsync(id);

    public Task<Customer> CreateAsync(Customer customer) => _customerRepository.AddAsync(customer);

    public async Task<Customer?> UpdateAsync(int id, Customer customer)
    {
        if (id != customer.Id) return null;
        return await _customerRepository.UpdateAsync(customer);
    }

    public Task<bool> DeleteAsync(int id) => _customerRepository.DeleteAsync(id);
}
