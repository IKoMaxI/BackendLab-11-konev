using System.ComponentModel.DataAnnotations;

namespace StoreApiLR11.DTOs.Product;

public record UpdateProductDto(
    [Required, StringLength(200)] string Name,
    [StringLength(1000)] string? Description,
    [Range(0.01, double.MaxValue)] decimal Price,
    [Range(0, int.MaxValue)] int Stock,
    [Range(1, int.MaxValue)] int CategoryId);
