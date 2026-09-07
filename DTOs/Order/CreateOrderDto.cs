using System.ComponentModel.DataAnnotations;

namespace StoreApiLR11.DTOs.Order;

public record CreateOrderDto(
    [Range(1, int.MaxValue)] int CustomerId,
    [Required, MinLength(1)] List<CreateOrderItemDto> Items);

public record CreateOrderItemDto(
    [Range(1, int.MaxValue)] int ProductId,
    [Range(1, 1000)] int Quantity);
