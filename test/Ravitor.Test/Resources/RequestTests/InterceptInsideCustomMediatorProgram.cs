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

namespace Some.Nested.Types
{
    public static class Program
    {
        public static async Task Main()
        {
            var services = new ServiceCollection();
            services.AddRavitor();

            var serviceProvider = services.BuildServiceProvider();
            var mediator = serviceProvider.GetRequiredService<IMediator>();
        }
    }

    public interface ICustomMediator : IMediator
    {
    }

    public abstract class CustomMediatorBase : ICustomMediator
    {
        public abstract ValueTask<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);

        public abstract ValueTask<TResponse> Send<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default) where TRequest : notnull, IRequest<TResponse>;
    }

    public class CustomMediator : CustomMediatorBase
    {
        private readonly Mediator mediator;

        public CustomMediator(Mediator Mediator)
        {
            mediator = Mediator;
        }

        public override ValueTask<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            return mediator.Send(request, cancellationToken);
        }

        public override ValueTask<TResponse> Send<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default)
        {
            return mediator.Send<TRequest, TResponse>(request, cancellationToken);
        }
    }
}
