using FieldOps.Api.Auditing;
using FieldOps.Api.Data;
using FieldOps.Api.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FieldOps.Api.Customers;

public record UpdatePhoneRequest(string? Phone);

[ApiController]
[Route("api/customers")]
public class CustomersController : ControllerBase
{
    // The Phone column holds up to 32 characters.
    private const int MaxPhoneLength = 32;
    private const string PhonePunctuation = "+() .-";

    private readonly PlatformDbContext _db;
    private readonly IAuditLog _audit;

    public CustomersController(PlatformDbContext db, IAuditLog audit)
    {
        _db = db;
        _audit = audit;
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

    [HttpPut("{id:int}/phone")]
    [Authorize(Policy = "Customers.Write")]
    public async Task<IActionResult> UpdatePhone(int id, [FromBody] UpdatePhoneRequest request, CancellationToken ct)
    {
        // An empty value clears the number.
        var phone = string.IsNullOrWhiteSpace(request.Phone) ? null : request.Phone.Trim();
        if (phone is not null && !IsValidPhone(phone))
        {
            return BadRequest("A phone number has at most 32 characters: digits, spaces, and + ( ) . -");
        }

        // The DbContext's query filter limits this to the current tenant, so
        // another tenant's customer is reported as not found.
        var customer = await _db.Customers.FirstOrDefaultAsync(c => c.Id == id, ct);
        if (customer is null)
        {
            return NotFound();
        }
        if (customer.Phone == phone)
        {
            return NoContent();
        }

        customer.Phone = phone;
        _audit.Record(nameof(Customer), customer.Id, "PhoneChanged");
        await _db.SaveChangesAsync(ct);
        return NoContent();
    }

    private static bool IsValidPhone(string phone)
    {
        return phone.Length <= MaxPhoneLength
            && phone.Any(char.IsDigit)
            && phone.All(c => char.IsDigit(c) || PhonePunctuation.Contains(c));
    }
}
