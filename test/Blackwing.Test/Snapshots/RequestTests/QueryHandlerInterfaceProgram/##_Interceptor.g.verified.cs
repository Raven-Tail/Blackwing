﻿//HintName: _Interceptor.g.cs
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by the Blackwing.Generator source generator
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

#nullable enable
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Blackwing.Contracts;
using Blackwing.Contracts.Requests;
using Microsoft.Extensions.DependencyInjection;

namespace System.Runtime.CompilerServices
{
    [Conditional("DEBUG")] // not needed post-build, so can evaporate it
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    sealed file class InterceptsLocationAttribute : Attribute
    {
        public InterceptsLocationAttribute(int version, string data)
        {
            _ = version;
            _ = data;
        }
    }
}

namespace Blackwing.Interceptor
{
    static file class Interceptors
    {
        public static ValueTask<TResponse> Send_Blackwing_Test_Integration_Ping<TResponse>(this global::Blackwing.Contracts.ISender sender, IRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            var rqst = request as global::Blackwing.Test.Integration.Ping;
            var task = sender.Send<global::Blackwing.Test.Integration.Ping, global::Blackwing.Test.Integration.Pong>(rqst!, cancellationToken);
            return Unsafe.As<ValueTask<global::Blackwing.Test.Integration.Pong>, ValueTask<TResponse>>(ref task);
        }

    }
}

#nullable disable
