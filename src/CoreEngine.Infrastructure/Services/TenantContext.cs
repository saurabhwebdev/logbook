using CoreEngine.Domain.Interfaces;

namespace CoreEngine.Infrastructure.Services;

public class TenantContext : ITenantContext
{
    public Guid TenantId { get; private set; } = Guid.Empty;
    public string TenantName { get; private set; } = string.Empty;

    public void SetTenant(Guid tenantId, string tenantName)
    {
        TenantId = tenantId;
        TenantName = tenantName;
    }
}
