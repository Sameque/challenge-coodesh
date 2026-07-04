namespace OrderGenerator.Domain.DTOs;

/// <summary>
/// Represents the financial exposure of a single symbol.
/// </summary>
public sealed record ExposureResponse(
    string Ticker,
    decimal ValueExposed
);
