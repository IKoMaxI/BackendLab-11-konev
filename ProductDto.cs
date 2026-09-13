namespace StoreApiLR11.DTOs.Product;

public record ProductDto(
    int Id,
    string Name,
    string? Description,
    decimal Price,
    int Stock,
    int CategoryId,
    string CategoryName);
