using System;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Order;

namespace Daeira.Benchmark
{
    [DisassemblyDiagnoser]
    [GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
    [CategoriesColumn]
    [Orderer(SummaryOrderPolicy.Declared, MethodOrderPolicy.Declared)]
    public class Float3Benchmark
    {
        [Benchmark]
        [BenchmarkCategory("Multiply")]
        public Float3Sse Multiply()
        {
            Float3Sse f1 = new Float3Sse(1, 1, 2);
            Matrix m = Matrix.Identity;
            Float3Sse result = new Float3Sse();
            for (var i = 0; i < 1000; i++)
            {
                result += m.MultiplyVector(f1);
            }

            return result;
        }

        [Benchmark]
        [BenchmarkCategory("Multiply")]
        public Float3 MultiplyBase()
        {
            Float3 f1 = new Float3(1, 1, 2);
            Matrix m = Matrix.Identity;
            Float3 result = new Float3();
            for (var i = 0; i < 1000; i++)
            {
                result += m.MultiplyVector(f1);
            }

            return result;
        }

        [Benchmark]
        [BenchmarkCategory("Multiply")]
        public Float3Sse MultiplySse()
        {
            Float3Sse f1 = new Float3Sse(1, 1, 2);
            Matrix m = Matrix.Identity;
            Float3Sse result = new Float3Sse();
            for (var i = 0; i < 1000; i++)
            {
                result += Matrix.MultiplyVector(m, f1);
            }

            return result;
        }

        [Benchmark]
        [BenchmarkCategory("GetElement")]
        public float GetElement()
        {
            Vector128<float> f1 = Vector128.Create(1f, 4f, 5f, 3f);
            float result = 0;
            for (var i = 0; i < 1000; i++)
            {
                result += f1.GetElement(i % 3);
            }

            return result;
        }

        [Benchmark]
        [BenchmarkCategory("GetElement")]
        public unsafe float GetElementUnsafe()
        {
            Vector128<float> f1 = Vector128.Create(1f, 4f, 5f, 3f);
            float result = 0;
            for (var i = 0; i < 1000; i++)
            {
                result += *((float*) &f1 + i % 3);
            }

            return result;
        }


        [Benchmark]
        [BenchmarkCategory("Create")]
        public Vector128<float> Create()
        {
            Vector128<float> result = Vector128<float>.Zero;
            for (var i = 0; i < 1000; i++)
            {
                result = Vector128.Create(i, i / 2f, 5f, 3f);
            }

            return result;
        }

        [Benchmark]
        [BenchmarkCategory("Create")]
        public unsafe Vector128<float> CreateUnsafe()
        {
            Vector128<float> result = Vector128<float>.Zero;
            for (var i = 0; i < 1000; i++)
            {
                var t = (i, i / 2f, 5f, 3f);
                result = *(Vector128<float>*)&t;
            }

            return result;
        }

        // [Benchmark]
        // [BenchmarkCategory("Multiply")]
        // public Float3 Multiply()
        // {
        //     Float3 f1 = new Float3(1, 1, 2);
        //     Float3 result = new Float3();
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
        // public Float3Sse MultiplySse()
        // {
        //     Float3Sse f1 = new Float3Sse(1, 1, 2);
        //     Float3Sse result = new Float3Sse();
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
        // public Float3 Add()
        // {
        //     Float3 f1 = new Float3(1, 1, 2);
        //     Float3 result = new Float3();
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
        // public Float3Sse AddSse()
        // {
        //     Float3Sse f1 = new Float3Sse(1, 1, 2);
        //     Float3Sse result = new Float3Sse();
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
        // public Float3 Negate()
        // {
        //     Float3 result = new Float3();
        //     for (var i = 0; i < 1000; i++)
        //     {
        //         result = -new Float3(i, 1, i);
        //     }
        //
        //     return result;
        // }
        //
        // [Benchmark]
        // [BenchmarkCategory("Negate")]
        // public Float3Sse NegateSse()
        // {
        //     Float3Sse result = new Float3Sse();
        //     for (var i = 0; i < 1000; i++)
        //     {
        //         result = -new Float3Sse(i, 1, i);
        //     }
        //
        //     return result;
        // }
        //
        // [Benchmark]
        // [BenchmarkCategory("Dot")]
        // public float Dot()
        // {
        //     Float3 f = new Float3();
        //     Float3 f1 = new Float3(1, 1, 2);
        //     var result = 0f;
        //     for (var i = 0; i < 1000; i++)
        //     {
        //         result += Float3.Dot(f, f1);
        //     }
        //
        //     return result;
        // }
        //
        // [Benchmark]
        // [BenchmarkCategory("Dot")]
        // public float DotSse()
        // {
        //     Float3Sse f = new Float3Sse();
        //     Float3Sse f1 = new Float3Sse(1, 1, 2);
        //     var result = 0f;
        //     for (var i = 0; i < 1000; i++)
        //     {
        //         result += Float3Sse.Dot(f, f1);
        //     }
        //
        //     return result;
        // }
        //
        // [Benchmark]
        // [BenchmarkCategory("Reflect")]
        // public Float3 Reflect()
        // {
        //     Float3 f = new Float3();
        //     Float3 f1 = new Float3(1, 1, 2);
        //     var result = new Float3();
        //     for (var i = 0; i < 1000; i++)
        //     {
        //         result += Float3.Reflect(f, f1);
        //     }
        //
        //     return result;
        // }
        //
        // [Benchmark]
        // [BenchmarkCategory("Reflect")]
        // public Float3Sse ReflectSse()
        // {
        //     Float3Sse f = new Float3Sse();
        //     Float3Sse f1 = new Float3Sse(1, 1, 2);
        //     var result = new Float3Sse();
        //     for (var i = 0; i < 1000; i++)
        //     {
        //         result += Float3Sse.Reflect(f, f1);
        //     }
        //
        //     return result;
        // }
        //
        // [Benchmark]
        // [BenchmarkCategory("Project")]
        // public Float3 Project()
        // {
        //     Float3 f = new Float3();
        //     Float3 f1 = new Float3(1, 1, 2);
        //     var result = new Float3();
        //     for (var i = 0; i < 1000; i++)
        //     {
        //         result += Float3.Project(f, f1);
        //     }
        //
        //     return result;
        // }
        //
        // [Benchmark]
        // [BenchmarkCategory("Project")]
        // public Float3Sse ProjectSse()
        // {
        //     Float3Sse f = new Float3Sse();
        //     Float3Sse f1 = new Float3Sse(1, 1, 2);
        //     var result = new Float3Sse();
        //     for (var i = 0; i < 1000; i++)
        //     {
        //         result += Float3Sse.Project(f, f1);
        //     }
        //
        //     return result;
        // }
        //
        // [Benchmark]
        // [BenchmarkCategory("Saturate")]
        // public Float3 Saturate()
        // {
        //     Float3 f1 = new Float3(1, 1, 2);
        //     var result = new Float3();
        //     for (var i = 0; i < 1000; i++)
        //     {
        //         result = Float3.Saturate(f1);
        //     }
        //
        //     return result;
        // }
        //
        // [Benchmark]
        // [BenchmarkCategory("Saturate")]
        // public Float3Sse SaturateSse()
        // {
        //     Float3Sse f1 = new Float3Sse(1, 1, 2);
        //     var result = new Float3Sse();
        //     for (var i = 0; i < 1000; i++)
        //     {
        //         result = Float3Sse.Saturate(f1);
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
        //     Float3 f1 = new Float3(1, 1, 2);
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
        //     Float3Sse f1 = new Float3Sse(1, 1, 2);
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
        // public Float3 Create()
        // {
        //     var result = Float3.Zero;
        //     for (var i = 0; i < 1000; i++)
        //     {
        //         result = new Float3(i, i, i);
        //     }
        //
        //     return result;
        // }
        //
        // [Benchmark]
        // [BenchmarkCategory("Create")]
        // public Float3Sse CreateSse()
        // {
        //     var result = Float3Sse.Zero;
        //     for (var i = 0; i < 1000; i++)
        //     {
        //         result = new Float3Sse(i, i, i);
        //     }
        //
        //     return result;
        // }
        //
        // [Benchmark]
        // [BenchmarkCategory("Normalize")]
        // public Float3 Normalize()
        // {
        //     var result = Float3.Zero;
        //     for (var i = 0; i < 1000; i++)
        //     {
        //         result += new Float3(i, i, i).Normalize() * Float3.UnitX;
        //     }
        //
        //     return result;
        // }
        //
        // [Benchmark]
        // [BenchmarkCategory("Normalize")]
        // public Float3Sse NormalizeSse()
        // {
        //     var result = Float3Sse.Zero;
        //     for (var i = 0; i < 1000; i++)
        //     {
        //         result += new Float3Sse(i, i, i).Normalize() * Float3Sse.UnitX;
        //     }
        //
        //     return result;
        // }
        //
        // [Benchmark]
        // [BenchmarkCategory("Normalize")]
        // public Float3Sse NormalizeSseExact()
        // {
        //     var result = Float3Sse.Zero;
        //     for (var i = 0; i < 1000; i++)
        //     {
        //         result += new Float3Sse(i, i, i).NormalizeExact() * Float3Sse.UnitX;
        //     }
        //
        //     return result;
        // }
    }
}