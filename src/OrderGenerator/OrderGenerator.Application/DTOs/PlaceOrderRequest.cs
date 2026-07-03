using OrderGenerator.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace OrderGenerator.Application.DTOs;

/// <summary>
/// Payload sent by the client to register a new order.
/// </summary>
public sealed record PlaceOrderRequest
{
    [Required]
    [MinLength(4)]
    [MaxLength(10)]
    public string Symbol { get; init; } = string.Empty;

    [Required]
    public OrderSide Side { get; init; }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than zero.")]
    public int Quantity { get; init; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]
    public decimal Price { get; init; }
}
