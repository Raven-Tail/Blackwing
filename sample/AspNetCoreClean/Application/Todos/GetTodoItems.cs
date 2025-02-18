using AspNetCoreSample.Domain;
using Ravitor.Contracts.Handlers;
using Ravitor.Contracts.Requests;

namespace AspNetCoreSample.Application;

public sealed record GetTodoItems() : IRequest<IEnumerable<TodoItem>>;

public sealed class TodoItemQueryHandler : IRequestHandler<GetTodoItems, IEnumerable<TodoItem>>
{
    private readonly ITodoItemRepository _repository;

    public TodoItemQueryHandler(ITodoItemRepository repository)
    {
        _repository = repository;
    }

    public ValueTask<IEnumerable<TodoItem>> Handle(GetTodoItems query, CancellationToken cancellationToken)
    {
        return _repository.GetItems(cancellationToken);
    }
}
