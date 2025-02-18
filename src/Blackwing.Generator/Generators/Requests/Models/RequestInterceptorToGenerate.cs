using Microsoft.CodeAnalysis.CSharp;

namespace Blackwing.Generator.Generators.Requests.Models;

#pragma warning disable RSEXPERIMENTAL002 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

internal readonly record struct RequestInterceptorToGenerate
{
    public readonly InterceptableLocation Location;
    public readonly string Sender;
    public readonly string Request;
    public readonly string Response;

    public RequestInterceptorToGenerate(InterceptableLocation location, string sender, string request, string response)
    {
        Location = location;
        Sender = sender;
        Request = request;
        Response = response;
    }
}
