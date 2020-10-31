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
            // var q1 = new Quaternion(2,3,4,5);
            // var q2 = new System.Numerics.Quaternion(2,3,4,5);
            // var m1 = new Matrix();
            // m1.M21 = 3;
            // m1.M32 = 10;
            // m1.M31 = 9998;
            // var m2 = new Matrix4x4();
            // m2.M12 = 3;
            // m2.M23 = 10;
            // m2.M13 = 9998;
            // Console.WriteLine(Quaternion.CreateFromRotationMatrix(m1));
            // Console.WriteLine(System.Numerics.Quaternion.CreateFromRotationMatrix(m2));
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
               // .AddExporter(RPlotExporter.Default)
                .AddLogger(ConsoleLogger.Default)
                .AddDiagnoser(MemoryDiagnoser.Default));
        }
    }
}