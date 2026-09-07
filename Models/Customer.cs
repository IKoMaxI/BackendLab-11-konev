using System.ComponentModel.DataAnnotations;

namespace StoreApiLR11.Models;

public class Customer : ISoftDeletable
{
    public int Id { get; set; }

    [Required, StringLength(150)]
    public string Name { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(255)]
    public string Email { get; set; } = string.Empty;

    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public ICollection<Order> Orders { get; set; } = [];
}
