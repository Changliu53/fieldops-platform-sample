using FieldOps.Api.Data;
using FieldOps.Api.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FieldOps.Api.Customers;

[ApiController]
[Route("api/customers")]
public class CustomersController : ControllerBase
{
    private readonly PlatformDbContext _db;

    public CustomersController(PlatformDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    [Authorize(Policy = "Customers.Read")]
    public async Task<List<Customer>> List(CancellationToken ct)
    {
        return await _db.Customers.OrderBy(c => c.Name).Take(200).ToListAsync(ct);
    }

    [HttpGet("{id:int}")]
    [Authorize(Policy = "Customers.Read")]
    public async Task<ActionResult<Customer>> Get(int id, CancellationToken ct)
    {
        var customer = await _db.Customers
            .Include(c => c.Wells)
            .FirstOrDefaultAsync(c => c.Id == id, ct);
        return customer is null ? NotFound() : customer;
    }
}
