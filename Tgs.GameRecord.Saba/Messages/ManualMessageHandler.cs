using System.Text;
using System.Text.Json;
using Tgs.GameRecord.Saba.Core.WorkerServices;
using Tgs.ServiceUtility;
using Valhalla.MessageQueue;

namespace Tgs.GameRecord.Saba.Messages;

public class ManualMessageHandler : IMessageProcessor
{
    private readonly ICommandService<ManualWorkerCommand> _service;

    public ManualMessageHandler(ICommandService<ManualWorkerCommand> service)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
    }

    public async ValueTask<ReadOnlyMemory<byte>> HandleAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default)
    {
        var json = Encoding.UTF8.GetString(data.Span);
        var request = JsonSerializer.Deserialize<ManualMessageRequest>(json)!;

        var command = new ManualWorkerCommand(
            ApiUrl: request.ApiUrl,
            OperatorId: request.OperatorId,
            VendorId: request.VendorId,
            StartTime: request.StartTime,
            EndTime: request.EndTime);

        await _service
            .ExecuteAsync(
                command: command,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var response = new ManualMessageResponse(true);
        return Encoding.UTF8.GetBytes(JsonSerializer.Serialize(response));
    }
}
