using StoreApiLR11.DTOs.Category;
using StoreApiLR11.DTOs.Customer;
using StoreApiLR11.DTOs.Order;
using StoreApiLR11.DTOs.Product;
using StoreApiLR11.Models;

namespace StoreApiLR11.Extensions;

public static class MappingExtensions
{
    public static CategoryDto ToDto(this Category entity) =>
        new(entity.Id, entity.Name, entity.Description);

    public static ProductDto ToDto(this Product entity) =>
        new(entity.Id, entity.Name, entity.Description, entity.Price, entity.Stock,
            entity.CategoryId, entity.Category?.Name ?? string.Empty);

    public static CustomerDto ToDto(this Customer entity) =>
        new(entity.Id, entity.Name, entity.Email);

    public static OrderDto ToDto(this Order order) => new(
        order.Id,
        order.CustomerId,
        order.Customer?.Name ?? "Unknown",
        order.OrderDate,
        order.TotalAmount,
        order.Status,
        order.OrderItems.Select(item => new OrderItemDto(
            item.ProductId,
            item.Product?.Name ?? string.Empty,
            item.Quantity,
            item.Price,
            item.Quantity * item.Price)).ToList());

    public static void UpdateFrom(this Product entity, UpdateProductDto dto)
    {
        entity.Name = dto.Name;
        entity.Description = dto.Description;
        entity.Price = dto.Price;
        entity.Stock = dto.Stock;
        entity.CategoryId = dto.CategoryId;
    }
}
