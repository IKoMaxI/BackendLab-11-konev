using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StoreApiLR11.Data;
using StoreApiLR11.DTOs.Product;
using StoreApiLR11.Extensions;
using StoreApiLR11.Models;

namespace StoreApiLR11.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController(StoreContext context) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts(
        int page = 1,
        int pageSize = 20,
        string sortBy = "id",
        string sortOrder = "asc",
        int? categoryId = null)
    {
        if (page < 1 || pageSize is < 1 or > 100) return BadRequest(new { message = "Invalid pagination values." });

        var query = context.Products.AsNoTracking().Include(product => product.Category).AsQueryable();

        if (categoryId.HasValue)
        {
            query = query.Where(product => product.CategoryId == categoryId.Value);
        }

        var descending = sortOrder.Equals("desc", StringComparison.OrdinalIgnoreCase);
        query = sortBy.ToLowerInvariant() switch
        {
            "name" => descending ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name),
            "price" => descending ? query.OrderByDescending(x => x.Price) : query.OrderBy(x => x.Price),
            "stock" => descending ? query.OrderByDescending(x => x.Stock) : query.OrderBy(x => x.Stock),
            _ => descending ? query.OrderByDescending(x => x.Id) : query.OrderBy(x => x.Id)
        };

        var products = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return Ok(products.Select(product => product.ToDto()));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProductDto>> GetProduct(int id)
    {
        var product = await context.Products
            .AsNoTracking()
            .Include(x => x.Category)
            .FirstOrDefaultAsync(x => x.Id == id);

        return product is null
            ? NotFound(new { message = $"Product with Id={id} not found." })
            : Ok(product.ToDto());
    }

    [HttpPost]
    public async Task<ActionResult<ProductDto>> CreateProduct(CreateProductDto dto)
    {
        if (!await context.Categories.AnyAsync(x => x.Id == dto.CategoryId))
        {
            return BadRequest(new { message = "Category not found." });
        }

        var product = new Product
        {
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            Stock = dto.Stock,
            CategoryId = dto.CategoryId
        };

        context.Products.Add(product);
        await context.SaveChangesAsync();
        await context.Entry(product).Reference(x => x.Category).LoadAsync();

        return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product.ToDto());
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateProduct(int id, UpdateProductDto dto)
    {
        var product = await context.Products.FindAsync(id);
        if (product is null) return NotFound();
        if (!await context.Categories.AnyAsync(x => x.Id == dto.CategoryId))
        {
            return BadRequest(new { message = "Category not found." });
        }

        product.UpdateFrom(dto);
        await context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var product = await context.Products.FindAsync(id);
        if (product is null) return NotFound();

        product.IsDeleted = true;
        product.DeletedAt = DateTime.UtcNow;
        await context.SaveChangesAsync();
        return NoContent();
    }
}
