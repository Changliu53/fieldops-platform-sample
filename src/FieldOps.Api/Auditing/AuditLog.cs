using FieldOps.Api.Data;
using FieldOps.Api.Tenancy;

namespace FieldOps.Api.Auditing;

public interface IAuditLog
{
    /// <summary>Adds an entry to the current unit of work. The caller saves.</summary>
    void Record(string entityName, int entityId, string action);
}

public class AuditLog : IAuditLog
{
    private readonly PlatformDbContext _db;
    private readonly ITenantContext _tenant;
    private readonly IHttpContextAccessor _http;

    public AuditLog(PlatformDbContext db, ITenantContext tenant, IHttpContextAccessor http)
    {
        _db = db;
        _tenant = tenant;
        _http = http;
    }

    public void Record(string entityName, int entityId, string action)
    {
        // The user comes from the session, never from the request body.
        var user = _http.HttpContext?.User.Identity?.Name ?? "unknown";
        _db.AuditEntries.Add(new AuditEntry
        {
            TenantId = _tenant.TenantId,
            EntityName = entityName,
            EntityId = entityId,
            Action = action,
            ChangedBy = user,
            ChangedAtUtc = DateTime.UtcNow,
        });
    }
}
