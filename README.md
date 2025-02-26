# Blackwing

[![Build Action](https://github.com/Raven-Tail/Blackwing/actions/workflows/build.yaml/badge.svg)](https://github.com/Raven-Tail/Blackwing/actions/workflows/build.yaml)
[![Publish Action](https://github.com/Raven-Tail/Blackwing/actions/workflows/publish.yaml/badge.svg)](https://github.com/Raven-Tail/Blackwing/actions/workflows/publish.yaml)
[![License](https://img.shields.io/github/license/raven-tail/blackwing?style=flat-square)](https://github.com/raven-tail/blackwing/blob/main/LICENSE)
[![.NET 8](https://img.shields.io/badge/.NET%208-%23512bd4?style=flat)](https://dotnet.microsoft.com/)
[![.NET 9](https://img.shields.io/badge/.NET%209-%23512bd4?style=flat)](https://dotnet.microsoft.com/)
[![Downloads](https://img.shields.io/nuget/dt/blackwing.contracts?style=flat-square)](https://www.nuget.org/packages/Blackwing.Contracts/)

Blackwing is the name of your dependable Raven that delivers your requests to the correct handler and returns with his response.

The library allows you to use the [mediator pattern](https://en.wikipedia.org/wiki/Mediator_pattern) while staying really minimal as it is a source generator that works only on the project you have it installed in combination with the contracts package.

Things the generator will do for you:
- Generate wrapper handlers that execute your handler inside a pipeline.
- Generate a DI registration method for all of the handlers generated in the current project.
- Generate interceptors to intercept the sending of your commands and execute the efficient `Send` method of the `IMediator` interface allowing you to pass your request without specifying all of the generics.

Things the interceptor can't do:
- Can't intercept methods that pass generic/abstract/interface requests as an argument to the `Send` method. Only concrete classes are supported. if this is breaking for you there is a fallback that can execute requests using reflection (not recommended) or you can use the fast method yourself which requires all of the generic arguments (preferred).

The goal is to be as minimal and straightforward as possible to give the best of both worlds. The library only generates `Handlers`, `Interceptors` and the `Registration` code.

## Packages

>[!Warning]
> .NET 9 SDK Required for the preview feature of [interceptors](https://github.com/dotnet/roslyn/blob/main/docs/features/interceptors.md).

| Package | Stable | Pre Release |
| --      |--|--|
| Blackwing.Contracts | [![Contracts](https://img.shields.io/nuget/v/Blackwing.Contracts)](https://www.nuget.org/packages/Blackwing.Contracts) | [![Contracts](https://img.shields.io/nuget/vpre/Blackwing.Contracts)](https://www.nuget.org/packages/Blackwing.Contracts) |
| Blackwing.Generator | [![Generator](https://img.shields.io/nuget/v/Blackwing.Generator)](https://www.nuget.org/packages/Blackwing.Generator) | [![Generator](https://img.shields.io/nuget/vpre/Blackwing.Generator)](https://www.nuget.org/packages/Blackwing.Generator) |
| Blackwing.Extensions.DependencyInjection | [![Dependency Injection Extensions](https://img.shields.io/nuget/v/Blackwing.Extensions.DependencyInjection)](https://www.nuget.org/packages/Blackwing.Extensions.DependencyInjection) | [![Dependency Injection Extensions](https://img.shields.io/nuget/vpre/Blackwing.Extensions.DependencyInjection)](https://www.nuget.org/packages/Blackwing.Extensions.DependencyInjection) |

# Usage

Description
- Blackwing.Contracts
    - Contains the `options`, `message`, `handler`, `pipeline` and `mediator` types/interfaces respective to their domain folder.
    - The root of the project contains the `sender`/`mediator` interfaces.
    - The `Options` folder contains the configuration attribute that configures the source generator.
    - The `Requests` folder contains all of the `message`, `handler` and `pipeline` definitions for the `IRequest` concept.
- Blackwing.Generator
    - Source Generator to generate `handlers`, `registration` and `interception`.
- Blackwing.Extensions.DependencyInjection 
    - Default `IMediator` & `ISender` implementations for the `Microsoft.Extensions.DependencyInjection` packages.

Installation:
- `Blackwing.Contracts` in projects that you want to store you message objects.
- `Blackwing.Generator` in projects that implement the message handlers and in projects that will use the `IMediator` to send messages (so that the interception is generated).
- `Blackwing.Extensions.DependencyInjection` in the project that brings all your shared code together. This is usually the edge/outermost project (ASP.NET Core, Worker, Console etc.)

## Configuration

The generator is configured through an `assembly` attribute eg:
```C#
[assembly:BlackwingOptions(ServicesClassPublic = true, ServicesClassName = "MyTestHandlers")]
```
The available options are:
- `HandlersClassPublic` (`bool`): Allows you to define if the handler wrapper class should be public or internal (defaults to internal).
- `ServicesNamespace` (`string`): Allows you to define the namespace of the service collection extension class (defaults to Microsoft.Extensions.DependencyInjection).
- `ServicesClassPublic` (`bool`): Allows you to define if the service collection extension class should be public or internal (defaults to internal).
- `ServicesClassName` (`string`): Allows you to define the service collection extension class name (defaults to BlackwingServiceCollectionExtensions).
- `ServicesMethodName` (`string`): Allows you to define the service collection extension method name (defaults to AddBlackwingHandlers).
- `DisableInterceptor` (`bool`): Allows you to disable the interceptors generation (defaults to false).

> [!TIP]
> You can also disable the interceptors generation by setting the `DisableBlackwingInterceptor` MSBuild Property to `true`.

# Getting Started

The section will describe how to get started with Blackwing using the [Console](./sample/Console/) sample project as the example.

> [!TIP]
> The [sample](./sample/) folder also contains more complex samples.

1. Add all three packages
```console
dotnet add package Blackwing.Contracts
dotnet add package Blackwing.Generator
dotnet add package Blackwing.Extensions.DependencyInjection
```
2. Create `IRequest<out TResponse>` and `Response` types
```csharp
public sealed record Ping(Guid Id) : IRequest<Pong>;

public sealed record Pong(Guid Id);
```
3. Implement the `IRequestHandler<TRequest, TResponse>` interface to handle the request.
```csharp
public sealed class PingHandler : IRequestHandler<Ping, Pong>
{
    public ValueTask<Pong> Handle(Ping request, CancellationToken cancellationToken)
    {
        Console.WriteLine("4) Returning pong!");
        return new ValueTask<Pong>(new Pong(request.Id));
    }
}
```
4. Optionally implement some pipeline behaviors.
```csharp
public sealed class GenericLoggerHandler<TRequest, TResponse> : IRequestPipeline<TRequest, TResponse> where TRequest : IRequest
{
    public async ValueTask<TResponse> Handle(TRequest message, IRequestPipelineDelegate<TRequest, TResponse> next, CancellationToken cancellationToken)
    {
        Console.WriteLine("1) Running logger handler");
        try
        {
            var response = await next(message, cancellationToken);
            Console.WriteLine("5) No error!");
            return response;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw;
        }
    }
}

public sealed class PingValidator : IRequestPipeline<Ping, Pong>
{
    public ValueTask<Pong> Handle(Ping request, IRequestPipelineDelegate<Ping, Pong> next, CancellationToken cancellationToken)
    {
        Console.WriteLine("2) Running ping validator");
        if (request is null || request.Id == default)
            throw new ArgumentException("Invalid input");
        else
            Console.WriteLine("3) Valid input!");

        return next(request, cancellationToken);
    }
}
```
5. Register the default mediator implementation to the service collection.
```csharp
services.AddBlackwing();
```
6. Register the generated handlers to the service collection.
```csharp
services.AddBlackwingHandlers();
```
7. Send a message using the `IMediator`, `ISender` or `Mediator` types.
```csharp
var mediator = serviceProvider.GetRequiredService<IMediator>();

var id = Guid.NewGuid();
var request = new Ping(id);
var response = await mediator.Send(request);
```

# Contributing

For general contribution information you can read the [Raven Tail Contributing document](https://github.com/Raven-Tail/.github/blob/main/CONTRIBUTING.md).

## Local Development

To develop you need:
1. dotnet 9.0 SDK
2. Visual Studio or VS Code with the C# extension.
3. Configured your IDE for the [TUnit](https://thomhurst.github.io/TUnit/) library.

### Project Structure

The project is structured around the idea of feature folders. For example in contracts the root contains the `IMediator` and `ISender` interfaces as they are the main feature and the requests folder contains all the requests related interfaces. This is true throughout the project as seen in the generator too where the Requests related generator exists in the Generators/Requests folder.

### Executing and Testing your code

As you are developing a new feature or a bug fix you need to test and execute your code and for this you need to get familiar with the tests because this is the easier way of testing the generator output and help in verifying your work.

The tests are split in 3 projects
- `Blackwing.Test`: Base project testing generator output using the [Verify](https://github.com/VerifyTests/Verify) library. Using the `TestBase` base class you only need to call the `Verify()` method and it will search for a file in resources. The convention is this `Resources/{Test Class Name}/{Test Method Name}.cs` and the verify snapshots will be created at the `Snapshots/{Test Class Name}/{Test Method Name}/` directory. The purpose of this is to make sure the generator will always generate the same output for the same input text. For new texts this output will need to be verified as described in the verify library.
- `Blackwing.Test.Integration`: Contains tests that use the generator to execute and test it's generated code.
- `Blackwing.Test.NugetIntegration`: Contains everything from the `Integrations project` but with a custom script will publish the generator locally as a nuget package to execute all the integration tests again ensuring nuget packages still work.

# Credits
This project was inspired by the [Mediator](https://github.com/martinothamar/Mediator) project and it got boostraped using some of it's samples and test cases. I would like to express my gratitude to all the contributors at that project as it helped me shape this project.

Additionally, the project wouldn't be as it is without the excellent [articles](https://andrewlock.net/series/creating-a-source-generator/) by [Andrew Lock](https://github.com/andrewlock) on source generators, which helped a lot especially on testing with Verify.
