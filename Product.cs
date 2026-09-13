using System.ComponentModel.DataAnnotations;

namespace StoreApiLR11.Models;

public class Product : ISoftDeletable
{
    public int Id { get; set; }

    [Required, StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; set; }

    [Range(0.01, double.MaxValue)]
    public decimal Price { get; set; }

    [Range(0, int.MaxValue)]
    public int Stock { get; set; }

    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public ICollection<OrderItem> OrderItems { get; set; } = [];
}
