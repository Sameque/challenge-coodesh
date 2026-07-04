namespace OrderGenerator.Infrastructure.Configuration;

/// <summary>
/// Configuration settings for the FIX 4.4 exchange connection.
/// Bound from appsettings.json section "FixSettings".
/// </summary>
public sealed class ExchangeSettings
{
    public const string SectionName = "ExchangeSettings";
    public string ApiBaseUrl { get; set; } = string.Empty;
}
