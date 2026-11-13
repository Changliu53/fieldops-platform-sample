namespace FieldOps.Api.Data.Entities;

public enum WorkOrderStatus
{
    Open = 0,
    InProgress = 1,
    Closed = 2,
}

public class WorkOrder : ITenantOwned
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public int WellId { get; set; }
    public Well Well { get; set; } = null!;
    public string CustomerName { get; set; } = "";
    public string Description { get; set; } = "";
    public WorkOrderStatus Status { get; set; }
    public DateTime ScheduledFor { get; set; }
    public decimal QuotedTotal { get; set; }
    public string? ClosedBy { get; set; }
    public DateTime? ClosedAt { get; set; }
}
