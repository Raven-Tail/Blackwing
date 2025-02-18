## ConsoleAOT

Just like Console, but with [NativeAOT](https://github.com/dotnet/runtimelab/tree/feature/NativeAOT) enabled.

### Build and run

```console
$ dotnet run
1) Running logger handler
2) Running ping validator
3) Valid input!
4) Returning pong!
5) No error!
-----------------------------------
ID: 4f5e8fe3-e64f-4042-9ed3-33b894be8776
Ping { Id = 4f5e8fe3-e64f-4042-9ed3-33b894be8776 }
Pong { Id = 4f5e8fe3-e64f-4042-9ed3-33b894be8776 }
Equal: True
```

### Comparison

Below the ConsoleAOT project is benchmarked against Console using [hyperfine](https://github.com/sharkdp/hyperfine).

You can execute `.\benchmark.ps1` to get the following output or compare them yourself as the command shows.

This was executed on an `AMD Ryzen 5 5600` processor.

```pwsh
$ hyperfine "../../artifacts/sample/publish/Console/release/Console.exe" "../../artifacts/sample/publish/ConsoleAOT/release/ConsoleAOT.exe"

Benchmark 1: Console.exe
  Time (mean ± σ):      57.8 ms ±   2.7 ms    [User: 23.3 ms, System: 7.1 ms]
  Range (min … max):    54.9 ms …  70.3 ms    45 runs

Benchmark 2: ConsoleAOT.exe
  Time (mean ± σ):      10.5 ms ±   4.2 ms    [User: 0.5 ms, System: 1.9 ms]
  Range (min … max):     9.0 ms …  42.8 ms    62 runs

  Warning: The first benchmarking run for this command was significantly slower than the rest (42.8 ms). This could be caused by (filesystem) caches that were not filled until after the first run. You should consider using the '--warmup' option to fill those caches before the actual benchmark. Alternatively, use the '--prepare' option to clear the caches before each timing run.

Summary
  ConsoleAOT.exe ran 5.50 ± 2.22 times faster than Console.exe
```