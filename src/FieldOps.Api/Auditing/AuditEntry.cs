namespace FieldOps.Api.Auditing;

/// <summary>One change to a record: who made it and when.</summary>
public class AuditEntry
{
    public long Id { get; set; }
    public int TenantId { get; set; }
    public string EntityName { get; set; } = "";
    public int EntityId { get; set; }
    public string Action { get; set; } = "";
    public string ChangedBy { get; set; } = "";
    public DateTime ChangedAtUtc { get; set; }
}
