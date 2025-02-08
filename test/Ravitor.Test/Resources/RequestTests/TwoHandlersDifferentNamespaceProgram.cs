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
using Ravitor.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Some.Nested.Types.One
{
    public static class Program
    {
        public static async Task Main()
        {
            var services = new ServiceCollection();

            services.AddRavitor();

            var serviceProvider = services.BuildServiceProvider();

            var mediator = serviceProvider.GetRequiredService<IMediator>();

            _ = await mediator.Send(new Ping(Guid.NewGuid()));
        }
    }

    public sealed record Ping(Guid Id) : IRequest<byte[]>;

    public sealed class PingHandler : IRequestHandler<Ping, byte[]>
    {
        public ValueTask<byte[]> Handle(Ping request, CancellationToken cancellationToken)
        {
            var bytes = request.Id.ToByteArray();
            return new ValueTask<byte[]>(bytes);
        }
    }
}

namespace Some.Nested.Types.Two
{
    public sealed record Ping(Guid Id) : IRequest<byte[]>;

    public sealed class PingHandler : IRequestHandler<Ping, byte[]>
    {
        public ValueTask<byte[]> Handle(Ping request, CancellationToken cancellationToken)
        {
            var bytes = request.Id.ToByteArray();
            return new ValueTask<byte[]>(bytes);
        }
    }
}
