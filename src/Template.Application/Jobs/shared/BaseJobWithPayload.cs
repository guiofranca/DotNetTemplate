using Coravel.Invocable;
using Coravel.Queuing.Interfaces;
using Microsoft.Extensions.Logging;

namespace Template.Application.Jobs.shared;

public abstract class BaseJobWithPayload<T> : IInvocable, IInvocableWithPayload<T> where T : class
{
    protected readonly IQueue _queue;
    protected readonly ILogger _logger;
    public required T Payload { get; set; }

    protected BaseJobWithPayload(IQueue queue, ILogger logger)
    {
        _queue = queue;
        _logger = logger;
    }

    public abstract Task Invoke();
}
