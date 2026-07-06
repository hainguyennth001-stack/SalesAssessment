using Microsoft.EntityFrameworkCore;
using SalesAssessment.Api.Data;
using SalesAssessment.Api.Models;

namespace SalesAssessment.Api.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly SalesAssessmentDbContext _db;

    public CustomerRepository(SalesAssessmentDbContext db)
    {
        _db = db;
    }

    public async Task<List<Customer>> GetAllAsync()
    {
        return await _db.Customers.Include(c => c.Orders).ToListAsync();
    }

    public async Task<Customer?> GetByIdAsync(int id)
    {
        return await _db.Customers.Include(c => c.Orders).FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Customer> AddAsync(Customer customer)
    {
        _db.Customers.Add(customer);
        await _db.SaveChangesAsync();
        return customer;
    }

    public async Task<Customer?> UpdateAsync(Customer customer)
    {
        var existing = await _db.Customers.FindAsync(customer.Id);
        if (existing == null) return null;

        existing.Name = customer.Name;
        existing.Email = customer.Email;
        existing.Phone = customer.Phone;
        existing.Address = customer.Address;

        await _db.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var existing = await _db.Customers.FindAsync(id);
        if (existing == null) return false;

        _db.Customers.Remove(existing);
        await _db.SaveChangesAsync();
        return true;
    }
}
