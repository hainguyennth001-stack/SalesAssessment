using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SalesAssessment.Api.Controllers;
using SalesAssessment.Api.Data;
using SalesAssessment.Api.Models;
using SalesAssessment.Api.Repositories;
using SalesAssessment.Api.Services;
using Xunit;

namespace SalesAssessment.Api.Tests;

public class OrderControllerTests
{
    private static OrdersController CreateController()
    {
        var options = new DbContextOptionsBuilder<SalesAssessmentDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var db = new SalesAssessmentDbContext(options);
        var repository = new OrderRepository(db);
        var service = new OrderService(repository);
        return new OrdersController(service);
    }

    [Fact]
    public async Task CreateOrder_ReturnsCreatedOrder()
    {
        var controller = CreateController();

        var order = new Order { CustomerId = 1, OrderDate = DateTime.UtcNow, TotalAmount = 150m, Status = "Pending" };

        var result = await controller.Create(order);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        var returned = Assert.IsType<Order>(createdResult.Value);
        Assert.Equal("Pending", returned.Status);
    }
}
