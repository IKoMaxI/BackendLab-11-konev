using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StoreApiLR11.Data;
using StoreApiLR11.DTOs.Category;
using StoreApiLR11.Extensions;
using StoreApiLR11.Models;

namespace StoreApiLR11.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController(StoreContext context) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategories()
    {
        var categories = await context.Categories
            .AsNoTracking()
            .OrderBy(category => category.Id)
            .ToListAsync();

        return Ok(categories.Select(category => category.ToDto()));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CategoryDto>> GetCategory(int id)
    {
        var category = await context.Categories.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        return category is null ? NotFound() : Ok(category.ToDto());
    }

    [HttpPost]
    public async Task<ActionResult<CategoryDto>> CreateCategory(CreateCategoryDto dto)
    {
        var category = new Category { Name = dto.Name, Description = dto.Description };
        context.Categories.Add(category);
        await context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, category.ToDto());
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateCategory(int id, UpdateCategoryDto dto)
    {
        var category = await context.Categories.FindAsync(id);
        if (category is null) return NotFound();

        category.Name = dto.Name;
        category.Description = dto.Description;
        await context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        var category = await context.Categories.FindAsync(id);
        if (category is null) return NotFound();

        category.IsDeleted = true;
        category.DeletedAt = DateTime.UtcNow;
        await context.SaveChangesAsync();
        return NoContent();
    }
}
