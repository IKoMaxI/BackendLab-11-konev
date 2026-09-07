using System.ComponentModel.DataAnnotations;

namespace StoreApiLR11.DTOs.Order;

public record UpdateOrderStatusDto(
    [Required, RegularExpression("^(Pending|Processing|Completed|Cancelled)$")]
    string Status);
