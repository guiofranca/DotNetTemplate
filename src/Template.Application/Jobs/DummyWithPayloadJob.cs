using Coravel.Queuing.Interfaces;
using Microsoft.Extensions.Logging;
using Template.Application.Jobs.shared;

namespace Template.Application.Jobs;

public class DummyWithPayloadJob : BaseJobWithPayload<DummyPayload>
{
    public DummyWithPayloadJob(IQueue queue, ILogger<DummyJob> logger) : base(queue, logger)
    {

    }

    public override async Task Invoke()
    {
        _logger.LogDebug($"Task Started! {Payload.Id}, {Payload.Name}");
        await Task.Delay(5000);
        _logger.LogDebug($"Task Finished! {Payload.Id}, {Payload.Name}");
    }
}
