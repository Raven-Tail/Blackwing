using System.Threading.Tasks;

namespace Blackwing.Test;

public sealed class RequestTests : TestBase
{
    [Test]
    public Task AbstractHandlerProgram()
    {
        return Verify();
    }

    [Test]
    public Task ByteArrayResponseProgram()
    {
        return Verify();
    }

    [Test]
    public Task CommandHandlerProgram()
    {
        return Verify();
    }

    [Test]
    public Task DeepNamespaceProgram()
    {
        return Verify();
    }

    [Test, Skip("Needs fixing to emit warning diagnostic that there are two handlers handling the same request.")]
    public Task DuplicateHandlersProgram()
    {
        // todo: Fix by introducing a warning that there are two handlers handling the same request (but do generate and add both handlers).
        return Verify();
    }

    [Test]
    public Task InsideClassProgram()
    {
        return Verify();
    }

    [Test]
    public Task InterceptInsideCustomMediatorProgram()
    {
        return Verify();
    }

    [Test, Skip("Needs fixing to emit warning diagnostic that there are more than two handlers handling the same request.")]
    public Task MultipleErrorsProgram()
    {
        // todo: Fix by introducing a warning that there are two handlers handling the same request (but do generate and add both handlers).
        return Verify();
    }

    [Test]
    public Task NoMessagesProgram()
    {
        return Verify();
    }

    [Test]
    public Task QueryHandlerInterfaceProgram()
    {
        return Verify();
    }

    [Test]
    public Task RequestWithoutContractProgram()
    {
        // todo: Try to Implement diagnostic.
        return Verify(true);
    }

    [Test]
    public Task RequestWithoutHandlerProgram()
    {
        return Verify();
    }

    [Test]
    public Task StaticNestedHandlerProgram()
    {
        return Verify();
    }

    [Test]
    public Task StructHandlerProgram()
    {
        // todo: Try to Implement diagnostic.
        return Verify();
    }

    [Test]
    public Task StructMediatorProgram()
    {
        return Verify();
    }

    [Test]
    public Task TwoHandlersDifferentNamespaceProgram()
    {
        return Verify();
    }
}
