using Microsoft.Extensions.Options;
using Tgs.GameCommand.Client;
using Tgs.GameCommand.Common;

namespace Tgs.GameRecord.SabaService.Core;

internal class UtensilRepository : IUtensil
{
    private readonly GameRecordInfoOptions _gameRecordInfo;
    private readonly IGameCommandClient _client;

    public UtensilRepository(
        IOptions<GameRecordInfoOptions> options,
        IGameCommandClient client)
    {
        _gameRecordInfo = options.Value;
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public ValueTask<Guid?> BrandGetGameProviderConfigurationIdAsync(
        Guid brandId,
        Guid gameProviderId,
        CancellationToken cancellationToken = default)
        => _client
        .BrandGetGameProviderConfigurationIdAsync(
            brandId: brandId,
            gameProviderId: gameProviderId,
            cancellationToken: cancellationToken);

    public async ValueTask<int> GetGameIdAsync(
        Guid gameProviderId,
        string gameExternalId,
        CancellationToken cancellationToken = default)
        => (await _client
        .GameGetAsync(
            gameProviderId: gameProviderId,
            gameExternalId: gameExternalId,
            cancellationToken: cancellationToken)
        .ConfigureAwait(false))?.GameId
        ?? throw new Exception($"The identifier for this game does not exist.({gameExternalId})");


    public async ValueTask<Guid> GetGameProviderIdAsync(
        CancellationToken cancellationToken = default)
    => await _client
        .GameProviderGetGameProviderIdAsync(
            gameProviderCode: _gameRecordInfo.GameProviderCode,
            cancellationToken)
        .ConfigureAwait(false)
        ?? throw new Exception($"The identifier for this game provider does not exist.({_gameRecordInfo.GameProviderCode})");

    public ValueTask<PlayerInfo?> GetPlayerAsync(
        string playerCode,
        CancellationToken cancellationToken = default)
        => _client
        .PlayerGetAsync(
            playerCode: playerCode,
            cancellationToken: cancellationToken);
}
