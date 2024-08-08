using System.Text;
using System.Text.Json;
using Tgs.GameRecord.Saba.Common;
using Tgs.GameRecord.Saba.Core;
using Tgs.GameRecord.Saba.Core.ApplicationServices;
using Valhalla.MessageQueue;

namespace Tgs.GameRecord.Saba.Messages;

public class ManualMessageHandler(
	ILogger<ManualMessageHandler> logger,
	IManualPullRecordDispatcher dispatcher) : IMessageHandler<ReadOnlyMemory<byte>>
{
	public ValueTask HandleAsync(string subject, ReadOnlyMemory<byte> data, IEnumerable<MessageHeaderValue>? headerValues, CancellationToken cancellationToken = default)
	{
		var json = Encoding.UTF8.GetString(data.Span);
		var message = JsonSerializer.Deserialize<SabaGameRecordManualMessage>(json)
			?? throw new NullReferenceException("The request data resolves to a null value");

		logger.LogInformation("{logInformation}",
			JsonSerializer.Serialize(new
			{
				LogOn = DateTime.Now.ToString("u"),
				Subject = subject,
				Data = new
				{
					message.ApiUrl,
					message.OperatorId,
					message.VendorId,
					StartTime = message.StartTime.ToString("u"),
					EndTime = message.EndTime.ToString("u"),
				},
				Message = "This receive message handler."
			}));

		return dispatcher.PushCommandAsync(
			command: new ManualPullRecordCommand(
				ApiUrl: message.ApiUrl,
				OperatorId: message.OperatorId,
				VendorId: message.VendorId,
				StartTime: message.StartTime,
				EndTime: message.EndTime),
			cancellationToken: cancellationToken);
	}
}
