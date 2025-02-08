namespace Ravitor.Contracts.Requests;

public interface IRequest
{
}

public interface IRequest<out TResponse> : IRequest
{
}
