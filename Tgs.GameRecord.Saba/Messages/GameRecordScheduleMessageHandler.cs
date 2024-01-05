using System.Text;
using System.Text.Json;
using Tgs.GameRecord.Saba.Common;
using Tgs.GameRecord.Saba.Core.ApplicationServices;
using Tgs.ServiceUtility;
using Valhalla.MessageQueue;

namespace Tgs.GameRecord.Saba.Messages;

public class GameRecordScheduleMessageHandler(
    ILogger<GameRecordScheduleMessageHandler> logger,
    ICommandService<SchedulePullRecordCommand> service) : IMessageHandler
{
    public ValueTask HandleAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default)
    {
        var json = Encoding.UTF8.GetString(data.Span);
        var message = JsonSerializer.Deserialize<SabaGameRecordScheduleMessage>(json)
            ?? throw new NullReferenceException("The request data resolves to a null value");

        logger.LogInformation("LogOn:{logOn} Information Request:{message}", DateTime.Now.ToString("o"), message);

        var command = new SchedulePullRecordCommand(
            ApiUrl: message.ApiUrl,
            OperatorId: message.OperatorId,
            VendorId: message.VendorId);

        return service.ExecuteAsync(
            command: command,
            cancellationToken: cancellationToken);
    }
}
