namespace StoreApiLR11.DTOs.Order;

public record OrderDto(
    int Id,
    int CustomerId,
    string CustomerName,
    DateTime OrderDate,
    decimal TotalAmount,
    string Status,
    List<OrderItemDto> Items);

public record OrderItemDto(
    int ProductId,
    string ProductName,
    int Quantity,
    decimal Price,
    decimal Subtotal);
