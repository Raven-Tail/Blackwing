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
$ hyperfine ..\..\artifacts\sample\publish\Console\release\Console.exe ..\..\artifacts\sample\publish\ConsoleAOT\release\ConsoleAOT.exe --warmup 5
Benchmark 1: Console.exe
  Time (mean ± σ):      57.7 ms ±   1.7 ms    [User: 16.9 ms, System: 2.9 ms]
  Range (min … max):    55.2 ms …  64.1 ms    48 runs

Benchmark 2: ConsoleAOT.exe
  Time (mean ± σ):       9.6 ms ±   0.4 ms    [User: 0.0 ms, System: 0.1 ms]
  Range (min … max):     8.7 ms …  11.6 ms    202 runs

Summary
  ConsoleAOT.exe ran 5.99 ± 0.33 times faster than Console.exe
```