using Coravel.Invocable;
using Coravel.Queuing.Interfaces;
using Microsoft.Extensions.Logging;

namespace Template.Application.Jobs.shared;

public abstract class BaseJob : IInvocable
{
    protected readonly IQueue _queue;
    protected readonly ILogger _logger;

    protected BaseJob(IQueue queue, ILogger logger)
    {
        _queue = queue;
        _logger = logger;
    }

    public abstract Task Invoke();
}
