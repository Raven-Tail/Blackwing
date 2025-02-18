using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Blackwing.Contracts;
using Blackwing.Contracts.Handlers;
using Blackwing.Contracts.Pipelines;
using Blackwing.Contracts.Requests;
using Blackwing.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Blackwing.Test.Integration;

public static class Program
{
    public static async Task Main()
    {
        var services = new ServiceCollection();

        services.AddBlackwing();

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
