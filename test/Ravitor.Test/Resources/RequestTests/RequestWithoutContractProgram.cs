using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Ravitor.Contracts;
using Ravitor.Contracts.Handlers;
using Ravitor.Contracts.Pipelines;
using Ravitor.Contracts.Requests;
using Ravitor.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Ravitor.Test.Integration;

public static class Program
{
    public static async Task Main()
    {
        var services = new ServiceCollection();

        services.AddRavitor();

        var sp = services.BuildServiceProvider();

        var sender = sp.GetRequiredService<Mediator>();

        var result = await sender.Send(new InsideClass.Ping());
    }
}

public class InsideClass
{
    public sealed class Ping;

    public sealed class Pong;

    public sealed class PingHandler : IRequestHandler<Ping, Pong>
    {
        public ValueTask<Pong> Handle(Ping request, CancellationToken cancellationToken)
        {
            var Pong = new Pong();
            return new(Pong);
        }
    }
}
