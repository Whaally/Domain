using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;

BenchmarkSwitcher
    .FromAssembly(typeof(Program).Assembly)
    .Run(args, config: ManualConfig
        .Create(DefaultConfig.Instance)
        .WithOptions(ConfigOptions.DisableOptimizationsValidator)); // Must be done because FluentResults is not optimized
    