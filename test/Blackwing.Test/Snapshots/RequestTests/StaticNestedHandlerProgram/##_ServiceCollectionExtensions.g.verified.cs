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
        services.AddScoped<global::Some.Nested.Types.Program.PingHandler>();
        services.AddScoped<global::Blackwing.Generator.Some.Nested.Types.Program_PingHandlerWrapper>();
        services.AddScoped<IRequestHandler<global::Some.Nested.Types.Program.Ping, global::Some.Nested.Types.Program.Pong>, global::Blackwing.Generator.Some.Nested.Types.Program_PingHandlerWrapper>(static sp => sp.GetRequiredService<global::Blackwing.Generator.Some.Nested.Types.Program_PingHandlerWrapper>());

        return services;
    }
}

#nullable disable
