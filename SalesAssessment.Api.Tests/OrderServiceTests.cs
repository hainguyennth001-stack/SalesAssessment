using Microsoft.EntityFrameworkCore;
using SalesAssessment.Api.Data;
using SalesAssessment.Api.Models;
using SalesAssessment.Api.Repositories;
using SalesAssessment.Api.Services;
using Xunit;

namespace SalesAssessment.Api.Tests;

public class OrderServiceTests
{
    private static OrderService CreateService()
    {
        var options = new DbContextOptionsBuilder<SalesAssessmentDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var db = new SalesAssessmentDbContext(options);
        var repository = new OrderRepository(db);
        return new OrderService(repository);
    }

    [Fact]
    public async Task CreateAsync_AddsOrder()
    {
        var service = CreateService();
        var order = new Order { CustomerId = 1, OrderDate = DateTime.UtcNow, TotalAmount = 99.99m, Status = "Pending" };

        var result = await service.CreateAsync(order);

        Assert.NotNull(result);
        Assert.Equal("Pending", result.Status);
    }
}
