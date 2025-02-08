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
        services.AddScoped<global::Some.Nested.Types.One.PingHandler>();
        services.AddScoped<global::Ravitor.Generator.Some.Nested.Types.One.PingHandlerWrapper>();
        services.AddScoped<IRequestHandler<global::Some.Nested.Types.One.Ping, byte[]>, global::Ravitor.Generator.Some.Nested.Types.One.PingHandlerWrapper>(static sp => sp.GetRequiredService<global::Ravitor.Generator.Some.Nested.Types.One.PingHandlerWrapper>());

        services.AddScoped<global::Some.Nested.Types.Two.PingHandler>();
        services.AddScoped<global::Ravitor.Generator.Some.Nested.Types.Two.PingHandlerWrapper>();
        services.AddScoped<IRequestHandler<global::Some.Nested.Types.Two.Ping, byte[]>, global::Ravitor.Generator.Some.Nested.Types.Two.PingHandlerWrapper>(static sp => sp.GetRequiredService<global::Ravitor.Generator.Some.Nested.Types.Two.PingHandlerWrapper>());

        return services;
    }
}

#nullable disable
