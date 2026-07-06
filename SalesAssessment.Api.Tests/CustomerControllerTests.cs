using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SalesAssessment.Api.Controllers;
using SalesAssessment.Api.Data;
using SalesAssessment.Api.Models;
using SalesAssessment.Api.Repositories;
using SalesAssessment.Api.Services;
using Xunit;

namespace SalesAssessment.Api.Tests;

public class CustomerControllerTests
{
    private static (SalesAssessmentDbContext Db, CustomersController Controller) CreateController()
    {
        var options = new DbContextOptionsBuilder<SalesAssessmentDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var db = new SalesAssessmentDbContext(options);
        var repository = new CustomerRepository(db);
        var service = new CustomerService(repository);
        var controller = new CustomersController(service);

        return (db, controller);
    }

    [Fact]
    public async Task CreateCustomer_ReturnsCreatedCustomer()
    {
        var (_, controller) = CreateController();

        var customer = new Customer { Name = "Test", Email = "test@example.com", Phone = "0123456789", Address = "Hanoi" };

        var result = await controller.Create(customer);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        var returned = Assert.IsType<Customer>(createdResult.Value);
        Assert.Equal("Test", returned.Name);
    }

    [Fact]
    public async Task GetCustomer_ReturnsNotFound_WhenMissing()
    {
        var (_, controller) = CreateController();

        var result = await controller.Get(999);

        Assert.IsType<NotFoundResult>(result);
    }
}
