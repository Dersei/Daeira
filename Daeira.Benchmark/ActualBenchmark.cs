using System;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;

namespace Daeira.Benchmark
{ 
    [DisassemblyDiagnoser]
    public class ActualBenchmark
    {
        // private void UseFloat3(out Float3 f)
        // {
        //     f = new Float3(0, -100, 0);
        // }
        //
        // private void UseVector3(out Vector3 v)
        // {
        //     v = new Vector3(0, -100, 0);
        // }

        //
        // [Benchmark]
        // public float Dot()
        // {
        //     Float4 f = new Float4();
        //     Float4 f1 = new Float4(1, 1, 2,3);
        //     var result = 0f;
        //     for (var i = 0; i < 1000; i++)
        //     {
        //         result = Float4.Dot(f,f1);
        //      //   f1 *= 2;
        //     }
        //     
        //     return result;
        // }
        //
        // [Benchmark]
        // public float DotSse()
        // {
        //     Float4Sse f = new Float4Sse();
        //     Float4Sse f1 = new Float4Sse(1, 1, 2,3);
        //     var result = 0f;
        //     for (var i = 0; i < 1000; i++)
        //     {
        //         result = Float4Sse.Dot(f,f1);
        //       //  f1 *= 2;
        //     }
        //
        //     return result;
        // }
        //
        // [Benchmark]
        // public float Length()
        // {
        //     Float4 f = new Float4();
        //     Float4 f1 = new Float4(1, 1, 2,3);
        //     var result = 0f;
        //     for (var i = 0; i < 1000; i++)
        //     {
        //         result += f1.LengthSquared;
        //         //   f1 *= 2;
        //     }
        //     
        //     return result;
        // }
        //
        // [Benchmark]
        // public float LengthSse()
        // {
        //     Float4Sse f = new Float4Sse();
        //     Float4Sse f1 = new Float4Sse(1, 1, 2,3);
        //     var result = 0f;
        //     for (var i = 0; i < 1000; i++)
        //     {
        //         result += f1.LengthSquared;
        //         //  f1 *= 2;
        //     }
        //
        //     return result;
        // }
        //
        //
        // [Benchmark]
        // public Float4 Create()
        // {
        //     var result = Float4.Zero;
        //     for (var i = 0; i < 1000; i++)
        //     {
        //         result = new Float4(i,i,i,i);
        //     }
        //     
        //     return result;
        // }
        //
        // [Benchmark]
        // public Float4Sse CreateSse()
        // {
        //     var result = Float4Sse.Zero;
        //     for (var i = 0; i < 1000; i++)
        //     {
        //         result = new Float4Sse(i,i,i,i);
        //     }
        //     
        //     return result;
        // }
        
        [Benchmark]
        public Float4Sse Normalize()
        {
            var result = Float4Sse.Zero;
            for (var i = 0; i < 1000; i++)
            {
                result += new Float4Sse(i,i,i,i).NormalizeExact() * Float4Sse.UnitX;
            }
            
            return result;
        }
        
        [Benchmark]
        public Float4Sse NormalizeRef()
        {
            var result = Float4Sse.Zero;
            for (var i = 0; i < 1000; i++)
            {
                result += new Float4Sse(i,i,i,i).NormalizeExactRef() * Float4Sse.UnitX;
            }
            
            return result;
        }

        // [Benchmark]
        // public bool TestFloat3Immutable()
        // {
        //     Float3 f = new Float3();
        //     Float3 f1 = new Float3(1, 1, 2);
        //     var result = true;
        //     for (var i = 0; i < 1000; i++)
        //     {
        //         result = f == f1;
        //         f1 *= 2;
        //     }
        //
        //     return result;
        // }

        // [Benchmark]
        // public bool TestVector3()
        // {
        //     Vector3 f = new Vector3();
        //     Vector3 f1 = new Vector3(1, 1, 2);
        //     var result = true;
        //     for (var i = 0; i < 1000; i++)
        //     {
        //         result = f == f1;
        //         f1 *= 2;
        //     }
        //
        //     return result;
        // }
        //
        // [Benchmark]
        // public Float4 Multiply()
        // {
        //     var v = new Float4(1,2,2,2);
        //     var v1 = Matrix.Identity;
        //     var result = Float4.One;
        //     for (int i = 0; i < 1000; i++)
        //     {
        //         result = v1 * v;
        //     }
        //
        //     return result;
        // }
        //
        // [Benchmark]
        // public Float4 MultiplySimd()
        // {
        //     var v = new Float4(1,2,2,2);
        //     var v1 = Matrix.Identity;
        //     var result = Float4.One;
        //     for (int i = 0; i < 1000; i++)
        //     {
        //         result = Matrix.MultiplyNotSimd(v1,v);
        //     }
        //
        //     return result;
        // }
        // [Benchmark]
        // public void MStart()
        // {
        //     M(1000);
        // }
        //
        // [Benchmark]
        // public void NStart()
        // {
        //     N(1000);
        // }
        //
        // [Benchmark]
        // public void OStart()
        // {
        //     O(1000);
        // }
        //
        // public void M(int value) {
        //     string x = "";
        //     for(int i = 0; i < 5000;i++)
        //     {
        //         x = i * 30 + value.ToString();
        //         Test(ref x);
        //     }
        // }
        //
        // public void N(int value) {
        //     for(int i = 0; i < 5000;i++)
        //     {
        //         string x = i * 30 + value.ToString();
        //         Test(ref x);
        //     }
        // }
        //
        // public void O(int value) {
        //     string x;
        //     for(int i = 0; i < 5000;i++)
        //     {
        //         x = i * 30 + value.ToString();
        //         Test(ref x);
        //     }
        // }
        //
        // [MethodImpl(MethodImplOptions.NoInlining)]
        // private void Test(ref string x)
        // {
        //     x += DateTime.Now.ToString(CultureInfo.InvariantCulture);
        // }
    }
}