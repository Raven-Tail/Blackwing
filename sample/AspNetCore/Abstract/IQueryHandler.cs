using Blackwing.Contracts.Requests;

namespace AspNetCore.Abstract;

public interface IQueryHandler<TQuery, TResponse> : IRequestHandler<TQuery, TResponse>
    where TQuery : notnull, IQuery<TResponse>
{
}
