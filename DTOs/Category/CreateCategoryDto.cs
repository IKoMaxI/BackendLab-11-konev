using System.ComponentModel.DataAnnotations;

namespace StoreApiLR11.DTOs.Category;

public record CreateCategoryDto(
    [Required, StringLength(100)] string Name,
    [StringLength(500)] string? Description);
