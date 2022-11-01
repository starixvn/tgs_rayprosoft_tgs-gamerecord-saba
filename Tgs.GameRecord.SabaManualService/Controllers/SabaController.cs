using Microsoft.AspNetCore.Mvc;
using Tgs.GameRecord.SabaManualService.ViewModels;
using Tgs.GameRecord.SabaService.Core.WorkerServices;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.SabaManualService.Controllers;

[ApiController]
[Route("[controller]")]
public class SabaController : ControllerBase
{
    /// <summary>
    /// Games the history by sports book asynchronous.
    /// </summary>
    /// <param name="service">The service.</param>
    /// <param name="model">The model.</param>
    /// <returns></returns>
    /// <remarks>
    /// <b>Example</b><br />
    ///
    ///     Post
    ///
    ///         {
    ///             "apiUrl": "http://h0i2tsa.bw6688.com/",
    ///             "operatorId": "tg",
    ///             "vendorId": "um5vspnzkx",
    ///             "startTime": "2022-10-27T15:50:00+08:00",
    ///             "endTime": "2022-10-27T15:55:00+08:00
    ///         }
    /// </remarks>
    [HttpPost]
    public ValueTask GameHistoryBySportsBookAsync(
                [FromServices] ICommandService<ManualWorkerCommand> service,
                GameRecordQueryViewModel model)
                => service.ExecuteAsync(new ManualWorkerCommand(
                    ApiUrl: model.ApiUrl,
                    OperatorId: model.OperatorId,
                    VendorId: model.VendorId,
                    StartTime: model.StartTime,
                    EndTime: model.EndTime));
}
