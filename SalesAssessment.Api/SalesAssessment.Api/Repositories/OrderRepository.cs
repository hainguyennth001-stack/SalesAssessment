using Microsoft.EntityFrameworkCore;
using SalesAssessment.Api.Data;
using SalesAssessment.Api.Models;

namespace SalesAssessment.Api.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly SalesAssessmentDbContext _db;

    public OrderRepository(SalesAssessmentDbContext db)
    {
        _db = db;
    }

    public async Task<List<Order>> GetAllAsync()
    {
        return await _db.Orders.Include(o => o.Customer).ToListAsync();
    }

    public async Task<Order?> GetByIdAsync(int id)
    {
        return await _db.Orders.Include(o => o.Customer).FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<Order> AddAsync(Order order)
    {
        _db.Orders.Add(order);
        await _db.SaveChangesAsync();
        return order;
    }

    public async Task<Order?> UpdateAsync(Order order)
    {
        var existing = await _db.Orders.FindAsync(order.Id);
        if (existing == null) return null;

        existing.OrderDate = order.OrderDate;
        existing.TotalAmount = order.TotalAmount;
        existing.Status = order.Status;
        existing.CustomerId = order.CustomerId;

        await _db.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var existing = await _db.Orders.FindAsync(id);
        if (existing == null) return false;

        _db.Orders.Remove(existing);
        await _db.SaveChangesAsync();
        return true;
    }
}
