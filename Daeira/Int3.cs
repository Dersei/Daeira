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

        public Int3(in Int2 value, int z) : this(value.X, value.Y, z)
        {
        }

        public int this[int index] => index switch
        {
            0 => X,
            1 => Y,
            2 => Z,
            _ => throw new ArgumentOutOfRangeException(nameof(index))
        };

        public static readonly Int3 Zero = new(0, 0, 0);
        public static readonly Int3 One = new(1, 1, 1);
        public static readonly Int3 Up = new(0, 1, 0);
        public static readonly Int3 Down = new(0, -1, 0);
        public static readonly Int3 Left = new(-1, 0, 0);
        public static readonly Int3 Right = new(1, 0, 0);
        public static readonly Int3 Forward = new(0, 0, 1);
        public static readonly Int3 Back = new(0, 0, -1);
        public static readonly Int3 UnitX = new(1, 0, 0);
        public static readonly Int3 UnitY = new(0, 1, 0);
        public static readonly Int3 UnitZ = new(0, 0, 1);
        public static readonly Int3 MaxValue = new(int.MaxValue, int.MaxValue, int.MaxValue);
        public static readonly Int3 MinValue = new(int.MinValue, int.MinValue, int.MinValue);

        public float Length => MathF.Sqrt(X * X + Y * Y + Z * Z);

        public float LengthSquared => X * X + Y * Y + Z * Z;

        #region Operators

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int3 operator +(in Int3 v1, in Int3 v2)
        {
            return new(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int3 operator -(in Int3 v1, in Int3 v2)
        {
            return new(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int3 operator *(in Int3 v1, in Int3 v2)
        {
            return new(v1.X * v2.X, v1.Y * v2.Y, v1.Z * v2.Z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int3 operator *(in Int3 v, int scalar)
        {
            return new(v.X * scalar, v.Y * scalar, v.Z * scalar);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int3 operator *(int scalar, in Int3 v)
        {
            return new(v.X * scalar, v.Y * scalar, v.Z * scalar);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int3 operator /(in Int3 v, int scalar)
        {
            return new(v.X / scalar, v.Y / scalar, v.Z / scalar);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int3 operator -(in Int3 v)
        {
            return new(-v.X, -v.Y, -v.Z);
        }

        #endregion

        #region Equality

        public static bool operator ==(in Int3 left, in Int3 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(in Int3 left, in Int3 right)
        {
            return !left.Equals(right);
        }

        public bool Equals(Int3 other)
        {
            return X == other.X && Y == other.Y && Z == other.Z;
        }

        public static bool Equals(in Int3 left, in Int3 right)
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
        public int Dot(in Int3 v)
        {
            return X * v.X + Y * v.Y + Z * v.Z;
        }

        public Int3 Cross(in Int3 v)
        {
            return new(Y * v.Z - Z * v.Y, Z * v.X - X * v.Z, X * v.Y - Y * v.X);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Dot(in Int3 v1, in Int3 v2)
        {
            return v1.X * v2.X + v1.Y * v2.Y + v1.Z * v2.Z;
        }
        
        public static float Distance(in Int3 v1, in Int3 v2)
        {
            var diffX = v1.X - v2.X;
            var diffY = v1.Y - v2.Y;
            var diffZ = v1.Z - v2.Z;
            return MathF.Sqrt(diffX * diffX + diffY * diffY + diffZ * diffZ);
        }

        public static float DistanceSquared(in Int3 v1, in Int3 v2)
        {
            var diffX = v1.X - v2.X;
            var diffY = v1.Y - v2.Y;
            var diffZ = v1.Z - v2.Z;
            return diffX * diffX + diffY * diffY + diffZ * diffZ;
        }
        
        public static Int3 Abs(in Int3 value)
        {
            return new(
                Math.Abs(value.X),
                Math.Abs(value.Y),
                Math.Abs(value.Z)
            );
        }

        public Vector3 ToBuiltIn()
        {
            return new(X, Y, Z);
        }

        public static Int3 Min(in Int3 value1, in Int3 value2)
        {
            return new(
                value1.X < value2.X ? value1.X : value2.X,
                value1.Y < value2.Y ? value1.Y : value2.Y,
                value1.Z < value2.Z ? value1.Z : value2.Z
            );
        }

        public static Int3 Max(in Int3 value1, in Int3 value2)
        {
            return new(
                value1.X > value2.X ? value1.X : value2.X,
                value1.Y > value2.Y ? value1.Y : value2.Y,
                value1.Z > value2.Z ? value1.Z : value2.Z
            );
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int3 Clamp(in Int3 value1, in Int3 min, in Int3 max)
        {
            // We must follow HLSL behavior in the case user specified min value is bigger than max value.
            return Min(Max(value1, min), max);
        }
        
        public Int3 Clamp(Int3 min, Int3 max)
        {
            var x = Math.Max(min.X, X);
            x = Math.Min(max.X, x);
            var y = Math.Max(min.Y, Y);
            y = Math.Min(max.Y, y);
            var z = Math.Max(min.Z, Z);
            z = Math.Min(max.Z, z);
            return new Int3(x, y, z);
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
            new(values.x, values.y, values.z);

        public static implicit operator (int x, int y, int z)(in Int3 v) => (v.X, v.Y, v.Z);

        public void Deconstruct(out int x, out int y, out int z) => (x, y, z) = (X, Y, Z);
    }
}