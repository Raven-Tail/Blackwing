﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Blackwing.Contracts;
using Blackwing.Contracts.Options;
using Blackwing.Contracts.Requests;
using Blackwing.Services;
using Microsoft.Extensions.DependencyInjection;

[assembly: BlackwingOptions(DisableInterceptor = true)]

namespace Some.Nested.Types
{
    public static class Program
    {
        public static async Task Main()
        {
            var services = new ServiceCollection();
            services.AddBlackwing();

            var serviceProvider = services.BuildServiceProvider();
            var mediator = serviceProvider.GetRequiredService<IMediator>();

            var id = Guid.NewGuid();
            var request = new Ping(id);

            _ = await mediator.Send(request);
        }
    }
}

//
// Types
//

public sealed record Ping(Guid Id) : IRequest<Pong>;

public sealed record Pong(Guid Id);

public sealed class PingHandler : IRequestHandler<Ping, Pong>
{
    public ValueTask<Pong> Handle(Ping request, CancellationToken cancellationToken)
    {
        return new ValueTask<Pong>(new Pong(request.Id));
    }
}