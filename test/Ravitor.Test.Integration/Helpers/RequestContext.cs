namespace Ravitor.Test.Integration.Helpers;

public sealed class RequestContext
{
    public bool SkipHandler { get; set; }

    public Stack<string> CallStack { get; } = [];
}
