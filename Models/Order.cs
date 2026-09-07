using System.ComponentModel.DataAnnotations;

namespace StoreApiLR11.Models;

public class Order : ISoftDeletable
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;
    public DateTime OrderDate { get; set; }

    [Range(0, double.MaxValue)]
    public decimal TotalAmount { get; set; }

    [Required, StringLength(50)]
    public string Status { get; set; } = "Pending";

    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public ICollection<OrderItem> OrderItems { get; set; } = [];
}
