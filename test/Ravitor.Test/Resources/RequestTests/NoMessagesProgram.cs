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
}
