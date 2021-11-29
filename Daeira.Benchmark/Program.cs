using System;
using System.Numerics;
using System.Runtime.Intrinsics.Arm;
using System.Runtime.Intrinsics.X86;
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
            var m = new Matrix(1, 2, 3, 4,
                5, 6, 7, 8,
                9, 10, 11, 12,
                13, 14, 15, 16);
            var f1 = new Float3(111, 8, 3.7f);
            var f2 = new Float3(1, 78, 4.9f);
            Console.WriteLine(m.MultiplyVector(f1));

            var fs1 = new Float3Sse(111, 8, 3.7f);
            var fs2 = new Float3Sse(1, 78, 4.9f);
            Console.WriteLine(m.MultiplyVector(fs1));
          //  Console.WriteLine(Float3Sse.Cross(fs1, fs2));

            BenchmarkRunner.Run<Float4Benchmark>(ManualConfig
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