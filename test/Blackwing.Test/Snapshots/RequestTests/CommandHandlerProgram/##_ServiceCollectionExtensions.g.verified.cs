﻿//HintName: _ServiceCollectionExtensions.g.cs
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by the Blackwing.Generator source generator
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

#nullable enable

using Blackwing.Contracts.Handlers;

namespace Microsoft.Extensions.DependencyInjection;

internal static class BlackwingServiceCollectionExtensions
{
    public static IServiceCollection AddBlackwingHandlers(this IServiceCollection services)
    {
        services.AddScoped<global::Blackwing.Test.Integration.PingCommandHandler>();
        services.AddScoped<global::Blackwing.Generator.Blackwing.Test.Integration.PingCommandHandlerWrapper>();
        services.AddScoped<IRequestHandler<global::Blackwing.Test.Integration.Ping, global::Blackwing.Test.Integration.Result<global::Blackwing.Test.Integration.Pong>>, global::Blackwing.Generator.Blackwing.Test.Integration.PingCommandHandlerWrapper>(static sp => sp.GetRequiredService<global::Blackwing.Generator.Blackwing.Test.Integration.PingCommandHandlerWrapper>());

        return services;
    }
}

#nullable disable
