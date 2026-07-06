# Đề bài và hướng dẫn thực hiện bài đánh giá năng lực

## 1. Mục tiêu bài tập
Xây dựng một ứng dụng quản lý bán hàng đơn giản với:
- Backend: `.NET 10`
- Frontend: `React`
- Database: `SQL Server`
- Docker: chạy môi trường SQL Server và backend/FE nếu có thể
- Testing: backend đạt ít nhất `60% coverage`
- Quy trình: dùng `Git` và `Gitflow`

## 2. Yêu cầu chức năng

### 2.1 Entities chính
Tạo 2 entity chính:
- `Customer`: ít nhất 5 trường
  - `Id` (int, PK)
  - `Name` (string)
  - `Email` (string)
  - `Phone` (string)
  - `Address` (string)
- `Order`: ít nhất 5 trường
  - `Id` (int, PK)
  - `OrderDate` (DateTime)
  - `CustomerId` (int, FK)
  - `TotalAmount` (decimal)
  - `Status` (string)
  - nếu muốn có thêm `Description` hoặc `OrderNumber` thì tốt hơn

### 2.2 Quan hệ database
- `Customer` 1 - n `Order`
- `Order.CustomerId` là khóa ngoại liên kết tới `Customer.Id`
- Dùng ràng buộc dữ liệu cơ bản: không null khi cần, độ dài chuỗi phù hợp

### 2.3 CRUD
- `Customer`: Create, Read, Update, Delete
- `Order`: Create, Read, Update, Delete
- Trên FE phải có UI hiển thị danh sách, thêm/sửa/xóa, và kết nối quan hệ
- Ví dụ: trang chi tiết Customer hiển thị các Order của khách đó

### 2.4 Docker
- Tạo `docker-compose.yml` để chạy SQL Server container
- Backend có thể chạy trong Docker hoặc chạy local kết nối tới SQL Server container
- Nếu muốn, FE cũng có thể chạy bằng Docker nhưng không bắt buộc

### 2.5 Test
- Tạo project unit test cho backend với `xUnit`
- Đạt tối thiểu 60% code coverage cho backend
- Kiểm tra service / repository / controller

### 2.6 Gitflow
- Dùng nhánh `main`, `develop`, `feature/*`
- Commit rõ ràng
- Ít nhất mô tả được Gitflow trong quá trình thực hiện

---

## 3. Kết cấu đề xuất thư mục

```text
c:\DEHA_SOFT\JMU\bsnipa01-sale-be\SalesAssessment\
  ├── SalesAssessment.sln
  ├── SalesAssessment.Api\
  ├── SalesAssessment.Api.Tests\
  └── salesapp-frontend\
```

> Mình khuyên bạn tạo folder `SalesAssessment` trong workspace này để chứa dự án đánh giá năng lực.

---

## 4. Hướng dẫn chi tiết tạo project

### 4.1 Bước 1: Chuẩn bị môi trường

1. Cài .NET 10 SDK.
2. Cài Node.js LTS (phiên bản 18/20 đều được).
3. Cài Docker Desktop.
4. Cài Git nếu chưa có.

### 4.2 Bước 2: Tạo folder chứa dự án

Trong File Explorer, tạo folder `c:\DEHA_SOFT\JMU\bsnipa01-sale-be\SalesAssessment` nếu chưa có.

### 4.3 Bước 3: Tạo solution và backend Web API bằng Visual Studio 2026

1. Mở Visual Studio 2026.
2. Chọn `Create a new project`.
3. Chọn mẫu `ASP.NET Core Web API`.
4. Đặt `Project name` là `SalesAssessment.Api`.
5. Đặt `Location` là `c:\DEHA_SOFT\JMU\bsnipa01-sale-be\SalesAssessment`.
6. Chọn `Framework` là `.NET 10.0` (net10.0).
7. Nhấn `Create`.

Visual Studio sẽ tạo solution `SalesAssessment.sln` và project `SalesAssessment.Api` trong folder đó.

### 4.4 Bước 4: Thêm Entity Framework Core và SQL Server vào project

1. Trong Solution Explorer, nhấp phải vào project `SalesAssessment.Api`.
2. Chọn `Manage NuGet Packages...`.
3. Vào tab `Browse` và tìm `Microsoft.EntityFrameworkCore.SqlServer`, sau đó cài đặt.
4. Tìm tiếp `Microsoft.EntityFrameworkCore.Tools` và cài đặt.
Swashbuckle.AspNetCore

### 4.5 Bước 5: Tạo models và DbContext

Tạo các file sau trong `SalesAssessment.Api`:
- `Models/Customer.cs`
- `Models/Order.cs`
- `Data/SalesAssessmentDbContext.cs`

Nội dung mẫu:

`Customer.cs`
```csharp
public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}
```

`Order.cs`
```csharp
public class Order
{
    public int Id { get; set; }
    public DateTime OrderDate { get; set; }
    public int CustomerId { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public Customer? Customer { get; set; }
}
```

`SalesAssessmentDbContext.cs`
```csharp
using Microsoft.EntityFrameworkCore;

public class SalesAssessmentDbContext : DbContext
{
    public SalesAssessmentDbContext(DbContextOptions<SalesAssessmentDbContext> options)
        : base(options)
    {
    }

    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Order> Orders => Set<Order>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Phone).HasMaxLength(50);
            entity.Property(e => e.Address).HasMaxLength(300);
            entity.HasMany(e => e.Orders)
                  .WithOne(o => o.Customer)
                  .HasForeignKey(o => o.CustomerId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Status).IsRequired().HasMaxLength(100);
            entity.Property(e => e.OrderDate).IsRequired();
        });
    }
}
```

### 4.6 Bước 6: Cấu hình connection string và DI

Mở `SalesAssessment.Api/Program.cs` và sửa:

```csharp
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<SalesAssessmentDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
```

Thêm `appsettings.json` trong `SalesAssessment.Api` nếu chưa có:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=SalesAssessmentDb;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

> Lưu ý: nếu chạy SQL Server container thì `Server=localhost,1433` sẽ đúng khi bạn map port 1433 trên host.

---

## 5. Tạo migration và cập nhật database

1. Mở Visual Studio 2026.
2. Vào menu `Tools` > `NuGet Package Manager` > `Package Manager Console`.
3. Trong `Package Manager Console`, chọn project `SalesAssessment.Api` làm `Default project`.
4. Gõ lệnh `Add-Migration InitialCreate` và nhấn Enter.
5. Khi migration được tạo, gõ `Update-Database` và nhấn Enter.

Nếu dùng Docker container cho SQL Server, hãy chắc chắn container đã chạy trước khi thực hiện `Update-Database`.

---

## 6. Tạo controller CRUD

Tạo 2 controller:
- `Controllers/CustomersController.cs`
- `Controllers/OrdersController.cs`

Mẫu `CustomersController.cs`:

```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly SalesAssessmentDbContext _db;

    public CustomersController(SalesAssessmentDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var customers = await _db.Customers.Include(c => c.Orders).ToListAsync();
        return Ok(customers);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var customer = await _db.Customers.Include(c => c.Orders).FirstOrDefaultAsync(c => c.Id == id);
        if (customer == null) return NotFound();
        return Ok(customer);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Customer customer)
    {
        _db.Customers.Add(customer);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = customer.Id }, customer);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Customer customer)
    {
        if (id != customer.Id) return BadRequest();
        var existing = await _db.Customers.FindAsync(id);
        if (existing == null) return NotFound();

        existing.Name = customer.Name;
        existing.Email = customer.Email;
        existing.Phone = customer.Phone;
        existing.Address = customer.Address;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var existing = await _db.Customers.FindAsync(id);
        if (existing == null) return NotFound();
        _db.Customers.Remove(existing);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
```

Mẫu `OrdersController.cs`:

```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly SalesAssessmentDbContext _db;

    public OrdersController(SalesAssessmentDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var orders = await _db.Orders.Include(o => o.Customer).ToListAsync();
        return Ok(orders);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var order = await _db.Orders.Include(o => o.Customer).FirstOrDefaultAsync(o => o.Id == id);
        if (order == null) return NotFound();
        return Ok(order);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Order order)
    {
        _db.Orders.Add(order);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = order.Id }, order);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Order order)
    {
        if (id != order.Id) return BadRequest();
        var existing = await _db.Orders.FindAsync(id);
        if (existing == null) return NotFound();

        existing.OrderDate = order.OrderDate;
        existing.TotalAmount = order.TotalAmount;
        existing.Status = order.Status;
        existing.CustomerId = order.CustomerId;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var existing = await _db.Orders.FindAsync(id);
        if (existing == null) return NotFound();
        _db.Orders.Remove(existing);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
```

---

## 7. Chạy backend kiểm tra

1. Mở solution `SalesAssessment.sln` trong Visual Studio 2026.
2. Chọn project `SalesAssessment.Api` làm startup project.
3. Chọn `Debug` > `Start Debugging` (F5) hoặc `Start Without Debugging` (Ctrl+F5).

Kiểm tra API bằng trình duyệt hoặc Postman:
- `https://localhost:5001/api/customers`
- `https://localhost:5001/api/orders`

Nếu dùng Swagger, truy cập: `https://localhost:5001/swagger`

### 7.1 Thêm JWT authentication (tuỳ chọn nhưng rất tốt)
Nếu muốn bảo vệ API bằng JWT, có thể thêm các bước sau:

1. Cài package `Microsoft.AspNetCore.Authentication.JwtBearer`.
2. Trong `appsettings.json`, thêm section:

```json
"Jwt": {
  "Key": "super-secret-key-1234567890-abcdefghijklmnopqrstuvwxyz"
}
```

3. Trong `Program.cs`, cấu hình authentication:

```csharp
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

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
```

4. Trong controller cần bảo vệ, thêm attribute `[Authorize]`.
5. Tạo endpoint login để trả JWT token, ví dụ:

```csharp
[HttpPost("login")]
public IActionResult Login(LoginRequest request)
{
    if (request.Username == "admin" && request.Password == "password123")
    {
        // tạo token ở đây
        return Ok(new { token = "jwt_token_here" });
    }

    return Unauthorized();
}
```

6. Khi gọi API cần auth, gửi header:

```http
Authorization: Bearer <token>
```

> Với mục đích test/local, có thể giữ key như trên. Với production, nên dùng key ngẫu nhiên dài hơn và không hardcode trực tiếp trong source.

---

## 8. Tạo frontend React

### 8.1 Tạo project React

Bạn có thể tạo frontend bằng Visual Studio 2026 hoặc bằng một công cụ khác.

- Nếu muốn tạo trong Visual Studio: chọn `Create a new project`, tìm `ASP.NET Core with React.js` hoặc tạo riêng project React trong Visual Studio Code.
- Nếu muốn dùng Vite: mở terminal trong thư mục `SalesAssessment` và chạy `npm create vite@latest salesapp-frontend -- --template react-ts`.

Nếu dùng Vite, sau khi tạo vào thư mục `salesapp-frontend` và chạy `npm install`.

### 8.2 Cấu trúc frontend
Tạo các component/pages:
- `src/pages/CustomerList.tsx`
- `src/pages/CustomerForm.tsx`
- `src/pages/OrderList.tsx`
- `src/pages/OrderForm.tsx`
- `src/api/api.ts`

### 8.3 Kết nối API
Trong `src/api/api.ts`:

```ts
const API_URL = import.meta.env.VITE_API_URL ?? "https://localhost:5001";

export async function getCustomers() {
  return fetch(`${API_URL}/api/customers`).then(res => res.json());
}

export async function getOrders() {
  return fetch(`${API_URL}/api/orders`).then(res => res.json());
}

export async function createCustomer(customer) {
  return fetch(`${API_URL}/api/customers`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(customer),
  });
}
```

> Với Create/Update/Delete cho cả customers và orders, tạo các hàm tương tự.

### 8.4 Chạy frontend
Từ `c:\DEHA_SOFT\JMU\bsnipa01-sale-be\SalesAssessment\salesapp-frontend`:

```powershell
npm install
npm run dev
```

Nếu dùng template React bình thường:

```powershell
npm install
npm start
```

### 8.5 Cấu hình proxy nếu cần
Nếu FE chạy local trên port 5173 và backend trên 5001, bạn có thể cấu hình proxy trong Vite hoặc `package.json`:

Vite: `vite.config.ts`
```ts
export default defineConfig({
  server: {
    proxy: {
      '/api': 'https://localhost:5001',
    },
  },
});
```

---

## 9. Cấu hình Docker

### 9.1 Tạo `docker-compose.yml`
Tạo file `docker-compose.yml` trong `SalesAssessment` với nội dung:

```yaml
version: '3.9'
services:
  sqlserver:
    image: mcr.microsoft.com/mssql/server:2022-latest
    container_name: salesassessment-sqlserver
    environment:
      SA_PASSWORD: "YourStrong!Passw0rd"
      ACCEPT_EULA: "Y"
    ports:
      - "1433:1433"
    volumes:
      - sqlserverdata:/var/opt/mssql

volumes:
  sqlserverdata:
```

### 9.2 Chạy container SQL Server
Từ `c:\DEHA_SOFT\JMU\bsnipa01-sale-be\SalesAssessment` chạy:

```powershell
docker compose up -d
```

Kiểm tra container:

```powershell
docker ps
```

### 9.3 Kết nối backend với SQL Server container
Trong `SalesAssessment.Api/appsettings.json`, dùng connection string:

```json
"DefaultConnection": "Server=localhost,1433;Database=SalesAssessmentDb;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True;"
```

Sau đó vào Visual Studio 2026 và mở `Package Manager Console` (Tools > NuGet Package Manager > Package Manager Console). Chọn project `SalesAssessment.Api` làm `Default project`, rồi gõ `Update-Database` và nhấn Enter.

---

## 10. Tạo project test backend

### 10.1 Tạo project xUnit bằng Visual Studio

1. Trong Visual Studio 2026, nhấp phải vào solution `SalesAssessment` trong Solution Explorer.
2. Chọn `Add` > `New Project`.
3. Chọn mẫu `xUnit Test Project`.
4. Đặt `Project name` là `SalesAssessment.Api.Tests`.
5. Đặt `Location` là `c:\DEHA_SOFT\JMU\bsnipa01-sale-be\SalesAssessment`.
6. Nhấn `Create`.
7. Trong Solution Explorer, nhấp phải vào project `SalesAssessment.Api.Tests` và chọn `Add` > `Project Reference...`, chọn `SalesAssessment.Api` và nhấn `OK`.
8. Mở `Manage NuGet Packages...` trên project `SalesAssessment.Api.Tests`.
9. Tìm và cài `coverlet.collector`.

### 10.2 Viết test mẫu
Tạo file `SalesAssessment.Api.Tests/CustomerControllerTests.cs`:

```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class CustomerControllerTests
{
    private SalesAssessmentDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<SalesAssessmentDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new SalesAssessmentDbContext(options);
    }

    [Fact]
    public async Task CreateCustomer_ReturnsCreatedCustomer()
    {
        var db = CreateDbContext();
        var controller = new CustomersController(db);

        var customer = new Customer { Name = "Test", Email = "test@example.com", Phone = "0123456789", Address = "Hanoi" };

        var result = await controller.Create(customer);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        var returned = Assert.IsType<Customer>(createdResult.Value);
        Assert.Equal("Test", returned.Name);
    }
}
```

> Viết thêm test cho `OrdersController`, validation, update, delete, và các interaction Customer-Order.

### 10.3 Chạy test và đo coverage
1. Mở Visual Studio 2026.
2. Chọn menu `Test` > `Test Explorer`.
3. Nhấp `Run All` để chạy toàn bộ test trong project `SalesAssessment.Api.Tests`.
4. Để đo coverage, mở `Test` > `Configure Code Coverage` hoặc dùng tiện ích đóng gói nếu Visual Studio đã cài `coverlet.collector`.

Nếu muốn, bạn vẫn có thể cấu hình code coverage bằng công cụ Visual Studio hoặc bằng `Package Manager Console`, nhưng phần backend chính vẫn nên kiểm tra bằng Test Explorer.

### 10.4 Đảm bảo coverage >= 60%
- Kiểm tra báo cáo code coverage trong Visual Studio Test Explorer hoặc báo cáo do Coverlet tạo.
- Nếu chưa đạt, bổ sung test cho các đường dẫn quan trọng: CRUD, logic cập nhật, xử lý lỗi

---

## 11. Git và Gitflow

### 11.1 Khởi tạo Git repo
Từ `SalesAssessment` chạy:

```powershell
git init
git checkout -b develop
```

### 11.2 Luồng Gitflow cơ bản
- `main`: chứa phiên bản release ổn định
- `develop`: chứa mã phát triển
- `feature/*`: tạo branch cho từng tính năng

### 11.3 Ví dụ lệnh Git

```powershell
git checkout -b feature/customer-order-crud
# làm việc, commit
git add .
git commit -m "feat: add customer and order models with API"

git checkout develop
git merge feature/customer-order-crud

# khi hoàn thành và kiểm tra xong
git checkout main
git merge develop
```

---

## 12. Kiểm tra cuối cùng

### 12.1 Backend
- Mở `SalesAssessment.sln` trong Visual Studio 2026.
- Chọn project `SalesAssessment.Api` làm startup project.
- Nhấn `F5` hoặc `Ctrl+F5` để chạy.
- Duyệt `https://localhost:5001/swagger`
- Gọi được các endpoint CRUD

### 12.2 Frontend
- Nếu dùng Vite: mở terminal trong thư mục `salesapp-frontend`, chạy `npm install` và `npm run dev`.
- Nếu dùng template React mặc định: chạy `npm install` và `npm start`.
- Truy cập UI và kiểm tra:
  - Tạo/sửa/xóa Customer
  - Tạo/sửa/xóa Order
  - Xem Order theo Customer

### 12.3 Docker
- Mở Docker Desktop và bật container SQL Server từ `docker-compose.yml`.
- Hoặc dùng terminal trong thư mục `SalesAssessment` chạy `docker compose up -d`.
- Kiểm tra container đang chạy qua Docker Desktop UI.
- Vào Visual Studio, mở `Package Manager Console`, chọn `SalesAssessment.Api` làm `Default project`, và chạy `Update-Database`.

### 12.4 Test
- Chạy test trong Visual Studio Test Explorer.
- Kiểm tra code coverage trong Visual Studio hoặc báo cáo Coverlet.
- Báo cáo coverage >= 60%

---

## 13. Gợi ý thêm
- Nếu muốn mở rộng, thêm entity `Product` và `OrderItem`
- Có thể tách repository/service layer để code backend rõ ràng hơn
- Dùng AutoMapper nếu bạn muốn tạo DTO riêng cho API
- Dùng `FluentValidation` để validate form trên backend

---

## 14. Tổng kết

Bài tập này đánh giá năng lực của bạn ở các mặt:
- Thiết kế database đúng quan hệ
- Viết API .NET 10
- Xây dựng frontend React kết nối API
- Sử dụng Docker và SQL Server
- Viết test và đạt coverage
- Dùng GitFlow quản lý code

Chúc bạn hoàn thành tốt! Nếu cần, tôi có thể giúp bạn tạo chi tiết từng file code trong backend và frontend.
