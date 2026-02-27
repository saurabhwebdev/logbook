# Development Guide

This guide explains how to develop new features, add modules, and contribute to CoreEngine.

---

## 📋 Table of Contents

1. [Project Structure](#project-structure)
2. [Architecture Overview](#architecture-overview)
3. [Adding a New Module](#adding-a-new-module)
4. [Creating Entities](#creating-entities)
5. [Implementing CQRS](#implementing-cqrs)
6. [Adding API Endpoints](#adding-api-endpoints)
7. [Building Frontend Pages](#building-frontend-pages)
8. [Testing](#testing)
9. [Code Standards](#code-standards)
10. [Git Workflow](#git-workflow)

---

## 🏗️ Project Structure

```
coreengine/
│
├── 📁 src/
│   ├── 🎯 CoreEngine.Domain/           # Enterprise Entities & Business Logic
│   │   ├── Entities/                   # Domain entities (User, Role, etc.)
│   │   ├── Enums/                      # Enumerations
│   │   └── Common/                     # Base classes (Entity, TenantScopedEntity)
│   │
│   ├── 📋 CoreEngine.Application/      # Use Cases & CQRS Handlers
│   │   ├── Features/                   # CQRS commands & queries by feature
│   │   │   ├── Users/
│   │   │   │   ├── Commands/
│   │   │   │   │   ├── CreateUser/
│   │   │   │   │   │   ├── CreateUserCommand.cs
│   │   │   │   │   │   ├── CreateUserCommandHandler.cs
│   │   │   │   │   │   └── CreateUserCommandValidator.cs
│   │   │   │   │   └── UpdateUser/
│   │   │   │   └── Queries/
│   │   │   │       ├── GetUsers/
│   │   │   │       └── GetUserById/
│   │   │   └── [Other Features]/
│   │   ├── Common/
│   │   │   ├── Interfaces/             # Service interfaces
│   │   │   └── Behaviours/             # Pipeline behaviors
│   │   └── DependencyInjection.cs      # Service registration
│   │
│   ├── 🔧 CoreEngine.Infrastructure/   # Data Access & External Services
│   │   ├── Persistence/
│   │   │   ├── ApplicationDbContext.cs # EF Core DbContext
│   │   │   ├── Migrations/             # EF Core migrations
│   │   │   └── Seed/                   # Database seeding
│   │   ├── Services/                   # Service implementations
│   │   ├── BackgroundJobs/             # Hangfire jobs
│   │   └── DependencyInjection.cs
│   │
│   ├── 🌐 CoreEngine.API/              # REST API Layer
│   │   ├── Controllers/                # API controllers
│   │   ├── Middleware/                 # Custom middleware
│   │   ├── Hubs/                       # SignalR hubs
│   │   ├── Attributes/                 # Custom attributes
│   │   └── Program.cs                  # Application entry point
│   │
│   └── 📦 CoreEngine.Shared/           # Shared Code
│       ├── Constants/                  # PermissionConstants, etc.
│       ├── Exceptions/                 # Custom exceptions
│       └── Extensions/                 # Extension methods
│
└── 📁 frontend/
    └── src/
        ├── 🔌 api/                     # Axios API clients
        ├── 🧩 components/              # Reusable React components
        ├── 🎨 contexts/                # React Context providers
        ├── 📄 pages/                   # Page components
        ├── 🖼️ layouts/                 # Layout components
        └── 📝 types/                   # TypeScript type definitions
```

---

## 🏛️ Architecture Overview

CoreEngine follows **Clean Architecture** with **CQRS pattern**:

```
┌─────────────────────────────────────────────────────────────────┐
│                         🌐 Presentation Layer                    │
│  ┌──────────────────────────┐  ┌──────────────────────────┐   │
│  │   React Frontend (SPA)   │  │   ASP.NET Core API       │   │
│  │   - Pages, Components    │  │   - Controllers          │   │
│  │   - State Management     │  │   - Middleware           │   │
│  └──────────────────────────┘  └──────────────────────────┘   │
└─────────────────────────────────────────────────────────────────┘
                            ↓                      ↓
┌─────────────────────────────────────────────────────────────────┐
│                      📋 Application Layer (CQRS)                 │
│  ┌──────────────────────────┐  ┌──────────────────────────┐   │
│  │     Commands (Write)     │  │    Queries (Read)        │   │
│  │  - CreateUserCommand     │  │  - GetUsersQuery         │   │
│  │  - UpdateUserCommand     │  │  - GetUserByIdQuery      │   │
│  │  - Validators            │  │  - DTOs                  │   │
│  └──────────────────────────┘  └──────────────────────────┘   │
│                MediatR (Command/Query Bus)                       │
└─────────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────────┐
│                      🎯 Domain Layer (Core)                      │
│  ┌──────────────────────────────────────────────────────────┐  │
│  │         Entities (User, Role, Tenant, etc.)              │  │
│  │         Business Rules & Domain Logic                    │  │
│  │         No Dependencies on Other Layers                  │  │
│  └──────────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────────┐
│                   🔧 Infrastructure Layer                        │
│  ┌──────────────────────────┐  ┌──────────────────────────┐   │
│  │   EF Core DbContext      │  │   External Services      │   │
│  │   - Repository Pattern   │  │   - Email (MailKit)      │   │
│  │   - Unit of Work         │  │   - Storage (Local/S3)   │   │
│  │   - Migrations           │  │   - SignalR Hubs         │   │
│  └──────────────────────────┘  └──────────────────────────┘   │
└─────────────────────────────────────────────────────────────────┘
                            ↓
                    💾 Database (SQL Server)
```

### 📊 Request Flow Diagram

```
┌─────────┐
│ Client  │
│ (React) │
└────┬────┘
     │ 1. HTTP Request (POST /api/users)
     ↓
┌────────────────┐
│  API Layer     │
│  Controller    │ ──→ [Authorize] attribute checks JWT
└────┬───────────┘
     │ 2. Send Command/Query via MediatR
     ↓
┌────────────────┐
│ Application    │
│ Command        │ ──→ FluentValidation checks rules
│ Handler        │
└────┬───────────┘
     │ 3. Call Domain Entity
     ↓
┌────────────────┐
│ Domain         │
│ Entity         │ ──→ Business logic validation
│ (User)         │
└────┬───────────┘
     │ 4. Persist via DbContext
     ↓
┌────────────────┐
│ Infrastructure │
│ DbContext      │ ──→ EF Core saves to database
│ SaveChanges    │     (Audit interceptor logs changes)
└────┬───────────┘
     │ 5. Return result
     ↓
┌────────────────┐
│ Response       │
│ (DTO)          │ ──→ JSON response to client
└────────────────┘
```

---

## 🆕 Adding a New Module

Let's create a **"Products"** module as an example:

### Step 1: Plan the Module

Define:
- **Entity structure:** What properties does Product have?
- **Operations:** CRUD, special actions?
- **Permissions:** Product.Create, Product.Read, etc.
- **Frontend:** Pages needed?

### Step 2: Create Domain Entity

📁 `src/CoreEngine.Domain/Entities/Product.cs`

```csharp
namespace CoreEngine.Domain.Entities;

/// <summary>
/// Represents a product in the system
/// </summary>
public class Product : TenantScopedEntity
{
    /// <summary>
    /// Product name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Product description
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Product SKU (Stock Keeping Unit)
    /// </summary>
    public string Sku { get; set; } = string.Empty;

    /// <summary>
    /// Unit price
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Stock quantity
    /// </summary>
    public int StockQuantity { get; set; }

    /// <summary>
    /// Is product active?
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Category ID (optional foreign key)
    /// </summary>
    public Guid? CategoryId { get; set; }

    /// <summary>
    /// Navigation to Category
    /// </summary>
    public Category? Category { get; set; }
}
```

**Key Points:**
- ✅ Extend `TenantScopedEntity` for automatic multi-tenancy
- ✅ Use nullable types for optional fields
- ✅ Add navigation properties for relationships
- ✅ Include XML comments for documentation

### Step 3: Update DbContext

📁 `src/CoreEngine.Infrastructure/Persistence/ApplicationDbContext.cs`

```csharp
public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    // ... existing DbSets ...

    // ✅ Add new DbSet
    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ... existing configurations ...

        // ✅ Configure Product entity
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasIndex(e => e.Sku).IsUnique();
            entity.Property(e => e.Price).HasPrecision(18, 2);
        });
    }
}
```

### Step 4: Create Migration

```bash
cd src/CoreEngine.API
dotnet ef migrations add AddProductEntity --project ../CoreEngine.Infrastructure
dotnet ef database update
```

### Step 5: Add Permissions

📁 `src/CoreEngine.Shared/Constants/PermissionConstants.cs`

```csharp
public static class PermissionConstants
{
    // ... existing permissions ...

    // ✅ Product permissions
    public const string ProductCreate = "Product.Create";
    public const string ProductRead = "Product.Read";
    public const string ProductUpdate = "Product.Update";
    public const string ProductDelete = "Product.Delete";
    public const string ProductExport = "Product.Export";
}
```

### Step 6: Create CQRS Commands

📁 `src/CoreEngine.Application/Features/Products/Commands/CreateProduct/`

**CreateProductCommand.cs:**
```csharp
using MediatR;

namespace CoreEngine.Application.Features.Products.Commands.CreateProduct;

public record CreateProductCommand : IRequest<Guid>
{
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string Sku { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public int StockQuantity { get; init; }
    public Guid? CategoryId { get; init; }
}
```

**CreateProductCommandHandler.cs:**
```csharp
using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Domain.Entities;
using MediatR;

namespace CoreEngine.Application.Features.Products.Commands.CreateProduct;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public CreateProductCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = new Product
        {
            Name = request.Name,
            Description = request.Description,
            Sku = request.Sku,
            Price = request.Price,
            StockQuantity = request.StockQuantity,
            CategoryId = request.CategoryId,
            IsActive = true
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync(cancellationToken);

        return product.Id;
    }
}
```

**CreateProductCommandValidator.cs:**
```csharp
using FluentValidation;

namespace CoreEngine.Application.Features.Products.Commands.CreateProduct;

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(200).WithMessage("Name cannot exceed 200 characters");

        RuleFor(x => x.Sku)
            .NotEmpty().WithMessage("SKU is required")
            .MaximumLength(50).WithMessage("SKU cannot exceed 50 characters");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than 0");

        RuleFor(x => x.StockQuantity)
            .GreaterThanOrEqualTo(0).WithMessage("Stock quantity cannot be negative");
    }
}
```

### Step 7: Create Query

📁 `src/CoreEngine.Application/Features/Products/Queries/GetProducts/`

**GetProductsQuery.cs:**
```csharp
using MediatR;

namespace CoreEngine.Application.Features.Products.Queries.GetProducts;

public record GetProductsQuery : IRequest<List<ProductDto>>
{
    public string? SearchTerm { get; init; }
    public bool? IsActive { get; init; }
}

public record ProductDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string Sku { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public int StockQuantity { get; init; }
    public bool IsActive { get; init; }
    public string? CategoryName { get; init; }
    public DateTime CreatedAt { get; init; }
}
```

**GetProductsQueryHandler.cs:**
```csharp
using CoreEngine.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.Products.Queries.GetProducts;

public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, List<ProductDto>>
{
    private readonly IApplicationDbContext _context;

    public GetProductsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<ProductDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Products.AsQueryable();

        // Apply filters
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            query = query.Where(p =>
                p.Name.Contains(request.SearchTerm) ||
                p.Sku.Contains(request.SearchTerm));
        }

        if (request.IsActive.HasValue)
        {
            query = query.Where(p => p.IsActive == request.IsActive.Value);
        }

        // Project to DTO
        var products = await query
            .Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Sku = p.Sku,
                Price = p.Price,
                StockQuantity = p.StockQuantity,
                IsActive = p.IsActive,
                CategoryName = p.Category != null ? p.Category.Name : null,
                CreatedAt = p.CreatedAt
            })
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);

        return products;
    }
}
```

### Step 8: Create API Controller

📁 `src/CoreEngine.API/Controllers/ProductsController.cs`

```csharp
using CoreEngine.API.Attributes;
using CoreEngine.Application.Features.Products.Commands.CreateProduct;
using CoreEngine.Application.Features.Products.Commands.UpdateProduct;
using CoreEngine.Application.Features.Products.Commands.DeleteProduct;
using CoreEngine.Application.Features.Products.Queries.GetProducts;
using CoreEngine.Application.Features.Products.Queries.GetProductById;
using CoreEngine.Shared.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoreEngine.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get all products
    /// </summary>
    [HttpGet]
    [RequirePermission(PermissionConstants.ProductRead)]
    public async Task<IActionResult> GetAll([FromQuery] string? searchTerm, [FromQuery] bool? isActive)
    {
        var query = new GetProductsQuery
        {
            SearchTerm = searchTerm,
            IsActive = isActive
        };

        var products = await _mediator.Send(query);
        return Ok(products);
    }

    /// <summary>
    /// Get product by ID
    /// </summary>
    [HttpGet("{id}")]
    [RequirePermission(PermissionConstants.ProductRead)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var product = await _mediator.Send(new GetProductByIdQuery { Id = id });
        return Ok(product);
    }

    /// <summary>
    /// Create new product
    /// </summary>
    [HttpPost]
    [RequirePermission(PermissionConstants.ProductCreate)]
    public async Task<IActionResult> Create([FromBody] CreateProductCommand command)
    {
        var productId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = productId }, new { id = productId });
    }

    /// <summary>
    /// Update product
    /// </summary>
    [HttpPut("{id}")]
    [RequirePermission(PermissionConstants.ProductUpdate)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductCommand command)
    {
        if (id != command.Id)
            return BadRequest("ID mismatch");

        await _mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// Delete product
    /// </summary>
    [HttpDelete("{id}")]
    [RequirePermission(PermissionConstants.ProductDelete)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _mediator.Send(new DeleteProductCommand { Id = id });
        return NoContent();
    }
}
```

### Step 9: Create Frontend API Client

📁 `frontend/src/api/productsApi.ts`

```typescript
import apiClient from './client';

export interface Product {
  id: string;
  name: string;
  description: string;
  sku: string;
  price: number;
  stockQuantity: number;
  isActive: boolean;
  categoryName?: string;
  createdAt: string;
}

export interface CreateProductRequest {
  name: string;
  description: string;
  sku: string;
  price: number;
  stockQuantity: number;
  categoryId?: string;
}

export interface UpdateProductRequest extends CreateProductRequest {
  id: string;
}

export const productsApi = {
  getAll: async (searchTerm?: string, isActive?: boolean): Promise<Product[]> => {
    const params = new URLSearchParams();
    if (searchTerm) params.append('searchTerm', searchTerm);
    if (isActive !== undefined) params.append('isActive', isActive.toString());

    const response = await apiClient.get(`/products?${params.toString()}`);
    return response.data;
  },

  getById: async (id: string): Promise<Product> => {
    const response = await apiClient.get(`/products/${id}`);
    return response.data;
  },

  create: async (data: CreateProductRequest): Promise<{ id: string }> => {
    const response = await apiClient.post('/products', data);
    return response.data;
  },

  update: async (id: string, data: UpdateProductRequest): Promise<void> => {
    await apiClient.put(`/products/${id}`, data);
  },

  delete: async (id: string): Promise<void> => {
    await apiClient.delete(`/products/${id}`);
  },
};
```

### Step 10: Create Frontend Page

📁 `frontend/src/pages/ProductsPage.tsx`

```typescript
import { useState } from 'react';
import { Typography, Table, Button, Modal, Form, Input, InputNumber, Switch, Space, message } from 'antd';
import { PlusOutlined, EditOutlined, DeleteOutlined } from '@ant-design/icons';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import type { ColumnsType } from 'antd/es/table';
import { productsApi, type Product, type CreateProductRequest } from '../api/productsApi';
import { useTenantTheme } from '../contexts/ThemeContext';
import PermissionGate from '../components/PermissionGate';

const { Text } = Typography;

export default function ProductsPage() {
  const { theme } = useTenantTheme();
  const queryClient = useQueryClient();
  const [modalOpen, setModalOpen] = useState(false);
  const [editingProduct, setEditingProduct] = useState<Product | null>(null);
  const [form] = Form.useForm();

  const { data: products, isLoading } = useQuery({
    queryKey: ['products'],
    queryFn: () => productsApi.getAll(),
  });

  const createMutation = useMutation({
    mutationFn: productsApi.create,
    onSuccess: () => {
      message.success('Product created');
      queryClient.invalidateQueries({ queryKey: ['products'] });
      setModalOpen(false);
      form.resetFields();
    },
  });

  const columns: ColumnsType<Product> = [
    {
      title: 'Product',
      dataIndex: 'name',
      key: 'name',
      render: (name: string, record: Product) => (
        <div>
          <div style={{ fontWeight: 500, color: '#1d1d1f' }}>{name}</div>
          <div style={{ fontSize: 12, color: '#86868b' }}>{record.sku}</div>
        </div>
      ),
    },
    {
      title: 'Price',
      dataIndex: 'price',
      key: 'price',
      render: (price: number) => `$${price.toFixed(2)}`,
    },
    {
      title: 'Stock',
      dataIndex: 'stockQuantity',
      key: 'stockQuantity',
    },
    {
      title: 'Status',
      dataIndex: 'isActive',
      key: 'isActive',
      render: (isActive: boolean) => (
        <span style={{ color: isActive ? '#34c759' : '#86868b' }}>
          {isActive ? 'Active' : 'Inactive'}
        </span>
      ),
    },
    {
      title: 'Actions',
      key: 'actions',
      render: (_, record: Product) => (
        <Space>
          <PermissionGate permissions={['Product.Update']}>
            <Button type="text" icon={<EditOutlined />} />
          </PermissionGate>
          <PermissionGate permissions={['Product.Delete']}>
            <Button type="text" danger icon={<DeleteOutlined />} />
          </PermissionGate>
        </Space>
      ),
    },
  ];

  return (
    <div>
      <div style={{ marginBottom: 24, display: 'flex', justifyContent: 'space-between' }}>
        <div>
          <h2 style={{ fontSize: 22, fontWeight: 700, color: '#1d1d1f', margin: 0 }}>Products</h2>
          <Text style={{ fontSize: 13, color: '#86868b' }}>Manage your product catalog</Text>
        </div>
        <PermissionGate permissions={['Product.Create']}>
          <Button type="primary" icon={<PlusOutlined />} onClick={() => setModalOpen(true)}>
            Add Product
          </Button>
        </PermissionGate>
      </div>

      <Table
        columns={columns}
        dataSource={products}
        loading={isLoading}
        rowKey="id"
        pagination={{ pageSize: 20 }}
      />

      <Modal
        title="New Product"
        open={modalOpen}
        onCancel={() => setModalOpen(false)}
        onOk={() => form.submit()}
      >
        <Form form={form} layout="vertical" onFinish={(values) => createMutation.mutate(values)}>
          <Form.Item name="name" label="Name" rules={[{ required: true }]}>
            <Input />
          </Form.Item>
          <Form.Item name="sku" label="SKU" rules={[{ required: true }]}>
            <Input />
          </Form.Item>
          <Form.Item name="price" label="Price" rules={[{ required: true }]}>
            <InputNumber style={{ width: '100%' }} prefix="$" min={0} step={0.01} />
          </Form.Item>
          <Form.Item name="stockQuantity" label="Stock" rules={[{ required: true }]}>
            <InputNumber style={{ width: '100%' }} min={0} />
          </Form.Item>
          <Form.Item name="description" label="Description">
            <Input.TextArea rows={3} />
          </Form.Item>
        </Form>
      </Modal>
    </div>
  );
}
```

### Step 11: Add Navigation

📁 `frontend/src/layouts/MainLayout.tsx`

```typescript
// Add to menu items array
{
  key: '/products',
  icon: <ShoppingOutlined />,
  label: 'Products',
  permission: 'Product.Read',
}
```

### Step 12: Add Route

📁 `frontend/src/App.tsx`

```typescript
import ProductsPage from './pages/ProductsPage';

// Add to Routes
<Route path="/products" element={<ProtectedRoute><ProductsPage /></ProtectedRoute>} />
```

---

## 🧪 Testing

### Unit Testing (Backend)

Create test project:
```bash
dotnet new xunit -n CoreEngine.Tests
cd CoreEngine.Tests
dotnet add reference ../src/CoreEngine.Application
dotnet add package Moq
dotnet add package FluentAssertions
```

Example test:

```csharp
using CoreEngine.Application.Features.Products.Commands.CreateProduct;
using FluentAssertions;
using Xunit;

namespace CoreEngine.Tests.Features.Products;

public class CreateProductCommandValidatorTests
{
    [Fact]
    public void Should_Have_Error_When_Name_Is_Empty()
    {
        // Arrange
        var validator = new CreateProductCommandValidator();
        var command = new CreateProductCommand { Name = "" };

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(command.Name));
    }
}
```

---

## 📏 Code Standards

### C# Standards

```csharp
✅ DO:
- Use PascalCase for classes, methods, properties
- Use camelCase for local variables, parameters
- Use explicit types (not var) for clarity
- Add XML comments for public APIs
- Keep methods under 50 lines
- Use async/await for I/O operations
- Follow SOLID principles

❌ DON'T:
- Use magic numbers (define constants)
- Catch generic exceptions without re-throwing
- Use Thread.Sleep (use Task.Delay)
- Expose domain entities in API responses (use DTOs)
```

### TypeScript Standards

```typescript
✅ DO:
- Use TypeScript strict mode
- Define interfaces for all data structures
- Use functional components with hooks
- Extract reusable logic into custom hooks
- Use React Query for server state
- Use const for immutable values

❌ DON'T:
- Use 'any' type (use 'unknown' if needed)
- Mutate state directly (use setState)
- Forget to handle loading/error states
- Put business logic in components
```

---

## 🔀 Git Workflow

### Branching Strategy

```
main (protected)
├── feature/add-products-module
├── bugfix/fix-email-validation
└── hotfix/critical-security-patch
```

### Commit Message Format

```
<type>: <subject>

<body>

<footer>
```

**Types:**
- `feat`: New feature
- `fix`: Bug fix
- `docs`: Documentation changes
- `style`: Code formatting
- `refactor`: Code restructuring
- `test`: Adding tests
- `chore`: Maintenance tasks

**Example:**
```
feat: Add Products module with CRUD operations

- Created Product entity with SKU, price, stock fields
- Implemented CQRS commands and queries
- Added ProductsController with full CRUD API
- Built ProductsPage with table and create modal
- Added Product.* permissions

Closes #123
```

---

## 🚀 Deployment Checklist

Before deploying:

- [ ] Run all tests: `dotnet test`
- [ ] Build frontend: `npm run build`
- [ ] Update database: `dotnet ef database update`
- [ ] Check migrations folder for new files
- [ ] Update appsettings.Production.json
- [ ] Set environment variables for secrets
- [ ] Run security scan
- [ ] Test in staging environment
- [ ] Create deployment documentation
- [ ] Notify team of deployment

---

## 📚 Additional Resources

- [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [CQRS Pattern](https://docs.microsoft.com/en-us/azure/architecture/patterns/cqrs)
- [MediatR Documentation](https://github.com/jbogard/MediatR)
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)
- [React Best Practices](https://react.dev/learn)

---

**Next:** [Deployment Guide →](DEPLOYMENT.md)
