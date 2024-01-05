using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Tgs.GameRecord.Saba.Common;
using Tgs.ServiceUtility;
using Valhalla.MessageQueue;

namespace Tgs.GameRecord.Saba.Core.JobApplicationServices;

internal class JobExecuteCommandService(
    ILogger<JobExecuteCommandService> logger,
    IMessageSender messageSender) : ICommandService<JobExecuteCommand>
{
    public ValueTask ExecuteAsync(JobExecuteCommand command, CancellationToken cancellationToken = default)
    {
        var parameters = new SabaGameRecordScheduleMessage
        {
            ApiUrl = command.ApiUrl,
            OperatorId = command.OperatorId,
            VendorId = command.VendorId
        };

        logger.LogDebug("LogOn:{logOn} Parameters:{parameters}", DateTime.Now.ToString("u"), parameters);

        return messageSender
            .PublishAsync(
            subject: SabaNatsConst.SchedulePublish,
            data: Encoding.UTF8.GetBytes(JsonSerializer.Serialize(parameters)),
            cancellationToken: cancellationToken);
    }
}
