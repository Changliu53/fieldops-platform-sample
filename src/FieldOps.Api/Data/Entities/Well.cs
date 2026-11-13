namespace FieldOps.Api.Data.Entities;

public class Well : ITenantOwned
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;
    public string Name { get; set; } = "";
    public decimal DepthFeet { get; set; }
}
