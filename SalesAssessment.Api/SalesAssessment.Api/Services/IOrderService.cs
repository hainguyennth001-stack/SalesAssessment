using SalesAssessment.Api.Models;

namespace SalesAssessment.Api.Services;

public interface IOrderService
{
    Task<List<Order>> GetAllAsync();
    Task<Order?> GetByIdAsync(int id);
    Task<Order> CreateAsync(Order order);
    Task<Order?> UpdateAsync(int id, Order order);
    Task<bool> DeleteAsync(int id);
}
