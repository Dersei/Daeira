using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Order;
using System;
using Daeira.Extensions;
using static Daeira.Float3Sse;

namespace Daeira.Benchmark
{
    [DisassemblyDiagnoser]
    [GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
    [CategoriesColumn]
    [Orderer(SummaryOrderPolicy.Declared)]
    public class LightBenchmark
    {
        public static Matrix Object2View = new(1, 2, 3, 4, 5, 6, 7, 8, 91, 2, 3, 43, 4, 5,1,1);
        public static Float3Sse TransformNormals(Float3Sse f) => Object2View.MultiplyVector(f);
        public static Float3 TransformNormals(Float3 f) => Object2View.MultiplyVector(f);
        public readonly static Float3Sse NormalSse = new(2, 3, 4);
        public readonly static Float3Sse CameraPositionSse = new(2, -3, 4);
        public readonly static Float3Sse PositionSse = new(2, 6, 4);
        public readonly static Float3Sse VertexPositionSse = new(2, 6, 4);
        public readonly static Float3 Normal = new(2, 3, 4);
        public readonly static Float3 CameraPosition = new(2, -3, 4);
        public readonly static Float3 Position = new(2, 6, 4);
        public readonly static Float3 VertexPosition = new(2, 6, 4);

        [Benchmark]
        public FloatColor CalculateSseLoop()
        {
            FloatColor f = FloatColor.White;
            for (int i = 0; i < 1000; i++)
            {
                f += CalculateSse();
            }

            return f;
        }
        
        [Benchmark]
        public FloatColor CalculateLoop()
        {
            FloatColor f = FloatColor.White;
            for (int i = 0; i < 1000; i++)
            {
                f += Calculate();
            }

            return f;
        }
        
        public FloatColor CalculateSse()
        {
            var n = TransformNormals(NormalSse).NormalizeExact();
            n = n.NormalizeExact();
            var v = (CameraPositionSse - VertexPositionSse).NormalizeExact();
            var r = (-PositionSse).Reflect(n);
            var diff = MathExtensions.Saturate(PositionSse.Dot(n));
            var spec = MathF.Pow( MathExtensions.Saturate(Dot(v, r)),  1 - 0.5f);

            return MathExtensions.Saturate(FloatColor.White * diff + FloatColor.White * spec).AlphaToOne();
        }
        
        public FloatColor Calculate()
        {
            var n = TransformNormals(Normal).NormalizeUnsafe();
            n = n.NormalizeUnsafe();
            var v = (CameraPosition - VertexPosition).NormalizeUnsafe();
            var r = (-Position).Reflect(n);
            var diff = MathExtensions.Saturate(Position.Dot(n));
            var spec = MathF.Pow( MathExtensions.Saturate(Float3.Dot(v, r)),  1 - 0.5f);

            return MathExtensions.Saturate(FloatColor.White * diff + FloatColor.White * spec).AlphaToOne();
        }
    }
}