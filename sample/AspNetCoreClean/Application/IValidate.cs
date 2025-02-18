using Blackwing.Contracts.Requests;
using System.Diagnostics.CodeAnalysis;

namespace AspNetCoreSample.Application;

public interface IValidate : IRequest
{
    bool IsValid([NotNullWhen(false)] out ValidationError? error);
}
