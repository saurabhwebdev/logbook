using CoreEngine.Domain.Common;

namespace CoreEngine.Domain.Entities;

public class Department : TenantScopedEntity
{
    public string Name { get; set; } = default!;
    public string? Code { get; set; }
    public Guid? ParentDepartmentId { get; set; }

    // Navigation
    public Tenant Tenant { get; set; } = default!;
    public Department? ParentDepartment { get; set; }
    public ICollection<Department> ChildDepartments { get; set; } = new List<Department>();
    public ICollection<User> Users { get; set; } = new List<User>();
}
