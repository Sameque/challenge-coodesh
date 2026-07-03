using OrderGenerator.Application.DTOs;
using OrderGenerator.Domain.Enums;
using OrderGenerator.Domain.Interfaces;

namespace OrderGenerator.Application.UseCases;

/// <summary>
/// Calculates the total financial exposure per symbol
/// by aggregating accepted buy orders.
/// </summary>
public sealed class GetExposureUseCase
{
    private readonly IExchangeApiClient _exchangeApiClient;

    public GetExposureUseCase(IExchangeApiClient exchangeApiClient)
    {
        _exchangeApiClient = exchangeApiClient ??
            throw new ArgumentNullException(nameof(exchangeApiClient));
    }

    public async Task<IReadOnlyList<ExposureResponse>> ExecuteAsync(
        CancellationToken cancellationToken = default)
    {
        return await _exchangeApiClient.GetExposureAsync(cancellationToken);
    }
}
