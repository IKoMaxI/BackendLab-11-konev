using System.ComponentModel.DataAnnotations;

namespace StoreApiLR11.DTOs.Customer;

public record CreateCustomerDto(
    [Required, StringLength(150)] string Name,
    [Required, EmailAddress, StringLength(255)] string Email);
