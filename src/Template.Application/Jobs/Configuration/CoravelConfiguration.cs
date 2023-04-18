using Coravel;
using Coravel.Queuing.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Template.Application.Jobs.Configuration;

public static class CoravelConfiguration
{
    public static void AddWorkerQueue(this IServiceCollection services)
    {
        services.AddQueue();
        services.AddTransient<DummyJob>();
        services.AddTransient<DummyWithPayloadJob>();
    }
}
