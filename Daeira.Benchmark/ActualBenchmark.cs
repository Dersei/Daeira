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

        [Benchmark]
        public bool TestFloat3Immutable()
        {
            Float3 f = new Float3();
            Float3 f1 = new Float3(1, 1, 2);
            var result = true;
            for (var i = 0; i < 1000; i++)
            {
                result = f == f1;
                f1 *= 2;
            }

            return result;
        }

        [Benchmark]
        public bool TestVector3()
        {
            Vector3 f = new Vector3();
            Vector3 f1 = new Vector3(1, 1, 2);
            var result = true;
            for (var i = 0; i < 1000; i++)
            {
                result = f == f1;
                f1 *= 2;
            }

            return result;
        }
    }
}