using System.Numerics;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Order;

namespace Daeira.Benchmark
{
    [DisassemblyDiagnoser]
    [GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
    [CategoriesColumn]
    [Orderer(SummaryOrderPolicy.Declared)]
    public class Float4Benchmark
    {
          
        [Benchmark]
        [BenchmarkCategory("Normalize")]
        public Float4 Normalize()
        {
            var result = Float4.Zero;
            for (var i = 0; i < 1000; i++)
            {
                result += Float4.Normalize(new Float4(i, i, i,i));
            }
        
            return result;
        }
        
        [Benchmark]
        [BenchmarkCategory("Normalize")]
        public Float4 NormalizeDot()
        {
            var result = Float4.Zero;
            for (var i = 0; i < 1000; i++)
            {
                result += Float4.NormalizeDot(new Float4(i, i, i,i));
            }
        
            return result;
        }
        
        [Benchmark]
        [BenchmarkCategory("Normalize")]
        public Vector4 NormalizeBuiltIn()
        {
            var result = Vector4.Zero;
            for (var i = 0; i < 1000; i++)
            {
                result += Vector4.Normalize(new Vector4(i, i, i,i));
            }
        
            return result;
        }
        
        [Benchmark]
        [BenchmarkCategory("Normalize")]
        public Float4 NormalizeUnsafe()
        {
            var result = Float4.Zero;
            for (var i = 0; i < 1000; i++)
            {
                result +=Float4.NormalizeUnsafe(new Float4(i, i, i,i));
            }
        
            return result;
        }

        #region Commented

         // [Benchmark]
        // [BenchmarkCategory("Multiply")]
        // public Float4 Multiply()
        // {
        //     Float4 f1 = new Float4(1, 1, 2, 3);
        //     Float4 result = new Float4();
        //     for (var i = 0; i < 1000; i++)
        //     {
        //         result *= f1;
        //     }
        //
        //     return result;
        // }
        //
        // [Benchmark]
        // [BenchmarkCategory("Multiply")]
        // public Float4Sse MultiplySse()
        // {
        //     Float4Sse f1 = new Float4Sse(1, 1, 2, 3);
        //     Float4Sse result = new Float4Sse();
        //     for (var i = 0; i < 1000; i++)
        //     {
        //         result *= f1;
        //     }
        //
        //     return result;
        // }
        //
        // [Benchmark]
        // [BenchmarkCategory("Add")]
        // public Float4 Add()
        // {
        //     Float4 f1 = new Float4(1, 1, 2, 3);
        //     Float4 result = new Float4();
        //     for (var i = 0; i < 1000; i++)
        //     {
        //         result += f1;
        //     }
        //
        //     return result;
        // }
        //
        // [Benchmark]
        // [BenchmarkCategory("Add")]
        // public Float4Sse AddSse()
        // {
        //     Float4Sse f1 = new Float4Sse(1, 1, 2, 3);
        //     Float4Sse result = new Float4Sse();
        //     for (var i = 0; i < 1000; i++)
        //     {
        //         result += f1;
        //     }
        //
        //     return result;
        // }
        //
        // [Benchmark]
        // [BenchmarkCategory("Negate")]
        // public Float4 Negate()
        // {
        //     Float4 result = new Float4();
        //     for (var i = 0; i < 1000; i++)
        //     {
        //         result = -new Float4(i, 1, i, 3);
        //     }
        //
        //     return result;
        // }
        //
        // [Benchmark]
        // [BenchmarkCategory("Negate")]
        // public Float4Sse NegateSse()
        // {
        //     Float4Sse result = new Float4Sse();
        //     for (var i = 0; i < 1000; i++)
        //     {
        //         result = -new Float4Sse(i, 1, i, 3);
        //     }
        //
        //     return result;
        // }
        //
        // [Benchmark]
        // [BenchmarkCategory("Dot")]
        // public float Dot()
        // {
        //     Float4 f = new Float4();
        //     Float4 f1 = new Float4(1, 1, 2, 3);
        //     var result = 0f;
        //     for (var i = 0; i < 1000; i++)
        //     {
        //         result += Float4.Dot(f, f1);
        //     }
        //
        //     return result;
        // }
        //
        // [Benchmark]
        // [BenchmarkCategory("Dot")]
        // public float DotSse()
        // {
        //     Float4Sse f = new Float4Sse();
        //     Float4Sse f1 = new Float4Sse(1, 1, 2, 3);
        //     var result = 0f;
        //     for (var i = 0; i < 1000; i++)
        //     {
        //         result += Float4Sse.Dot(f, f1);
        //     }
        //
        //     return result;
        // }
        //
        // [Benchmark]
        // [BenchmarkCategory("Reflect")]
        // public Float4 Reflect()
        // {
        //     Float4 f = new Float4();
        //     Float4 f1 = new Float4(1, 1, 2, 3);
        //     var result = new Float4();
        //     for (var i = 0; i < 1000; i++)
        //     {
        //         result += Float4.Reflect(f, f1);
        //     }
        //
        //     return result;
        // }
        //
        // [Benchmark]
        // [BenchmarkCategory("Reflect")]
        // public Float4Sse ReflectSse()
        // {
        //     Float4Sse f = new Float4Sse();
        //     Float4Sse f1 = new Float4Sse(1, 1, 2, 3);
        //     var result = new Float4Sse();
        //     for (var i = 0; i < 1000; i++)
        //     {
        //         result += Float4Sse.Reflect(f, f1);
        //     }
        //
        //     return result;
        // }
        //
        // [Benchmark]
        // [BenchmarkCategory("Project")]
        // public Float4 Project()
        // {
        //     Float4 f = new Float4();
        //     Float4 f1 = new Float4(1, 1, 2, 3);
        //     var result = new Float4();
        //     for (var i = 0; i < 1000; i++)
        //     {
        //         result += Float4.Project(f, f1);
        //     }
        //
        //     return result;
        // }
        //
        // [Benchmark]
        // [BenchmarkCategory("Project")]
        // public Float4Sse ProjectSse()
        // {
        //     Float4Sse f = new Float4Sse();
        //     Float4Sse f1 = new Float4Sse(1, 1, 2, 3);
        //     var result = new Float4Sse();
        //     for (var i = 0; i < 1000; i++)
        //     {
        //         result += Float4Sse.Project(f, f1);
        //     }
        //
        //     return result;
        // }
        //
        // [Benchmark]
        // [BenchmarkCategory("Saturate")]
        // public Float4 Saturate()
        // {
        //     Float4 f1 = new Float4(1, 1, 2, 3);
        //     var result = new Float4();
        //     for (var i = 0; i < 1000; i++)
        //     {
        //         result = Float4.Saturate(f1);
        //     }
        //
        //     return result;
        // }
        //
        // [Benchmark]
        // [BenchmarkCategory("Saturate")]
        // public Float4Sse SaturateSse()
        // {
        //     Float4Sse f1 = new Float4Sse(1, 1, 2, 3);
        //     var result = new Float4Sse();
        //     for (var i = 0; i < 1000; i++)
        //     {
        //         result = Float4Sse.Saturate(f1);
        //     }
        //
        //     return result;
        // }
        //
        // //
        // [Benchmark]
        // [BenchmarkCategory("Length")]
        // public float Length()
        // {
        //     Float4 f1 = new Float4(1, 1, 2, 3);
        //     var result = 0f;
        //     for (var i = 0; i < 1000; i++)
        //     {
        //         result += f1.Length;
        //         f1 *= 2;
        //     }
        //
        //     return result;
        // }
        //
        // [Benchmark]
        // [BenchmarkCategory("Length")]
        // public float LengthSse()
        // {
        //     Float4Sse f1 = new Float4Sse(1, 1, 2, 3);
        //     var result = 0f;
        //     for (var i = 0; i < 1000; i++)
        //     {
        //         result += f1.Length;
        //         f1 *= 2;
        //     }
        //
        //     return result;
        // }
        //
        // //
        // //
        // [Benchmark]
        // [BenchmarkCategory("Create")]
        // public Float4 Create()
        // {
        //     var result = Float4.Zero;
        //     for (var i = 0; i < 1000; i++)
        //     {
        //         result = new Float4(i, i, i, i);
        //     }
        //
        //     return result;
        // }
        //
        // [Benchmark]
        // [BenchmarkCategory("Create")]
        // public Float4Sse CreateSse()
        // {
        //     var result = Float4Sse.Zero;
        //     for (var i = 0; i < 1000; i++)
        //     {
        //         result = new Float4Sse(i, i, i, i);
        //     }
        //
        //     return result;
        // }
        //
        // [Benchmark]
        // [BenchmarkCategory("Normalize")]
        // public Float4 Normalize()
        // {
        //     var result = Float4.Zero;
        //     for (var i = 0; i < 1000; i++)
        //     {
        //         result += new Float4(i, i, i, i).Normalize() * Float4.UnitX;
        //     }
        //
        //     return result;
        // }
        //
        // [Benchmark]
        // [BenchmarkCategory("Normalize")]
        // public Float4Sse NormalizeSse()
        // {
        //     var result = Float4Sse.Zero;
        //     for (var i = 0; i < 1000; i++)
        //     {
        //         result += new Float4Sse(i, i, i, i).Normalize() * Float4Sse.UnitX;
        //     }
        //
        //     return result;
        // }
        //
        // [Benchmark]
        // [BenchmarkCategory("Normalize")]
        // public Float4Sse NormalizeSseExact()
        // {
        //     var result = Float4Sse.Zero;
        //     for (var i = 0; i < 1000; i++)
        //     {
        //         result += new Float4Sse(i, i, i, i).NormalizeExact() * Float4Sse.UnitX;
        //     }
        //
        //     return result;
        // }

        #endregion
    }
}