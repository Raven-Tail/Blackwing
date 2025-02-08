using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Running;
using Ravitor.Benchmark;

//RavitorImpl.Setup();
//await RavitorImpl.Run();

BenchmarkRunner.Run<Benchmarks>();

[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
public class Benchmarks
{
    [GlobalSetup]
    public void GlobalSetup()
    {
        RavitorImpl.Setup();
        MediatRImpl.Setup();
        MediatorImpl.Setup();
    }

    [Benchmark(Baseline = true)]
    public Task Ravitor()
        => RavitorImpl.Run();

    [Benchmark]
    public Task Ravitor_Class()
        => RavitorImpl.Run_Class();

    [Benchmark]
    public Task MediatR()
        => MediatRImpl.Run();

    [Benchmark]
    public Task Mediator()
        => MediatorImpl.Run();

    [Benchmark]
    public Task Mediator_CLass()
        => MediatorImpl.Run_Class();
}
