using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Order;

namespace Daeira.Benchmark
{
    [DisassemblyDiagnoser]
    [GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
    [CategoriesColumn]
    [Orderer(SummaryOrderPolicy.Declared)]
    public class MatrixBenchmark
    {
        [Benchmark]
        public Matrix MultiplySse()
        {
            var m1 = new Matrix(1, 1, 2,1,2,3,5,7,5,3,23,1,2,3,5,6);
            var result = Matrix.Identity;
            for (var i = 0; i < 1000; i++)
            {
                result *= m1;
            }

            return result;
        }
        
        [Benchmark]
        public Matrix Multiply()
        {
            var m1 = new Matrix(1, 1, 2,1,2,3,5,7,5,3,23,1,2,3,5,6);
            var result = Matrix.Identity;
            for (var i = 0; i < 1000; i++)
            {
                result = Matrix.Multiply(result, m1);
            }

            return result;
        }
        
        
    }
}