using SalesAssessment.Api.Models;

namespace SalesAssessment.Api.Repositories;

public interface IOrderRepository
{
    Task<List<Order>> GetAllAsync();
    Task<Order?> GetByIdAsync(int id);
    Task<Order> AddAsync(Order order);
    Task<Order?> UpdateAsync(Order order);
    Task<bool> DeleteAsync(int id);
}
