using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Tgs.GameRecord.Saba.Common;
using Valhalla.MessageQueue;

namespace Tgs.GameRecord.Saba.Controllers;

[Route("[controller]")]
[ApiController]
public class SabaController : ControllerBase
{
	/// <summary>
	/// Games the record manual asynchronous.
	/// </summary>
	/// <param name="messageSender">The message sender.</param>
	/// <param name="source">The source.</param>
	/// <returns></returns>
	/// <remarks>
	/// <b>Example</b><br />
	///
	///     Post
	///         {
	///             "ApiUrl": "http://i1y2tsa.bw6688.com/",
	///             "OperatorId": "Pega",
	///             "VendorId": "lmaz5farkr",
	///             "StartTime": "2023-08-24T18:15:00+08:00",
	///             "EndTime": "2023-08-24T19:30:00+08:00"
	///         }
	/// </remarks>
	[HttpPost("manual")]
	public ValueTask GameRecordManualAsync(
		[FromServices] IMessageSender messageSender,
		SabaGameRecordManualMessage source)
		=> messageSender.PublishAsync(
			subject: SabaNatsConst.ManualPublish,
			data: Encoding.UTF8.GetBytes(JsonSerializer.Serialize(source)),
			cancellationToken: HttpContext.RequestAborted);

	/// <summary>
	/// Games the record schedule asynchronous.
	/// </summary>
	/// <param name="messageSender">The message sender.</param>
	/// <param name="source">The source.</param>
	/// <returns></returns>
	/// <remarks>
	/// <b>Example</b><br />
	///
	///     Post
	///         {
	///             "ApiUrl": "http://i1y2tsa.bw6688.com/",
	///             "OperatorId": "Pega",
	///             "VendorId": "lmaz5farkr"
	///         }
	/// </remarks>
	[HttpPost("schedule")]
	public ValueTask GameRecordScheduleAsync(
		[FromServices] IMessageSender messageSender,
		SabaGameRecordScheduleMessage source)
		=> messageSender.PublishAsync(
			subject: SabaNatsConst.SchedulePublish,
			data: Encoding.UTF8.GetBytes(JsonSerializer.Serialize(source)),
			cancellationToken: HttpContext.RequestAborted);
}
