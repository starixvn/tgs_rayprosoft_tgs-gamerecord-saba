using System.Text;
using System.Text.Json;
using Tgs.GameRecord.Saba.Common;
using Tgs.GameRecord.Saba.Core.ApplicationServices;
using Tgs.ServiceUtility;
using Valhalla.MessageQueue;

namespace Tgs.GameRecord.Saba.Messages;

public class GameRecordManualMessageHandler(
    ILogger<GameRecordManualMessageHandler> logger,
    ICommandService<ManualPullRecordCommand> service) : IMessageHandler
{
    public ValueTask HandleAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default)
    {
        var json = Encoding.UTF8.GetString(data.Span);
        var message = JsonSerializer.Deserialize<SabaGameRecordManualMessage>(json)
            ?? throw new NullReferenceException("The request data resolves to a null value");

        logger.LogInformation("LogOn:{logOn} Information Request:{message}", DateTime.Now.ToString("o"), message);

        var command = new ManualPullRecordCommand(
            ApiUrl: message.ApiUrl,
            OperatorId: message.OperatorId,
            VendorId: message.VendorId,
            StartTime: message.StartTime,
            EndTime: message.EndTime);

        return service.ExecuteAsync(
            command: command,
            cancellationToken: cancellationToken);
    }
}
