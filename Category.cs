using System.ComponentModel.DataAnnotations;

namespace StoreApiLR11.Models;

public class Category : ISoftDeletable
{
    public int Id { get; set; }

    [Required, StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public ICollection<Product> Products { get; set; } = [];
}
