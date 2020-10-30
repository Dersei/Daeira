using System;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using Daeira.Extensions;

namespace Daeira
{
    public readonly struct Float2 : IEquatable<Float2>, IFormattable
    {
        public readonly float X;
        public readonly float Y;

        public Float2(float x, float y)
        {
            X = x;
            Y = y;
        }
        
        public Float2(float value) : this(value, value)
        {
        }

        public float this[int index] => index switch
        {
            0 => X,
            1 => Y,
            _ => throw new ArgumentOutOfRangeException(nameof(index))
        };

        public static readonly Float2 Zero = new Float2(0, 0);
        public static readonly Float2 One = new Float2(1, 1);
        public static readonly Float2 UnitX = new Float2(1f, 0f);
        public static readonly Float2 UnitY = new Float2(0f, 1f);
        
        private const float Epsilon = 0.00001f;
        private const float DoubledEpsilon = Epsilon * Epsilon;
        private const float EpsilonNormalSqrt = 1e-15f;

        public static readonly Float2 PositiveInfinity =
            new Float2(float.PositiveInfinity, float.PositiveInfinity);

        public static readonly Float2 NegativeInfinity =
            new Float2(float.NegativeInfinity, float.NegativeInfinity);

        public float Length => MathF.Sqrt(X * X + Y * Y);

        public float LengthSquared => X * X + Y * Y;

        #region Operators

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Float2 operator +(Float2 v1, Float2 v2)
        {
            return new Float2(v1.X + v2.X, v1.Y + v2.Y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Float2 operator -(Float2 v1, Float2 v2)
        {
            return new Float2(v1.X - v2.X, v1.Y - v2.Y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Float2 operator *(Float2 v1, Float2 v2)
        {
            return new Float2(v1.X * v2.X, v1.Y * v2.Y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Float2 operator *(Float2 v, float scalar)
        {
            return new Float2(v.X * scalar, v.Y * scalar);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Float2 operator *(float scalar, Float2 v)
        {
            return new Float2(v.X * scalar, v.Y * scalar);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Float2 operator /(Float2 v, float scalar)
        {
            var inverse = 1f / scalar;
            return new Float2(v.X * inverse, v.Y * inverse);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Float2 operator /(float scalar, Float2 v)
        {
            return new Float2(scalar / v.X, scalar / v.Y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Float2 operator -(Float2 v)
        {
            return new Float2(-v.X, -v.Y);
        }

        #endregion

        #region Equality

        public static bool operator ==(Float2 left, Float2 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Float2 left, Float2 right)
        {
            return !left.Equals(right);
        }

        public bool Equals(Float2 other)
        {
            var diffX = X - other.X;
            var diffY = Y - other.Y;
            var sqrMag = diffX * diffX + diffY * diffY;
            return sqrMag < DoubledEpsilon;
        }

        public static bool Equals(Float2 left, Float2 right)
        {
            return left == right;
        }

        public static bool FloatEquals(Float2 left, Float2 right)
        {
            return left.X == right.X && left.Y == right.Y;
        }

        public override bool Equals(object? obj)
        {
            return obj is Float2 other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }

        #endregion

        #region ToString

        public string ToString(string? format, IFormatProvider? formatProvider)
        {
            return
                $"({X.ToString(format, formatProvider)}, {Y.ToString(format, formatProvider)})";
        }

        public string ToString(string? format)
        {
            return ToString(format, CultureInfo.CurrentCulture);
        }

        public override string ToString()
        {
            return $"({X}, {Y})";
        }

        #endregion


        public Float2 Normalize()
        {
            var length = Length;
            if (length > Epsilon)
            {
                return this / length;
            }

            return Zero;
        }
        
        public static Float2 Normalize(Float2 vector)
        {
            return vector.Normalize();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float Dot(Float2 v)
        {
            return X * v.X + Y * v.Y;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Dot(Float2 v1, Float2 v2)
        {
            return v1.X * v2.X + v1.Y * v2.Y;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Float2 Reflect(Float2 normal)
        {
            return -2 * Dot(this, normal) * normal + this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Float2 Reflect(Float2 vector, Float2 normal)
        {
            return -2 * Dot(vector, normal) * normal + vector;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Float2 Lerp(Float2 v, float t)
        {
            return new Float2(X + t * (v.X - X), Y + t * (v.Y - Y));
        }
        
        
        public static Float2 Lerp(Float2 a, Float2 b, float t)
        {
            t = MathExtensions.Clamp01(t);
            return LerpUnclamped(a, b, t);
        }

        // Linearly interpolates between two vectors without clamping the interpolant
        public static Float2 LerpUnclamped(Float2 a, Float2 b, float t)
        {
            return new Float2(
                a.X + (b.X - a.X) * t,
                a.Y + (b.Y - a.Y) * t
            );
        }

        public static float Angle(Float2 from, Float2 to)
        {
            // sqrt(a) * sqrt(b) = sqrt(a * b) -- valid for real numbers
            var denominator = MathF.Sqrt(@from.LengthSquared * to.LengthSquared);
            if (denominator < EpsilonNormalSqrt)
            {
                return 0f;
            }

            var dot = MathExtensions.Clamp(Dot(from, to) / denominator, -1f, 1f);
            return MathF.Acos(dot) * MathExtensions.Rad2Deg;
        }

        public static float SignedAngle(Float2 from, Float2 to)
        {
            var unsignedAngle = Angle(from, to);
            var sign = MathF.Sign(from.X * to.Y - from.Y * to.X);
            return unsignedAngle * sign;
        }

        public static float Distance(Float2 v1, Float2 v2)
        {
            var diffX = v1.X - v2.X;
            var diffY = v1.Y - v2.Y;
            return MathF.Sqrt(diffX * diffX + diffY * diffY);
        }
        
        public static float DistanceSquared(Float2 v1, Float2 v2)
        {
            var diffX = v1.X - v2.X;
            var diffY = v1.Y - v2.Y;
            return diffX * diffX + diffY * diffY;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Float2 SquareRoot(Float2 value)
        {
            return new Float2(
                MathF.Sqrt(value.X),
                MathF.Sqrt(value.Y)
            );
        }
        
        public static Float2 Abs(Float2 value)
        {
            return new Float2(
                MathF.Abs(value.X),
                MathF.Abs(value.Y)
            );
        }

        public Vector2 ToBuiltIn()
        {
            return new Vector2(X, Y);
        }

        public static Float2 FromBuiltIn(Vector2 vector2)
        {
            return new Float2(vector2.X, vector2.Y);
        }

        public static Float2 Transform(Float2 vector3, float scale, Float2 position, Float2 rotationAxis, float angle)
        {
            var matrixScale = Matrix3x2.CreateScale(scale);
            var matrixPosition = Matrix3x2.CreateTranslation(position.ToBuiltIn());
            var matrixRotation = Matrix3x2.CreateRotation(angle * MathExtensions.Deg2Rad, rotationAxis.ToBuiltIn());
            var transformMatrix = matrixScale * matrixRotation * matrixPosition;
            return FromBuiltIn(Vector2.Transform(vector3.ToBuiltIn(), transformMatrix));
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Float2 Min(Float2 value1, Float2 value2)
        {
            return new Float2(
                value1.X < value2.X ? value1.X : value2.X,
                value1.Y < value2.Y ? value1.Y : value2.Y
            );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Float2 Max(Float2 value1, Float2 value2)
        {
            return new Float2(
                value1.X > value2.X ? value1.X : value2.X,
                value1.Y > value2.Y ? value1.Y : value2.Y
            );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Float2 Clamp(Float2 value1, Float2 min, Float2 max)
        {
            // We must follow HLSL behavior in the case user specified min value is bigger than max value.
            return Min(Max(value1, min), max);
        }
        
        /// <summary>Copies the contents of the vector into the given array, starting from index.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyTo(float[] array, int index = 0)
        {
            if (array is null)
            {
                throw new NullReferenceException(nameof(array));
            }

            if (index < 0 || index >= array.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            if (array.Length - index < 2)
            {
                throw new ArgumentException("array.Length - index < 2");
            }

            array[index] = X;
            array[index + 1] = Y;
        }



        public static implicit operator Float2((float x, float y, float z) values) =>
            new Float2(values.x, values.y);

        public static implicit operator (float x, float y)(Float2 v) => (v.X, v.Y);

        public void Deconstruct(out float x, out float y) => (x, y) = (X, Y);
    }
}