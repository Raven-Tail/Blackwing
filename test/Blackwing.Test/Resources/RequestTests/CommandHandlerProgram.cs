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

        var sender = sp.GetRequiredService<ISender>();

        var result = await sender.Send(new Ping(), default);
    }
}

// Define base convention of Command and command handler
public sealed class Result<TResult>;

public abstract record Command<TResult> : IRequest<Result<TResult>>
    where TResult : notnull;

public abstract class CommandHandler<TCommand, TResult> :
    IRequestHandler<TCommand, Result<TResult>>
    where TCommand : Command<TResult>
    where TResult : notnull
{
    public abstract ValueTask<Result<TResult>> Handle(TCommand command, CancellationToken cancellationToken);
}

// Define class from convention
public sealed record Pong;

public sealed record Ping : Command<Pong>;

public sealed class PingCommandHandler : CommandHandler<Ping, Pong>
{
    public override ValueTask<Result<Pong>> Handle(Ping Ping, CancellationToken cancellationToken)
    {
        var r = new Result<Pong>();
        return new(r);
    }
}
