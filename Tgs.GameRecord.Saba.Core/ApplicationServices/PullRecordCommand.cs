using Tgs.GameInterface.SabaService.Protos;

namespace Tgs.GameRecord.Saba.Core.ApplicationServices;

public record PullRecordCommand(IEnumerable<GameHistory> GameHistories);
