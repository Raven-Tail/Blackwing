﻿//HintName: _ServiceCollectionExtensions.g.cs
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by the Ravitor.Generator source generator
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

#nullable enable

using Ravitor.Contracts.Handlers;
using Microsoft.Extensions.DependencyInjection;

namespace Ravitor.Generator;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRavitorHandlers(this IServiceCollection services)
    {
        services.AddScoped<global::PingHandler>();
        services.AddScoped<global::Ravitor.Generator.PingHandlerWrapper>();
        services.AddScoped<IRequestHandler<global::Ping, global::Pong>, global::Ravitor.Generator.PingHandlerWrapper>(static sp => sp.GetRequiredService<global::Ravitor.Generator.PingHandlerWrapper>());

        return services;
    }
}

#nullable disable
