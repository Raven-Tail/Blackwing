using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Blackwing.Contracts;
using Blackwing.Contracts.Options;
using Blackwing.Contracts.Requests;
using Blackwing.Services;
using Microsoft.Extensions.DependencyInjection;

[assembly:BlackwingOptions(ServicesClassName = "SomeLibrary_BlackwingCollectionExtensions")]

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
        }
    }
}
