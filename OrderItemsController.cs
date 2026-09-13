using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StoreApiLR11.Data;
using StoreApiLR11.Models;

namespace StoreApiLR11.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrderItemsController(StoreContext context) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await context.OrderItems.AsNoTracking()
        .Select(x => new ItemResponse(x.Id, x.OrderId, x.ProductId, x.Quantity, x.Price)).ToListAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var item = await context.OrderItems.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        return item is null ? NotFound() : Ok(ToResponse(item));
    }

    [HttpPost]
    public Task<IActionResult> Create(ItemRequest dto) => Change(null, dto, false);

    [HttpPut("{id:int}")]
    public Task<IActionResult> Update(int id, ItemRequest dto) => Change(id, dto, false);

    [HttpDelete("{id:int}")]
    public Task<IActionResult> Delete(int id) => Change(id, null, true);

    private Task<IActionResult> Change(int? id, ItemRequest? dto, bool delete)
    {
        return context.Database.CreateExecutionStrategy().ExecuteAsync<IActionResult>(async () =>
        {
            context.ChangeTracker.Clear();
            await using var transaction = await context.Database.BeginTransactionAsync(
                System.Data.IsolationLevel.Serializable);
            var item = id.HasValue ? await context.OrderItems.FindAsync(id.Value) : null;
            if (id.HasValue && item is null) return NotFound();
            if (item is not null && dto is not null && item.OrderId != dto.OrderId)
                return BadRequest(new { message = "Cannot move an item to another order." });

            var order = await context.Orders.FindAsync(item?.OrderId ?? dto!.OrderId);
            if (order is null) return NotFound();
            if (order.Status != "Pending")
                return Conflict(new { message = "Only pending orders can be edited." });

            if (item is not null)
            {
                var oldProduct = await context.Products.IgnoreQueryFilters()
                    .FirstAsync(x => x.Id == item.ProductId);
                oldProduct.Stock += item.Quantity;
                order.TotalAmount -= item.Quantity * item.Price;
            }

            if (delete)
            {
                item!.IsDeleted = true;
                item.DeletedAt = DateTime.UtcNow;
            }
            else
            {
                var product = await context.Products.FirstOrDefaultAsync(x => x.Id == dto!.ProductId);
                if (product is null) return NotFound();
                if (product.Stock < dto!.Quantity)
                    return BadRequest(new { message = "Not enough stock." });
                product.Stock -= dto.Quantity;
                var sameProduct = item?.ProductId == product.Id;
                item ??= new OrderItem { OrderId = order.Id };
                item.ProductId = product.Id;
                item.Quantity = dto.Quantity;
                if (!sameProduct) item.Price = product.Price;
                order.TotalAmount += item.Quantity * item.Price;
                if (!id.HasValue) context.OrderItems.Add(item);
            }

            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            return id.HasValue ? NoContent()
                : CreatedAtAction(nameof(GetById), new { id = item!.Id }, ToResponse(item));
        });
    }

    private static ItemResponse ToResponse(OrderItem item) =>
        new(item.Id, item.OrderId, item.ProductId, item.Quantity, item.Price);

    public record ItemRequest(
        [property: Range(1, int.MaxValue)] int OrderId,
        [property: Range(1, int.MaxValue)] int ProductId,
        [property: Range(1, int.MaxValue)] int Quantity);

    public record ItemResponse(int Id, int OrderId, int ProductId, int Quantity, decimal Price);
}
