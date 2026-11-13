using FieldOps.Api.Auditing;
using FieldOps.Api.Data.Entities;
using FieldOps.Api.Tenancy;
using Microsoft.EntityFrameworkCore;

namespace FieldOps.Api.Data;

public class PlatformDbContext : DbContext
{
    private readonly ITenantContext _tenant;

    public PlatformDbContext(DbContextOptions<PlatformDbContext> options, ITenantContext tenant)
        : base(options)
    {
        _tenant = tenant;
    }

    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Well> Wells => Set<Well>();
    public DbSet<WorkOrder> WorkOrders => Set<WorkOrder>();
    public DbSet<AuditEntry> AuditEntries => Set<AuditEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Tenant isolation. Every query on these tables is limited to the
        // current tenant unless a caller turns the filter off, which is only
        // allowed in admin tooling.
        modelBuilder.Entity<Customer>().HasQueryFilter(c => c.TenantId == _tenant.TenantId);
        modelBuilder.Entity<Well>().HasQueryFilter(w => w.TenantId == _tenant.TenantId);
        modelBuilder.Entity<WorkOrder>().HasQueryFilter(w => w.TenantId == _tenant.TenantId);

        modelBuilder.Entity<Customer>().Property(c => c.Name).HasMaxLength(200);
        modelBuilder.Entity<Customer>().Property(c => c.Phone).HasMaxLength(32);
        modelBuilder.Entity<Well>().Property(w => w.Name).HasMaxLength(200);
        modelBuilder.Entity<Well>().Property(w => w.DepthFeet).HasPrecision(9, 2);
        modelBuilder.Entity<WorkOrder>().Property(w => w.CustomerName).HasMaxLength(200);
        modelBuilder.Entity<WorkOrder>().Property(w => w.QuotedTotal).HasPrecision(12, 2);
        modelBuilder.Entity<WorkOrder>().HasIndex(w => new { w.TenantId, w.Status });
    }
}
