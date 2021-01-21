using System;
using Daeira.Extensions;
using Xunit;

namespace Daeira.Tests
{
    public class Float4Test
    {
        [Fact]
        public void Addition()
        {
            var f1 = new Float4(1.5f, 1f, 4.5f, 6.7f);
            var f2 = new Float4(16.9f, 123f, 134.5f, 1096.9f);
            var result = new Float4(18.4f, 124f, 139f, 1103.6f);
            Assert.Equal(result, f1+f2);
        }
        
        [Fact]
        public void Subtraction()
        {
            var f1 = new Float4(1.5f, 1f, 4.5f, 6.7f);
            var f2 = new Float4(16.9f, 123f, 134.5f, 1096.9f);
            var result = new Float4(15.4f, 122f, 130f, 1090.2001f);
            Assert.Equal(result, f2-f1);
        }
        
        [Fact]
        public void Multiplication()
        {
            var f1 = new Float4(1.5f, 1f, 4.5f, 6.7f);
            var f2 = new Float4(16.9f, 123f, 134.5f, 1096.9f);
            var result = new Float4(25.349998f, 123f, 605.25f, 7349.23f);
            Assert.Equal(result, f1*f2);
            Assert.Equal(f2*f1, f1*f2);
        }
        
        [Fact]
        public void Division()
        {
            const float f1 = 2f;
            var f2 = new Float4(16.9f, 123f, 134.5f, 1096.9f);
            var result = new Float4(0.1183432f, 0.016260162f, 0.014869888f, 0.0018233202f);
            Assert.Equal(result, f1/f2);
        }
        
        [Fact]
        public void InvertDivision()
        {
            const float f1 = 2f;
            var f2 = new Float4(16.9f, 123f, 134.5f, 1096.9f);
            var result = new Float4(8.45f, 61.5f, 67.25f, 548.45f);
            Assert.Equal(result, f2/f1);
        }
        
        [Fact]
        public void Dot()
        {
            var f1 = new Float4(1.5f, 1f, 4.5f, 6.7f);
            var f2 = new Float4(16.9f, 123f, 134.5f, 1096.9f);
            const float result = 8102.83f;
            Assert.Equal(result, f1.Dot(f2));
            Assert.Equal(f1.Dot(f2), f2.Dot(f1));
        }
        
        [Fact]
        public void Reflect()
        {
            var f1 = new Float4(1.5f, 1f, 4.5f, 6.7f);
            var f2 = new Float4(16.9f, 123f, 134.5f, 1096.9f);
            var result = new Float4(-273874.16f, -1993295.2f, -2179656.8f, -17775984f);
            Assert.Equal(result, Float4.Reflect(f1,f2));
        }
        
        [Fact]
        public void Saturate()
        {
            var f2 = new Float4(0.9f, -123f, 134.5f, 1096.9f);
            var result = new Float4(0.9f, 0, 1, 1);
            Assert.Equal(result, Float4.Saturate(f2));
        }
        
        [Fact]
        public void Normalize()
        {
            var f1 = new Float4(1.5f, 1f, 4.5f, 6.7f).Normalize();
            var f2 = new Float4(16.9f, 123f, 134.5f, 1096.9f).Normalize();
            Assert.True(f1.Length.IsAbout(1));
            Assert.True(f2.Length.IsAbout(1));
        }
        
        [Fact]
        public void Equality()
        {
            var f1 = new Float4(1.5f, 1f, 4.5f, 6.7f);
            var f2 = new Float4(1.5f, 1f, 4.5f, 6.7f);
            Assert.True(f1 == f2);
            Assert.False(f1 != f2);
            Assert.True(f1.Normalize() == f2.Normalize());
            Assert.False(f1.Normalize() != f2.Normalize());
        }
    }
}