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
            // var m = new Matrix(
            //     1, 2, 3, 4,
            //     5, 6, 7, 8,
            //     9, 10, 11, 12,
            //     13, 14, 15, 16
            // );
            //
            // var m4 = new Matrix4x4(
            //     1, 2, 3, 4,
            //     5, 6, 7, 8,
            //     9, 10, 11, 12,
            //     13, 14, 15, 16
            // );
            // Console.WriteLine(m * m);
            // Console.WriteLine(m4 * m4);

            // Complex c = new Complex(float.MaxValue / (MathF.Sqrt(2.0f) + 1.0f), 1);
            // ComplexF c1 = new ComplexF(float.MaxValue / (MathF.Sqrt(2.0f) + 1.0f), 1);
            //
            // Console.WriteLine(Complex.Sqrt(c) == Complex.Sqrt(c));
            // Console.WriteLine(ComplexF.Sqrt(c1) == ComplexF.Sqrt(c1));
            //
            // var fc = new FloatColor();
            
            var f1 = new Float4(1.5f, 1f, 4.5f, 6.7f);
            var f2 = new Float4(16.9f, 123f, 134.5f, 1096.9f);
            Console.WriteLine(f1.Dot(f2));
            var f3 = new Float4Sse(1.5f, 1f, 4.5f, 6.7f).NormalizeExact();
            var f4 = new Float4Sse(16.9f, 123f, 134.5f, 1096.9f).NormalizeExact();
            Console.WriteLine(f3.Length);
            Console.WriteLine(f4.Length);
            Float4Sse fs = new Float4Sse(2,3,6,7);
            Float4Sse f1s = new Float4Sse(1.5f, 1.3f, 2,3);
            Console.WriteLine(fs+f1s);
            Console.WriteLine(fs.Dot(f1s));
            Console.WriteLine(f2.Dot(f1));
            return;
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