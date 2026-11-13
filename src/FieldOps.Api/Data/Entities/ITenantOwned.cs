namespace FieldOps.Api.Data.Entities;

/// <summary>Marks an entity whose rows belong to one tenant.</summary>
public interface ITenantOwned
{
    int TenantId { get; set; }
}
