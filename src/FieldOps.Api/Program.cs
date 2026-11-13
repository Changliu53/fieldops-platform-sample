using FieldOps.Api.Auditing;
using FieldOps.Api.Data;
using FieldOps.Api.Tenancy;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ITenantContext, HttpTenantContext>();
builder.Services.AddScoped<IAuditLog, AuditLog>();
builder.Services.AddDbContext<PlatformDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Platform")));

builder.Services.AddAuthentication().AddJwtBearer();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("WorkOrders.Read", policy => policy.RequireClaim("scope", "workorders.read"));
    options.AddPolicy("WorkOrders.Write", policy => policy.RequireClaim("scope", "workorders.write"));
    options.AddPolicy("Customers.Read", policy => policy.RequireClaim("scope", "customers.read"));
    options.AddPolicy("Customers.Write", policy => policy.RequireClaim("scope", "customers.write"));
});

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
