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

        var sender = sp.GetRequiredService<ISender>();

        var result = await sender.Send(new Ping(), default);
    }
}

// Define base convention of Command and command handler
public interface IQuery<T> : IRequest<T>
{
}

public interface IQueryHandler<TQuery, TResponse> : IRequestHandler<TQuery, TResponse>
    where TQuery : notnull, IQuery<TResponse>
{
}

// Define class from convention
public sealed record Pong;

public sealed record Ping : IQuery<Pong>;

public sealed record PingCommandHandler : IQueryHandler<Ping, Pong>
{
    public ValueTask<Pong> Handle(Ping Ping, CancellationToken cancellationToken)
    {
        var r = new Pong();
        return new(r);
    }
}
