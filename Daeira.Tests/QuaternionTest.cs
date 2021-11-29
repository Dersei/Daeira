using System.Numerics;
using Xunit;

namespace Daeira.Tests
{
    public class QuaternionTest
    {
        [Fact]
        public void BuiltInVsCustom()
        {
            var custom = Quaternion.CreateFromRotationMatrix(new Matrix(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16));
            var builtin = System.Numerics.Quaternion.CreateFromRotationMatrix(new Matrix4x4(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13,
                14, 15, 16));
            Assert.Equal(custom.X, builtin.X);
            Assert.Equal(custom.Y, builtin.Y);
            Assert.Equal(custom.Z, builtin.Z);
            Assert.Equal(custom.W, builtin.W);
        }
    }
}