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

using Blackwing.Contracts.Requests;

namespace Microsoft.Extensions.DependencyInjection;

internal static class BlackwingServiceCollectionExtensions
{
    public static IServiceCollection AddBlackwingHandlers(this IServiceCollection services)
    {
        services.AddScoped<global::PingHandler>();
        services.AddScoped<global::Blackwing.Generator.PingHandlerWrapper>();
        services.AddScoped<IRequestHandler<global::Ping, global::Pong>, global::Blackwing.Generator.PingHandlerWrapper>(static sp => sp.GetRequiredService<global::Blackwing.Generator.PingHandlerWrapper>());

        return services;
    }
}

#nullable disable
