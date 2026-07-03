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
        return await _exchangeApiClient.GetSymbolsAsync(cancellationToken);
    }
}
