using OrderGenerator.Application.DTOs;
using OrderGenerator.Domain.Interfaces;

namespace OrderGenerator.Application.UseCases;

/// <summary>
/// Returns all available tradeable symbols.
/// </summary>
public sealed class GetSymbolsUseCase
{
    private readonly IExchangeApiClient _exchangeApiClient;

    public GetSymbolsUseCase(IExchangeApiClient exchangeApiClient)
    {
        _exchangeApiClient = exchangeApiClient ??
            throw new ArgumentNullException(nameof(exchangeApiClient));
    }

    public async Task<IReadOnlyList<SymbolResponse>> ExecuteAsync(
        CancellationToken cancellationToken = default)
    {
        var symbols =  await _exchangeApiClient.GetSymbolsAsync(cancellationToken);

        return symbols
                .Select(s => new SymbolResponse(s.Ticker, s.Description))
                .ToList().AsReadOnly();

    }
}
