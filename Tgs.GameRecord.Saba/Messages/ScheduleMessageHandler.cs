using System.Text;
using System.Text.Json;
using Tgs.GameRecord.Saba.Common;
using Tgs.GameRecord.Saba.Core;
using Tgs.GameRecord.Saba.Core.ApplicationServices;
using Valhalla.MessageQueue;

namespace Tgs.GameRecord.Saba.Messages;

public class ScheduleMessageHandler(
	ILogger<ScheduleMessageHandler> logger,
	ISchedulePullRecordDispatcher dispatcher) : IMessageHandler<ReadOnlyMemory<byte>>
{
	public ValueTask HandleAsync(string subject, ReadOnlyMemory<byte> data, IEnumerable<MessageHeaderValue>? headerValues, CancellationToken cancellationToken = default)
	{
		var json = Encoding.UTF8.GetString(data.Span);
		var message = JsonSerializer.Deserialize<SabaGameRecordScheduleMessage>(json)
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
					message.VendorId
				},
				Message = "This receive message handler."
			}));

		return dispatcher.PushCommandAsync(
			command: new SchedulePullRecordCommand(
				ApiUrl: message.ApiUrl,
				OperatorId: message.OperatorId,
				VendorId: message.VendorId),
			cancellationToken: cancellationToken);
	}
}
