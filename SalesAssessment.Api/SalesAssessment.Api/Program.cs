using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SalesAssessment.Api.Data;
using SalesAssessment.Api.Models;
using SalesAssessment.Api.Repositories;
using SalesAssessment.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddDbContext<SalesAssessmentDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var jwtKey = builder.Configuration["Jwt:Key"] ?? "super-secret-key-1234567890-abcdefghijklmnopqrstuvwxyz";
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IOrderService, OrderService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapGet("/", () => Results.Ok(new { message = "SalesAssessment API is running" }));

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<SalesAssessmentDbContext>();
    db.Database.Migrate();

    if (!db.Customers.Any())
    {
        db.Customers.AddRange(
            new Customer { Name = "Alice Nguyen", Email = "alice@example.com", Phone = "0901000001", Address = "Hanoi" },
            new Customer { Name = "Bob Tran", Email = "bob@example.com", Phone = "0901000002", Address = "Da Nang" }
        );
        db.SaveChanges();
    }

    if (!db.Orders.Any())
    {
        var customer = db.Customers.First();
        db.Orders.AddRange(
            new Order { CustomerId = customer.Id, OrderDate = DateTime.UtcNow.AddDays(-5), TotalAmount = 150.50m, Status = "Pending" },
            new Order { CustomerId = customer.Id, OrderDate = DateTime.UtcNow.AddDays(-1), TotalAmount = 320.00m, Status = "Completed" }
        );
        db.SaveChanges();
    }
}

app.Run();
