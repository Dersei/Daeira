using System;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Daeira
{
    public readonly struct Int3 : IEquatable<Int3>, IFormattable
    {
        public readonly int X;
        public readonly int Y;
        public readonly int Z;

        public Int3(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Int3(int value) : this(value, value, value)
        {
        }

        public Int3(Int2 value, int z) : this(value.X, value.Y, z)
        {
        }

        public int this[int index] => index switch
        {
            0 => X,
            1 => Y,
            2 => Z,
            _ => throw new ArgumentOutOfRangeException(nameof(index))
        };

        public static readonly Int3 Zero = new Int3(0, 0, 0);
        public static readonly Int3 One = new Int3(1, 1, 1);
        public static readonly Int3 Up = new Int3(0, 1, 0);
        public static readonly Int3 Down = new Int3(0, -1, 0);
        public static readonly Int3 Left = new Int3(-1, 0, 0);
        public static readonly Int3 Right = new Int3(1, 0, 0);
        public static readonly Int3 Forward = new Int3(0, 0, 1);
        public static readonly Int3 Back = new Int3(0, 0, -1);
        public static readonly Int3 UnitX = new Int3(1, 0, 0);
        public static readonly Int3 UnitY = new Int3(0, 1, 0);
        public static readonly Int3 UnitZ = new Int3(0, 0, 1);

        public static readonly Int3 MaxValue = new Int3(int.MaxValue, int.MaxValue, int.MaxValue);

        public static readonly Int3 MinValue = new Int3(int.MinValue, int.MinValue, int.MinValue);

        public float Length => MathF.Sqrt(X * X + Y * Y + Z * Z);

        public float LengthSquared => X * X + Y * Y + Z * Z;

        #region Operators

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int3 operator +(Int3 v1, Int3 v2)
        {
            return new Int3(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int3 operator -(Int3 v1, Int3 v2)
        {
            return new Int3(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int3 operator *(Int3 v1, Int3 v2)
        {
            return new Int3(v1.X * v2.X, v1.Y * v2.Y, v1.Z * v2.Z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int3 operator *(Int3 v, int scalar)
        {
            return new Int3(v.X * scalar, v.Y * scalar, v.Z * scalar);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int3 operator *(int scalar, Int3 v)
        {
            return new Int3(v.X * scalar, v.Y * scalar, v.Z * scalar);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int3 operator /(Int3 v, int scalar)
        {
            return new Int3(v.X / scalar, v.Y / scalar, v.Z / scalar);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int3 operator -(Int3 v)
        {
            return new Int3(-v.X, -v.Y, -v.Z);
        }

        #endregion

        #region Equality

        public static bool operator ==(Int3 left, Int3 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Int3 left, Int3 right)
        {
            return !left.Equals(right);
        }

        public bool Equals(Int3 other)
        {
            return X == other.X && Y == other.Y && Z == other.Z;
        }

        public static bool Equals(Int3 left, Int3 right)
        {
            return left == right;
        }

        public override bool Equals(object? obj)
        {
            return obj is Int3 other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y, Z);
        }

        #endregion

        #region ToString

        public string ToString(string? format, IFormatProvider? formatProvider)
        {
            return
                $"({X.ToString(format, formatProvider)}, {Y.ToString(format, formatProvider)}, {Z.ToString(format, formatProvider)})";
        }

        public string ToString(string? format)
        {
            return ToString(format, CultureInfo.CurrentCulture);
        }

        public override string ToString()
        {
            return $"({X}, {Y}, {Z})";
        }

        #endregion


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Dot(Int3 v)
        {
            return X * v.X + Y * v.Y + Z * v.Z;
        }

        public Int3 Cross(Int3 v)
        {
            return new Int3(Y * v.Z - Z * v.Y, Z * v.X - X * v.Z, X * v.Y - Y * v.X);
        }

        private const float EpsilonNormalSqrt = 1e-15f;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Dot(Int3 v1, Int3 v2)
        {
            return v1.X * v2.X + v1.Y * v2.Y + v1.Z * v2.Z;
        }
        
        public static float Distance(Int3 v1, Int3 v2)
        {
            var diffX = v1.X - v2.X;
            var diffY = v1.Y - v2.Y;
            var diffZ = v1.Z - v2.Z;
            return MathF.Sqrt(diffX * diffX + diffY * diffY + diffZ * diffZ);
        }

        public static float DistanceSquared(Int3 v1, Int3 v2)
        {
            var diffX = v1.X - v2.X;
            var diffY = v1.Y - v2.Y;
            var diffZ = v1.Z - v2.Z;
            return diffX * diffX + diffY * diffY + diffZ * diffZ;
        }
        
        public static Int3 Abs(Int3 value)
        {
            return new Int3(
                Math.Abs(value.X),
                Math.Abs(value.Y),
                Math.Abs(value.Z)
            );
        }

        public Vector3 ToBuiltIn()
        {
            return new Vector3(X, Y, Z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int3 Min(Int3 value1, Int3 value2)
        {
            return new Int3(
                value1.X < value2.X ? value1.X : value2.X,
                value1.Y < value2.Y ? value1.Y : value2.Y,
                value1.Z < value2.Z ? value1.Z : value2.Z
            );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int3 Max(Int3 value1, Int3 value2)
        {
            return new Int3(
                value1.X > value2.X ? value1.X : value2.X,
                value1.Y > value2.Y ? value1.Y : value2.Y,
                value1.Z > value2.Z ? value1.Z : value2.Z
            );
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int3 Clamp(Int3 value1, Int3 min, Int3 max)
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

            if (array.Length - index < 3)
            {
                throw new ArgumentException("array.Length - index < 3");
            }

            array[index] = X;
            array[index + 1] = Y;
            array[index + 2] = Z;
        }

        public static implicit operator Int3((int x, int y, int z) values) =>
            new Int3(values.x, values.y, values.z);

        public static implicit operator (int x, int y, int z)(Int3 v) => (v.X, v.Y, v.Z);

        public void Deconstruct(out int x, out int y, out int z) => (x, y, z) = (X, Y, Z);
    }
}