namespace OrderGenerator.Domain.DTOs;

/// <summary>
/// Represents a tradeable symbol returned to the client.
/// </summary>
public sealed record SymbolResponse(
    string Ticker,
    string? Description
);
