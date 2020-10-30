using System;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Daeira
{
    public readonly struct Int2 : IEquatable<Int2>, IFormattable
    {
        public readonly int X;
        public readonly int Y;

        public Int2(int x, int y)
        {
            X = x;
            Y = y;
        }

        public Int2(int value) : this(value, value)
        {
        }

        public int this[int index] => index switch
        {
            0 => X,
            1 => Y,
            _ => throw new ArgumentOutOfRangeException(nameof(index))
        };

        public static readonly Int2 Zero = new Int2(0, 0);
        public static readonly Int2 One = new Int2(1, 1);
        public static readonly Int2 UnitX = new Int2(1, 0 );
        public static readonly Int2 UnitY = new Int2(0, 1);

        public static readonly Int2 MaxValue = new Int2(int.MaxValue, int.MaxValue);

        public static readonly Int2 MinValue = new Int2(int.MinValue, int.MinValue);

        public float Length => MathF.Sqrt(X * X + Y * Y);

        public float LengthSquared => X * X + Y * Y ;

        #region Operators

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int2 operator +(Int2 v1, Int2 v2)
        {
            return new Int2(v1.X + v2.X, v1.Y + v2.Y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int2 operator -(Int2 v1, Int2 v2)
        {
            return new Int2(v1.X - v2.X, v1.Y - v2.Y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int2 operator *(Int2 v1, Int2 v2)
        {
            return new Int2(v1.X * v2.X, v1.Y * v2.Y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int2 operator *(Int2 v, int scalar)
        {
            return new Int2(v.X * scalar, v.Y * scalar);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int2 operator *(int scalar, Int2 v)
        {
            return new Int2(v.X * scalar, v.Y * scalar);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int2 operator /(Int2 v, int scalar)
        {
            return new Int2(v.X / scalar, v.Y / scalar);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int2 operator -(Int2 v)
        {
            return new Int2(-v.X, -v.Y);
        }

        #endregion

        #region Equality

        public static bool operator ==(Int2 left, Int2 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Int2 left, Int2 right)
        {
            return !left.Equals(right);
        }

        public bool Equals(Int2 other)
        {
            return X == other.X && Y == other.Y;
        }

        public static bool Equals(Int2 left, Int2 right)
        {
            return left == right;
        }

        public override bool Equals(object? obj)
        {
            return obj is Int2 other && Equals(other);
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


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Dot(Int2 v)
        {
            return X * v.X + Y * v.Y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Dot(Int2 v1, Int2 v2)
        {
            return v1.X * v2.X + v1.Y * v2.Y;
        }
        
        public static float Distance(Int2 v1, Int2 v2)
        {
            var diffX = v1.X - v2.X;
            var diffY = v1.Y - v2.Y;
            return MathF.Sqrt(diffX * diffX + diffY * diffY);
        }

        public static float DistanceSquared(Int2 v1, Int2 v2)
        {
            var diffX = v1.X - v2.X;
            var diffY = v1.Y - v2.Y;
            return diffX * diffX + diffY * diffY;
        }
        
        public static Int2 Abs(Int2 value)
        {
            return new Int2(
                Math.Abs(value.X),
                Math.Abs(value.Y)
                );
        }

        public Vector2 ToBuiltIn()
        {
            return new Vector2(X, Y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int2 Min(Int2 value1, Int2 value2)
        {
            return new Int2(
                value1.X < value2.X ? value1.X : value2.X,
                value1.Y < value2.Y ? value1.Y : value2.Y
            );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int2 Max(Int2 value1, Int2 value2)
        {
            return new Int2(
                value1.X > value2.X ? value1.X : value2.X,
                value1.Y > value2.Y ? value1.Y : value2.Y
            );
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int2 Clamp(Int2 value1, Int2 min, Int2 max)
        {
            // We must follow HLSL behavior in the case user specified min value is bigger than max value.
            return Min(Max(value1, min), max);
        }


        /// <summary>Copies the contents of the vector into the given array, starting from index.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyTo(int[] array, int index = 0)
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

        public static implicit operator Int2((int x, int y) values) =>
            new Int2(values.x, values.y);

        public static implicit operator (int x, int y)(Int2 v) => (v.X, v.Y);

        public void Deconstruct(out int x, out int y) => (x, y) = (X, Y);
    }
}