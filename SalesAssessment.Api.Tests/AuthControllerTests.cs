using Microsoft.AspNetCore.Mvc;
using SalesAssessment.Api.Controllers;
using SalesAssessment.Api.Models;
using Xunit;

namespace SalesAssessment.Api.Tests;

public class AuthControllerTests
{
    [Fact]
    public async Task Login_WithValidCredentials_ReturnsToken()
    {
        var controller = new AuthController();

        var result = await controller.Login(new LoginRequest { Username = "admin", Password = "password123" });

        var okResult = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<LoginResponse>(okResult.Value);
        Assert.False(string.IsNullOrWhiteSpace(value.Token));
    }
}
