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
            var m = new Matrix(
                1, 2, 3, 4,
                5, 6, 7, 8,
                9, 10, 11, 12,
                13, 14, 15, 16
            );
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
            
            var f1 = new Float3(0.5f, -1f, 4.5f);
            var f2 = new Float3(16.9f, 123f, 134.5f);
            var fs1 = new Float3Sse(0.5f, -1f, 4.5f);
            var fs2 = new Float3Sse(16.9f, 123f, 134.5f);
            // Console.WriteLine(f1 * f2);
            // Console.WriteLine(f1.Dot(f2));
            // Console.WriteLine(fs1 * fs2);
            // Console.WriteLine(fs1.Dot(fs2));
            Console.WriteLine(m.MultiplyVector(f1));
            Console.WriteLine(m.MultiplyVector(fs1));
            Console.WriteLine(Matrix.MultiplyVector(m,f1));
            Console.WriteLine(Matrix.MultiplyVector(m,fs1));
          //  return;
            BenchmarkRunner.Run<Float3Benchmark>(ManualConfig
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