using Microsoft.EntityFrameworkCore;
using SalesAssessment.Api.Data;
using SalesAssessment.Api.Models;
using SalesAssessment.Api.Repositories;
using Xunit;

namespace SalesAssessment.Api.Tests;

public class CustomerRepositoryTests
{
    private static CustomerRepository CreateRepository()
    {
        var options = new DbContextOptionsBuilder<SalesAssessmentDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var db = new SalesAssessmentDbContext(options);
        return new CustomerRepository(db);
    }

    [Fact]
    public async Task AddAsync_PersistsCustomer()
    {
        var repository = CreateRepository();
        var customer = new Customer { Name = "Linh", Email = "linh@test.com", Phone = "0111", Address = "HCM" };

        var result = await repository.AddAsync(customer);

        Assert.NotNull(result);
        Assert.True(result.Id > 0);
    }
}
