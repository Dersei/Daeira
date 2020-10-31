using System.Numerics;
using BenchmarkDotNet.Attributes;

namespace Daeira.Benchmark
{
    public class ActualBenchmark
    {
        private void UseFloat3(out Float3 f)
        {
            f = new Float3(0, -100, 0);
        }

        private void UseVector3(out Vector3 v)
        {
            v = new Vector3(0, -100, 0);
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

        [Benchmark]
        public Matrix Transpose()
        {
            var m = new Matrix(
                new Float4(1, 2, 3, 4),
                new Float4(5, 6, 7, 8),
                new Float4(9, 10, 11, 12),
                new Float4(13, 14, 15, 16)
            );
            var result = new Matrix();
            for (int i = 0; i < 100; i++)
            {
                result = Matrix.Transpose(m);
            }

            return result;
        }
        
        [Benchmark]
        public Matrix4x4 Transpose4x4()
        {
            var m = new Matrix4x4(
                1, 2, 3, 4,
                5, 6, 7, 8,
                9, 10, 11, 12,
                13, 14, 15, 16
            );
            var result = new Matrix4x4();
            for (int i = 0; i < 100; i++)
            {
                result = Matrix4x4.Transpose(m);
            }

            return result;
        }
    }
}