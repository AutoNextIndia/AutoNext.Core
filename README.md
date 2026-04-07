# 📚 **AutoNext.Core - Complete README.md Content**

Here's a comprehensive README.md for your AutoNext.Core library:

```markdown
# 🚀 AutoNext.Core - Enterprise Microservices Core Library

[![NuGet Version](https://img.shields.io/nuget/v/AutoNext.Core.svg)](https://www.nuget.org/packages/AutoNext.Core/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/AutoNext.Core.svg)](https://www.nuget.org/packages/AutoNext.Core/)
[![License](https://img.shields.io/github/license/autonext/core.svg)](https://github.com/autonext/core/blob/main/LICENSE)
[![.NET](https://img.shields.io/badge/.NET-9.0-blue.svg)](https://dotnet.microsoft.com/)
[![Build Status](https://img.shields.io/github/actions/workflow/status/autonext/core/publish.yml)](https://github.com/autonext/core/actions)

**Enterprise-grade foundation for .NET microservices** with PostgreSQL, RabbitMQ, Redis, CQRS, Repository Pattern, and comprehensive cross-cutting concerns.

## 📦 Table of Contents

- [Features](#-features)
- [Quick Start](#-quick-start)
- [Installation](#-installation)
- [Core Components](#-core-components)
- [Usage Examples](#-usage-examples)
- [Configuration](#-configuration)
- [API Reference](#-api-reference)
- [Best Practices](#-best-practices)
- [Contributing](#-contributing)
- [License](#-license)

## ✨ Features

### 🗄️ **Database & Persistence**
- ✅ PostgreSQL with Entity Framework Core 9.0
- ✅ Dapper support for high-performance queries
- ✅ Generic Repository Pattern (IRepository, IReadOnlyRepository)
- ✅ Unit of Work pattern for transactions
- ✅ Specification Pattern for complex queries
- ✅ Automatic audit fields (CreatedAt, CreatedBy, UpdatedAt, UpdatedBy)
- ✅ Soft delete support
- ✅ Built-in health checks

### 📨 **Message Bus**
- ✅ RabbitMQ integration
- ✅ Event-driven architecture support
- ✅ Delayed message publishing
- ✅ Request-Response pattern
- ✅ Automatic retry with Polly
- ✅ Dead letter exchange support
- ✅ Correlation ID propagation

### 💾 **Caching**
- ✅ In-memory cache (for development)
- ✅ Redis distributed cache (for production)
- ✅ Hybrid cache strategy
- ✅ Cache invalidation patterns
- ✅ Cache-aside pattern
- ✅ Automatic cache health checks

### 📝 **Logging & Monitoring**
- ✅ Serilog integration
- ✅ Structured logging
- ✅ Enriched with CorrelationId, MachineName, ThreadId
- ✅ Console, File, and JSON output formats
- ✅ Request/Response logging
- ✅ Performance monitoring
- ✅ Health check endpoints (/health, /ready, /live)

### 🛡️ **Cross-Cutting Concerns**
- ✅ Global exception handling
- ✅ Correlation ID middleware
- ✅ Request logging middleware
- ✅ Performance monitoring middleware
- ✅ Tenant resolution middleware
- ✅ Complete exception hierarchy
- ✅ Validation with FluentValidation

### 🔒 **Security & Identity**
- ✅ JWT Bearer authentication ready
- ✅ Role-based authorization
- ✅ Permission-based authorization
- ✅ Current user context service
- ✅ Password hashing utilities

## 🚀 Quick Start

### Step 1: Install the Package

```bash
dotnet add package AutoNext.Core
```

### Step 2: Configure Your Application

```csharp
// Program.cs
using AutoNext.Core.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add AutoNext Core services
builder.Services.AddAutoNextCore(builder.Configuration);

// Add PostgreSQL with Entity Framework
builder.Services.AddPostgreSqlDbContext<AppDbContext>(builder.Configuration);

// Add repositories (Unit of Work + Generic repositories)
builder.Services.AddRepositories();

// Add RabbitMQ message bus
builder.Services.AddRabbitMqMessageBus(builder.Configuration);

// Add Redis cache (optional, falls back to in-memory)
builder.Services.AddRedisCache(builder.Configuration);

// Configure Serilog
builder.Host.UseAutoNextSerilog();

var app = builder.Build();

// Use AutoNext middlewares
app.UseAutoNextMiddleware();

// Map health checks
app.MapHealthChecks("/health");

app.Run();
```

### Step 3: Create Your DbContext

```csharp
using AutoNext.Core.Abstractions;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    
    public DbSet<Order> Orders { get; set; }
    public DbSet<Product> Products { get; set; }
}
```

### Step 4: Create Your First Entity

```csharp
using AutoNext.Core.Base;

public class Order : BaseEntity
{
    public string OrderNumber { get; set; }
    public decimal TotalAmount { get; set; }
    public string CustomerId { get; set; }
    public OrderStatus Status { get; set; }
}
```

### Step 5: Create a Service with Repository

```csharp
using AutoNext.Core.Abstractions;

public class OrderService
{
    private readonly IRepository<Order, Guid> _orderRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMessageBus _messageBus;
    
    public OrderService(
        IRepository<Order, Guid> orderRepository,
        IUnitOfWork unitOfWork,
        IMessageBus messageBus)
    {
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
        _messageBus = messageBus;
    }
    
    public async Task<Order> CreateOrderAsync(CreateOrderRequest request)
    {
        var order = new Order
        {
            OrderNumber = GenerateOrderNumber(),
            TotalAmount = request.Amount,
            CustomerId = request.CustomerId,
            Status = OrderStatus.Pending
        };
        
        await _orderRepository.AddAsync(order);
        await _unitOfWork.SaveChangesAsync();
        
        // Publish event
        await _messageBus.PublishAsync(new OrderCreatedEvent(order.Id));
        
        return order;
    }
}
```

## 📦 Installation

### Via .NET CLI

```bash
dotnet add package AutoNext.Core
```

### Via Package Manager Console

```powershell
Install-Package AutoNext.Core
```

### Via Visual Studio NuGet Package Manager

1. Right-click on your project
2. Select "Manage NuGet Packages"
3. Search for "AutoNext.Core"
4. Click "Install"

## 🏗️ Core Components

### 1. **Repository Pattern**

```csharp
// Write repository (with change tracking)
public interface IRepository<T, TId>
{
    Task<T?> GetByIdAsync(TId id, CancellationToken cancellationToken = default);
    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(T entity, CancellationToken cancellationToken = default);
}

// Read repository (optimized for queries)
public interface IReadOnlyRepository<T, TId>
{
    Task<T?> GetByIdAsync(TId id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<T>> FindAsync(ISpecification<T> specification);
    Task<PagedResult<T>> GetPagedAsync(ISpecification<T> specification, int page, int pageSize);
}
```

### 2. **Specification Pattern**

```csharp
// Define a specification
public class ActiveOrdersSpecification : BaseSpecification<Order>
{
    public ActiveOrdersSpecification()
    {
        AddCriteria(o => o.Status == OrderStatus.Active);
        AddInclude(o => o.Customer);
        ApplyOrderByDescending(o => o.CreatedAt);
    }
}

// Use the specification
var spec = new ActiveOrdersSpecification();
var activeOrders = await _orderReadRepository.FindAsync(spec);
```

### 3. **Unit of Work**

```csharp
// Automatic transaction management
public async Task ProcessOrderAsync(Order order)
{
    await _unitOfWork.BeginTransactionAsync();
    
    try
    {
        await _orderRepository.AddAsync(order);
        await _inventoryRepository.UpdateStockAsync(order.Items);
        await _unitOfWork.SaveChangesAsync();
        await _unitOfWork.CommitTransactionAsync();
    }
    catch
    {
        await _unitOfWork.RollbackTransactionAsync();
        throw;
    }
}
```

### 4. **Message Bus**

```csharp
// Publish an event
await _messageBus.PublishAsync(new OrderCreatedEvent(orderId));

// Publish with delay
await _messageBus.PublishDelayedAsync(new ReminderEvent(), TimeSpan.FromMinutes(5));

// Request-Response pattern
var response = await _messageBus.RequestResponseAsync<GetOrderQuery, OrderDto>(query);
```

### 5. **Caching**

```csharp
// Get or create cached value
var orders = await _cache.GetOrCreateAsync("orders:active", async () =>
{
    return await _orderRepository.GetActiveOrdersAsync();
}, TimeSpan.FromMinutes(10));

// Remove from cache
await _cache.RemoveAsync("orders:active");
```

## ⚙️ Configuration

### appsettings.json Example

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=AutoNextDB;Username=postgres;Password=admin",
    "Redis": "localhost:6379"
  },
  "Database": {
    "CommandTimeout": 30,
    "MaxRetryCount": 5,
    "EnableSensitiveDataLogging": false
  },
  "RabbitMQ": {
    "Host": "localhost",
    "Port": 5672,
    "Username": "guest",
    "Password": "guest",
    "VirtualHost": "/"
  },
  "Cache": {
    "Provider": "Redis",
    "DefaultSlidingExpirationSeconds": 600,
    "DefaultAbsoluteExpirationSeconds": 3600
  },
  "Jwt": {
    "SecretKey": "your-secret-key-minimum-32-characters",
    "Issuer": "https://autonext-api.com",
    "Audience": "autonext-apps",
    "AccessTokenLifetimeMinutes": 15
  },
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "Microsoft.EntityFrameworkCore": "Warning"
      }
    }
  },
  "HealthChecks": {
    "EnableDatabaseCheck": true,
    "EnableRabbitMQCheck": true,
    "EnableRedisCheck": true
  }
}
```

## 📚 Usage Examples

### Example 1: Complete Microservice Setup

```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);

// 1. Add AutoNext Core
builder.Services.AddAutoNextCore(builder.Configuration);

// 2. Add Database
builder.Services.AddPostgreSqlDbContext<AppDbContext>(builder.Configuration);
builder.Services.AddRepositories();

// 3. Add Caching
builder.Services.AddRedisCache(builder.Configuration);

// 4. Add Message Bus
builder.Services.AddRabbitMqMessageBus(builder.Configuration);

// 5. Add CORS
builder.Services.AddAutoNextCors(builder.Configuration);

// 6. Configure Serilog
builder.Host.UseAutoNextSerilog();

var app = builder.Build();

// 7. Use Middlewares
app.UseAutoNextMiddleware();
app.UseAuthentication();
app.UseAuthorization();
app.UseCors("AutoNextCors");

// 8. Map Endpoints
app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
```

### Example 2: Custom Repository Implementation

```csharp
public interface IOrderRepository : IRepository<Order, Guid>
{
    Task<IEnumerable<Order>> GetOrdersByCustomerAsync(Guid customerId);
}

public class OrderRepository : EfRepository<Order, Guid>, IOrderRepository
{
    public OrderRepository(AppDbContext context) : base(context)
    {
    }
    
    public async Task<IEnumerable<Order>> GetOrdersByCustomerAsync(Guid customerId)
    {
        return await _dbSet
            .Where(o => o.CustomerId == customerId)
            .Include(o => o.Items)
            .ToListAsync();
    }
}
```

### Example 3: Event Handler

```csharp
public class OrderCreatedHandler : IMessageConsumer<OrderCreatedEvent>
{
    private readonly ILoggerService<OrderCreatedHandler> _logger;
    private readonly ICacheService _cache;
    
    public OrderCreatedHandler(
        ILoggerService<OrderCreatedHandler> logger,
        ICacheService cache)
    {
        _logger = logger;
        _cache = cache;
    }
    
    public async Task HandleAsync(OrderCreatedEvent message, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Order {OrderId} created", message.OrderId);
        
        // Invalidate cache
        await _cache.RemoveByPrefixAsync($"orders:customer:{message.CustomerId}");
        
        // Process business logic
        // ...
    }
}
```

## 🎯 Best Practices

### 1. **Use Read-Only Repository for Queries**

```csharp
// ✅ GOOD - Read-only for queries
public class OrderQueryService
{
    private readonly IReadOnlyRepository<Order, Guid> _orderRepository;
}

// ❌ BAD - Using write repository for queries
public class OrderQueryService
{
    private readonly IRepository<Order, Guid> _orderRepository;
}
```

### 2. **Use Specifications for Complex Queries**

```csharp
// ✅ GOOD - Encapsulate query logic
var spec = new ActiveOrdersSpecification();
var orders = await _repository.FindAsync(spec);

// ❌ BAD - Scattered query logic
var orders = await _repository.GetQueryable()
    .Where(o => o.Status == "Active")
    .Where(o => o.CreatedAt > DateTime.UtcNow.AddDays(-30))
    .ToListAsync();
```

### 3. **Always Use Cancellation Tokens**

```csharp
// ✅ GOOD - Support cancellation
public async Task<Order> GetOrderAsync(Guid id, CancellationToken cancellationToken)
{
    return await _repository.GetByIdAsync(id, cancellationToken);
}

// ❌ BAD - No cancellation support
public async Task<Order> GetOrderAsync(Guid id)
{
    return await _repository.GetByIdAsync(id);
}
```

### 4. **Cache Appropriately**

```csharp
// ✅ GOOD - Cache read-heavy data
var products = await _cache.GetOrCreateAsync("products:featured", async () =>
{
    return await _productRepository.GetFeaturedProductsAsync();
}, TimeSpan.FromMinutes(15));

// ❌ BAD - Don't cache frequently changing data
var inventory = await _cache.GetOrCreateAsync($"product:{id}:stock", async () =>
{
    return await _inventoryRepository.GetStockAsync(id);
}, TimeSpan.FromSeconds(5)); // Too short to be useful
```

## 🔧 Troubleshooting

### Common Issues and Solutions

| Issue | Solution |
|-------|----------|
| **Connection pool timeout** | Increase `MaxPoolSize` in connection string |
| **RabbitMQ connection drops** | Enable automatic recovery in settings |
| **Redis cache not working** | Check Redis connection string and firewall |
| **Health checks failing** | Verify service dependencies are running |

### Enable Detailed Logging

```csharp
// For development only
builder.Services.AddPostgreSqlDbContext<AppDbContext>(builder.Configuration, options =>
{
    options.EnableSensitiveDataLogging();
    options.EnableDetailedErrors();
});
```

## 🤝 Contributing

We welcome contributions! Please see our [Contributing Guide](CONTRIBUTING.md).

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙏 Acknowledgments

- [Entity Framework Core](https://github.com/dotnet/efcore)
- [Dapper](https://github.com/DapperLib/Dapper)
- [RabbitMQ .NET Client](https://github.com/rabbitmq/rabbitmq-dotnet-client)
- [StackExchange.Redis](https://github.com/StackExchange/StackExchange.Redis)
- [Serilog](https://github.com/serilog/serilog)
- [Polly](https://github.com/App-vNext/Polly)

## 📞 Support

- 📧 Email: support@autonext.com
- 🐛 Issues: [GitHub Issues](https://github.com/autonext/core/issues)
- 📚 Docs: [Documentation](https://docs.autonext.com)

---

**Built with ❤️ by AutoNext Team**

[Report Bug](https://github.com/autonext/core/issues) · [Request Feature](https://github.com/autonext/core/issues)
```

---

## 📝 **Additional Files You May Need**

### **CHANGELOG.md**

```markdown
# Changelog

All notable changes to AutoNext.Core will be documented in this file.

## [1.0.0] - 2024-01-15

### Added
- Initial release of AutoNext.Core
- PostgreSQL EF Core 9.0 support
- Repository and Unit of Work patterns
- Specification pattern implementation
- RabbitMQ message bus integration
- Redis distributed caching
- Serilog logging integration
- Health checks for all services
- Global exception handling middleware
- Correlation ID tracking
- Request logging middleware
- Performance monitoring
- Complete exception hierarchy
- FluentValidation integration
- JWT authentication support
- CORS configuration
- Multi-tenant middleware

### Changed
- N/A (initial release)

### Fixed
- N/A (initial release)

### Security
- Added secure password hashing
- Added SQL injection protection via parameterized queries
```

### **CONTRIBUTING.md**

```markdown
# Contributing to AutoNext.Core

We love your input! We want to make contributing to AutoNext.Core as easy and transparent as possible.

## Development Process

1. Fork the repo and create your branch from `main`
2. Install dependencies: `dotnet restore`
3. Build the project: `dotnet build`
4. Run tests: `dotnet test`
5. Make your changes
6. Submit a pull request

## Pull Request Process

1. Update the README.md with details of changes if needed
2. Update the CHANGELOG.md
3. The PR will be merged once you have the sign-off of maintainers

## Any contributions you make will be under the MIT Software License

When you submit code changes, your submissions are understood to be under the same [MIT License](LICENSE) that covers the project.
```

---

## ✅ **What's Included**

- ✅ Complete README with badges, features, and examples
- ✅ Installation instructions for all methods
- ✅ 10+ real-world code examples
- ✅ Configuration guide
- ✅ Best practices section
- ✅ Troubleshooting table
- ✅ Contributing guidelines
- ✅ Changelog template
- ✅ Support information

**Your README.md is now complete and professional!** 🎉
