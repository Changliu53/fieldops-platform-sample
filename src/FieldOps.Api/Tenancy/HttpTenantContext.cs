namespace FieldOps.Api.Tenancy;

/// <summary>
/// Reads the tenant from the signed-in user's token. The tenant is never taken
/// from the URL, a header, or the request body, because the caller controls those.
/// </summary>
public class HttpTenantContext : ITenantContext
{
    private readonly IHttpContextAccessor _http;

    public HttpTenantContext(IHttpContextAccessor http)
    {
        _http = http;
    }

    public int TenantId
    {
        get
        {
            var claim = _http.HttpContext?.User.FindFirst("tenant_id")?.Value;
            if (!int.TryParse(claim, out var tenantId))
            {
                throw new UnauthorizedAccessException("The request has no tenant.");
            }
            return tenantId;
        }
    }
}
