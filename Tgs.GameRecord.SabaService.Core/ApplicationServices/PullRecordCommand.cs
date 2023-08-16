using Tgs.GameInterface.SabaService.Protos;

namespace Tgs.GameRecord.SabaService.Core.ApplicationServices;

public record PullRecordCommand(IEnumerable<GameHistory> GameHistories);
