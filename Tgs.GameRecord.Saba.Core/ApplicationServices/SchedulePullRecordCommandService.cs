using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tgs.GameInterface.SabaService.Client;
using Tgs.GameInterface.SabaService.Protos;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.Saba.Core.ApplicationServices;

internal class SchedulePullRecordCommandService(
	ILogger<SchedulePullRecordCommandService> logger,
	IOptions<GameRecordInfoOptions> options,
	IGameRecordScheduleRepository scheduleRepository,
	IGameInterfaceSabaClient gameInterfaceSabaClient,
	ICommandService<HandleRecordCommand> service) : ICommandService<SchedulePullRecordCommand>
{
	private readonly GameRecordInfoOptions _gameRecordInfo = options.Value;

	public async ValueTask ExecuteAsync(SchedulePullRecordCommand command, CancellationToken cancellationToken = default)
	{
		var gameRecordSchedule = await scheduleRepository.GetAsync(
			operatorId: command.OperatorId,
			vendorId: command.VendorId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false) ??
			await scheduleRepository.AddAsync(
				operatorId: command.OperatorId,
				vendorId: command.VendorId,
				cancellationToken: cancellationToken)
			.ConfigureAwait(false)
			?? throw new Exception($"Failed to get Game Record Schedule data.({command.OperatorId})");

		var versionKey = gameRecordSchedule.VersionKey;

		if (versionKey < _gameRecordInfo.VersionKey)
		{
			versionKey = _gameRecordInfo.VersionKey;
		}

		logger.LogInformation("{command.OperatorId}取得VersionKey:{versionKey}", command.OperatorId, versionKey);

		long tempVersionKey;
		do
		{
			tempVersionKey = versionKey;
			var reply = await gameInterfaceSabaClient.GameHistoryAsync(
				apiUrl: command.ApiUrl,
				operatorId: command.OperatorId,
				vendorId: command.VendorId,
				versionKey: tempVersionKey,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			var gameHistories = reply.Items ?? Enumerable.Empty<GameHistory>();

			if (gameHistories.Any())
			{
				foreach (var gameHistory in gameHistories)
				{
					try
					{
						await service.ExecuteAsync(
							command: new HandleRecordCommand(gameHistory),
							cancellationToken: cancellationToken)
							.ConfigureAwait(false);
					}
					catch (Exception ex)
					{
						logger.LogError(ex, "遊戲紀錄處理發生例外.({history})", gameHistory);
					}
				}
			}

			versionKey = reply.LastVersionKey;
		} while (tempVersionKey != versionKey);

		await scheduleRepository.UpdateAsync(
			operatorId: command.OperatorId,
			vendorId: command.VendorId,
			versionKey: versionKey,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}
}
