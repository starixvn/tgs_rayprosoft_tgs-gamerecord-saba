using Microsoft.Extensions.Options;
using Tgs.GameProviders.QueryServer.Client;
using Tgs.GameProviders.QueryServer.Models;

namespace Tgs.GameRecord.SabaService.Core;

internal class UtensilRepository : IUtensil
{
    private readonly GameRecordInfoOptions _gameRecordInfo;
    private readonly IGameProvidersQueryServerClient _client;

    public UtensilRepository(
         IOptions<GameRecordInfoOptions> options,
        IGameProvidersQueryServerClient client)
    {
        _gameRecordInfo = options.Value;
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public ValueTask<Guid?> BrandGetGameProviderConfigurationIdAsync(Guid brandId, Guid gameProviderId)
        => _client.BrandGetGameProviderConfigurationIdAsync(brandId, gameProviderId);

    public async ValueTask<int> GetGameIdAsync(Guid gameProviderId, string gameExternalId)
    {
        var gameModel = await _client.GameGetAsync(gameProviderId, gameExternalId).ConfigureAwait(false);
        return gameModel?.GameId ?? throw new Exception($"The identifier for this game does not exist.({gameExternalId})");
    }

    public async ValueTask<Guid> GetGameProviderIdAsync()
    => await _client.GameProviderGetGameProviderIdAsync(_gameRecordInfo.GameProviderCode).ConfigureAwait(false)
        ?? throw new Exception($"The identifier for this game provider does not exist.({_gameRecordInfo.GameProviderCode})");

    public ValueTask<PlayerModel?> GetPlayerAsync(string playerCode)
        => _client.PlayerGetAsync(playerCode);
}
