using Ravitor.Contracts.Handlers;

namespace AspNetCore.Abstract;

public interface IQueryHandler<TQuery, TResponse> : IRequestHandler<TQuery, TResponse>
    where TQuery : notnull, IQuery<TResponse>
{
}
