using Microsoft.EntityFrameworkCore;
using SalesAssessment.Api.Data;
using SalesAssessment.Api.Models;
using SalesAssessment.Api.Repositories;
using Xunit;

namespace SalesAssessment.Api.Tests;

public class OrderRepositoryTests
{
    private static OrderRepository CreateRepository()
    {
        var options = new DbContextOptionsBuilder<SalesAssessmentDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var db = new SalesAssessmentDbContext(options);
        return new OrderRepository(db);
    }

    [Fact]
    public async Task AddAsync_PersistsOrder()
    {
        var repository = CreateRepository();
        var order = new Order { CustomerId = 1, OrderDate = DateTime.UtcNow, TotalAmount = 200m, Status = "Completed" };

        var result = await repository.AddAsync(order);

        Assert.NotNull(result);
        Assert.True(result.Id > 0);
    }
}
