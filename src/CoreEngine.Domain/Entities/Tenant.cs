using CoreEngine.Domain.Common;

namespace CoreEngine.Domain.Entities;

public class Tenant : BaseEntity
{
    public string Name { get; set; } = default!;
    public string Subdomain { get; set; } = default!;
    public string? ConnectionString { get; set; }
    public bool IsActive { get; set; } = true;
    public string? Settings { get; set; }

    // Branding / Theming
    public string? LogoUrl { get; set; }
    public string? PrimaryColor { get; set; }
    public string? SidebarColor { get; set; }

    // Navigation
    public ICollection<User> Users { get; set; } = new List<User>();
    public ICollection<Role> Roles { get; set; } = new List<Role>();
    public ICollection<Department> Departments { get; set; } = new List<Department>();
}
