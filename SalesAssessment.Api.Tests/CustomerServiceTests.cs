using Microsoft.EntityFrameworkCore;
using SalesAssessment.Api.Data;
using SalesAssessment.Api.Models;
using SalesAssessment.Api.Repositories;
using SalesAssessment.Api.Services;
using Xunit;

namespace SalesAssessment.Api.Tests;

public class CustomerServiceTests
{
    private static CustomerService CreateService()
    {
        var options = new DbContextOptionsBuilder<SalesAssessmentDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var db = new SalesAssessmentDbContext(options);
        var repository = new CustomerRepository(db);
        return new CustomerService(repository);
    }

    [Fact]
    public async Task CreateAsync_AddsCustomer()
    {
        var service = CreateService();
        var customer = new Customer { Name = "Ana", Email = "ana@test.com", Phone = "0123", Address = "Hue" };

        var result = await service.CreateAsync(customer);

        Assert.NotNull(result);
        Assert.Equal("Ana", result.Name);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenMissing()
    {
        var service = CreateService();

        var result = await service.GetByIdAsync(999);

        Assert.Null(result);
    }
}
