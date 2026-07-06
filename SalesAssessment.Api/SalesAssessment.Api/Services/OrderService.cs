using SalesAssessment.Api.Models;
using SalesAssessment.Api.Repositories;

namespace SalesAssessment.Api.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;

    public OrderService(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public Task<List<Order>> GetAllAsync() => _orderRepository.GetAllAsync();

    public Task<Order?> GetByIdAsync(int id) => _orderRepository.GetByIdAsync(id);

    public Task<Order> CreateAsync(Order order) => _orderRepository.AddAsync(order);

    public async Task<Order?> UpdateAsync(int id, Order order)
    {
        if (id != order.Id) return null;
        return await _orderRepository.UpdateAsync(order);
    }

    public Task<bool> DeleteAsync(int id) => _orderRepository.DeleteAsync(id);
}
