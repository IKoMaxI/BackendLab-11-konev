using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StoreApiLR11.Data;
using StoreApiLR11.DTOs.Customer;
using StoreApiLR11.Extensions;
using StoreApiLR11.Models;

namespace StoreApiLR11.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController(StoreContext context) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CustomerDto>>> GetCustomers()
    {
        var customers = await context.Customers.AsNoTracking()
            .OrderBy(customer => customer.Id)
            .ToListAsync();
        return Ok(customers.Select(customer => customer.ToDto()));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CustomerDto>> GetCustomer(int id)
    {
        var customer = await context.Customers.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        return customer is null ? NotFound() : Ok(customer.ToDto());
    }

    [HttpPost]
    public async Task<ActionResult<CustomerDto>> CreateCustomer(CreateCustomerDto dto)
    {
        if (await context.Customers.AnyAsync(x => x.Email == dto.Email))
        {
            return BadRequest(new { message = "Email is already registered." });
        }

        var customer = new Customer { Name = dto.Name, Email = dto.Email };
        context.Customers.Add(customer);
        await context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetCustomer), new { id = customer.Id }, customer.ToDto());
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateCustomer(int id, UpdateCustomerDto dto)
    {
        var customer = await context.Customers.FindAsync(id);
        if (customer is null) return NotFound();
        if (await context.Customers.AnyAsync(x => x.Email == dto.Email && x.Id != id))
        {
            return BadRequest(new { message = "Email is already registered." });
        }

        customer.Name = dto.Name;
        customer.Email = dto.Email;
        await context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteCustomer(int id)
    {
        var customer = await context.Customers.FindAsync(id);
        if (customer is null) return NotFound();

        customer.IsDeleted = true;
        customer.DeletedAt = DateTime.UtcNow;
        await context.SaveChangesAsync();
        return NoContent();
    }
}
