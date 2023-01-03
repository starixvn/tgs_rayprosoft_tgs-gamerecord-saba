using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Tgs.GameCommand.Common;

namespace Tgs.GameRecord.SabaService.Core;

internal class UtensilMemoryCacheRepository : IUtensil
{
    private readonly TimeSpan _absoluteExpirationRelativeToNow;
    private readonly GameRecordInfoOptions _gameRecordInfo;
    private readonly IMemoryCache _memoryCache;
    private readonly IUtensil _utensil;

    public UtensilMemoryCacheRepository(
        IOptions<GameRecordInfoOptions> options,
        IMemoryCache memoryCache,
        IUtensil utensil,
        TimeSpan absoluteExpirationRelativeToNow)
    {
        _gameRecordInfo = options.Value;
        _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        _utensil = utensil ?? throw new ArgumentNullException(nameof(utensil));
        _absoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow;
    }

    public async ValueTask<Guid?> BrandGetGameProviderConfigurationIdAsync(
        Guid brandId,
        Guid gameProviderId,
        CancellationToken cancellationToken = default)
    {
        var key = $"{_gameRecordInfo.GameProviderCode}:{brandId}:{gameProviderId}";
        if (!_memoryCache.TryGetValue(key, out Guid? gameProviderConfigurationId))
        {
            gameProviderConfigurationId = await _utensil
                .BrandGetGameProviderConfigurationIdAsync(
                    brandId: brandId,
                    gameProviderId: gameProviderId,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            if (gameProviderConfigurationId == null)
            {
                return null;
            }
            _ = _memoryCache.Set(key, gameProviderConfigurationId, absoluteExpirationRelativeToNow: _absoluteExpirationRelativeToNow);
        }
        return gameProviderConfigurationId;
    }

    public async ValueTask<int> GetGameIdAsync(
        Guid gameProviderId,
        string gameExternalId,
        CancellationToken cancellationToken = default)
    {
        var key = $"{_gameRecordInfo.GameProviderCode}:{gameExternalId}";
        if (!_memoryCache.TryGetValue(key, out int gameId))
        {
            gameId = await _utensil
                .GetGameIdAsync(
                    gameProviderId: gameProviderId,
                    gameExternalId: gameExternalId,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            _ = _memoryCache.Set(key, gameId);
        }
        return gameId;
    }

    public async ValueTask<Guid> GetGameProviderIdAsync(
        CancellationToken cancellationToken = default)
    {
        var key = $"{_gameRecordInfo.GameProviderCode}";
        if (!_memoryCache.TryGetValue(key, out Guid gameProviderId))
        {
            gameProviderId = await _utensil
                .GetGameProviderIdAsync(
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            _ = _memoryCache.Set(key, gameProviderId);
        }
        return gameProviderId;
    }

    public async ValueTask<PlayerInfo?> GetPlayerAsync(
        string playerCode,
        CancellationToken cancellationToken = default)
    {
        var key = $"{_gameRecordInfo.GameProviderCode}:{playerCode}";
        if (!_memoryCache.TryGetValue(key, out PlayerInfo? player))
        {
            player = await _utensil
                .GetPlayerAsync(
                    playerCode: playerCode,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            if (player == null)
            {
                return null;
            }
            _ = _memoryCache.Set(key, player, _absoluteExpirationRelativeToNow);
        }
        return player;
    }
}
