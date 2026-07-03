namespace OrderGenerator.Application.DTOs;

/// <summary>
/// Represents the financial exposure of a single symbol.
/// </summary>
public sealed record ExposureResponse(
    string Symbol,
    decimal ValueExposed
);
