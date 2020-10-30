using System;
using System.Numerics;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Exporters.Csv;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Running;

namespace Daeira.Benchmark
{
    internal static class Program
    {
        private static void Main()
        {
            BenchmarkRunner.Run<ActualBenchmark>(ManualConfig
                .Create(DefaultConfig.Instance)
                .AddColumn(TargetMethodColumn.Method)
                .AddColumn(StatisticColumn.Mean)
                .AddColumn(StatisticColumn.Median)
                .AddColumn(StatisticColumn.Min)
                .AddColumn(StatisticColumn.Max)
                .AddColumn(StatisticColumn.OperationsPerSecond)
                .AddColumn(RankColumn.Arabic)
                .AddExporter(CsvExporter.Default)
                .AddExporter(CsvMeasurementsExporter.Default)
                .AddExporter(RPlotExporter.Default)
                .AddLogger(ConsoleLogger.Default)
                .AddDiagnoser(MemoryDiagnoser.Default));
        }
    }
}