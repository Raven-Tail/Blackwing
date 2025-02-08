using Ravitor.Generator.Models;
using System.Text;

namespace Ravitor.Generator.Generators;

public static class HandlerGenerator
{
    public static (string HintName, string Content) Generate(in HandlerToGenerate handlerToGenerate)
    {
        var handler = handlerToGenerate.RequestHandler;
        var wrapperNamespace = handlerToGenerate.WrapperNamespace;
        var wrapper = handlerToGenerate.WrapperHandler;
        var request = handlerToGenerate.Request;
        var response = handlerToGenerate.Response;

        // todo: Allow to define if handlers should be public or internal (internal default).

        var content = $$"""
            {{Constants.Header}}

            using Ravitor.Contracts.Handlers;
            using Ravitor.Contracts.Pipelines;
            using Microsoft.Extensions.DependencyInjection;

            namespace {{wrapperNamespace}};

            internal sealed class {{wrapper}} : {{Constants.Handlers.IRequestHandler}}<{{request}}, {{response}}>
            {
                private readonly {{Constants.Pipelines.IRequestPipelineDelegate}}<{{request}}, {{response}}> handler;

                public {{wrapper}}(
                    {{handler}} handler,
                    IEnumerable<{{Constants.Pipelines.IRequestPipeline}}<{{request}}, {{response}}>> behaviors)
                {
                    var finalHandler = ({{Constants.Pipelines.IRequestPipelineDelegate}}<{{request}}, {{response}}>)handler.Handle;
                    foreach (var pipeline in behaviors.Reverse())
                    {
                        var handlerCopy = finalHandler;
                        var pipelineCopy = pipeline;
                        finalHandler = ({{request}} request, CancellationToken cancellationToken) => pipelineCopy.Handle(request, handlerCopy, cancellationToken);
                    }
                    this.handler = finalHandler;
                }

                public ValueTask<{{response}}> Handle({{request}} request, CancellationToken cancellationToken = default)
                {
                    return handler(request, cancellationToken);
                }
            }

            {{Constants.Footer}}

            """;

        var nameSb = new StringBuilder(handler);
        nameSb.Replace("global::", "");
        nameSb.Replace('.', '_');

        return ($"{nameSb}.g.cs", content);
    }
}
