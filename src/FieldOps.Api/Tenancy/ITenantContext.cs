namespace FieldOps.Api.Tenancy;

/// <summary>The operating company the current request belongs to.</summary>
public interface ITenantContext
{
    int TenantId { get; }
}
