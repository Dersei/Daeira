using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Daeira.Extensions;

namespace Daeira
{
    public readonly struct Quaternion : IEquatable<Quaternion>
    {
        private const float SlerpEpsilon = 1e-6f;

        /// <summary>The X value of the vector component of the quaternion.</summary>
        public readonly float X;

        /// <summary>The Y value of the vector component of the quaternion.</summary>
        public readonly float Y;

        /// <summary>The Z value of the vector component of the quaternion.</summary>
        public readonly float Z;

        /// <summary>The rotation component of the quaternion.</summary>
        public readonly float W;

        private const int Count = 4;

        /// <summary>Constructs a quaternion from the specified components.</summary>
        /// <param name="x">The value to assign to the X component of the quaternion.</param>
        /// <param name="y">The value to assign to the Y component of the quaternion.</param>
        /// <param name="z">The value to assign to the Z component of the quaternion.</param>
        /// <param name="w">The value to assign to the W component of the quaternion.</param>
        public Quaternion(float x, float y, float z, float w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        /// <summary>Creates a quaternion from the specified vector and rotation parts.</summary>
        /// <param name="vectorPart">The vector part of the quaternion.</param>
        /// <param name="scalarPart">The rotation part of the quaternion.</param>
        public Quaternion(Float3 vectorPart, float scalarPart)
        {
            X = vectorPart.X;
            Y = vectorPart.Y;
            Z = vectorPart.Z;
            W = scalarPart;
        }

        /// <summary>Gets a quaternion that represents a zero.</summary>
        /// <value>A quaternion whose values are <c>(0, 0, 0, 0)</c>.</value>
        public static Quaternion Zero => default;

        /// <summary>Gets a quaternion that represents no rotation.</summary>
        /// <value>A quaternion whose values are <c>(0, 0, 0, 1)</c>.</value>
        public static Quaternion Identity => new(0, 0, 0, 1);

        public float this[int index] => GetElement(this, index);

        /// <summary>Gets the element at the specified index.</summary>
        /// <param name="quaternion">The vector of the element to get.</param>
        /// <param name="index">The index of the element to get.</param>
        /// <returns>The value of the element at <paramref name="index" />.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index" /> was less than zero or greater than the number of elements.</exception>
        internal static float GetElement(Quaternion quaternion, int index)
        {
            if ((uint)index >= Count)
            {
                throw new ArgumentOutOfRangeException(index.ToString());
            }

            var result = quaternion;
            return GetElementUnsafe(ref result, index);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float GetElementUnsafe(ref Quaternion quaternion, int index)
        {
            Debug.Assert(index is >= 0 and < Count);
            return Unsafe.Add(ref Unsafe.As<Quaternion, float>(ref quaternion), index);
        }


        /// <summary>Gets a value that indicates whether the current instance is the identity quaternion.</summary>
        /// <value><see langword="true" /> if the current instance is the identity quaternion; otherwise, <see langword="false" />.</value>
        /// <altmember cref="System.Numerics.Quaternion.Identity"/>
        public readonly bool IsIdentity => this == Identity;

        /// <summary>Adds each element in one quaternion with its corresponding element in a second quaternion.</summary>
        /// <param name="value1">The first quaternion.</param>
        /// <param name="value2">The second quaternion.</param>
        /// <returns>The quaternion that contains the summed values of <paramref name="value1" /> and <paramref name="value2" />.</returns>
        /// <remarks>The <see cref="System.Numerics.Quaternion.op_Addition" /> method defines the operation of the addition operator for <see cref="System.Numerics.Quaternion" /> objects.</remarks>
        public static Quaternion operator +(Quaternion value1, Quaternion value2)
        {
            return new Quaternion(
                value1.X + value2.X,
                value1.Y + value2.Y,
                value1.Z + value2.Z,
                value1.W + value2.W);
        }

        /// <summary>Divides one quaternion by a second quaternion.</summary>
        /// <param name="value1">The dividend.</param>
        /// <param name="value2">The divisor.</param>
        /// <returns>The quaternion that results from dividing <paramref name="value1" /> by <paramref name="value2" />.</returns>
        /// <remarks>The <see cref="System.Numerics.Quaternion.op_Division" /> method defines the division operation for <see cref="System.Numerics.Quaternion" /> objects.</remarks>
        public static Quaternion operator /(Quaternion value1, Quaternion value2)
        {
            var q1x = value1.X;
            var q1y = value1.Y;
            var q1z = value1.Z;
            var q1w = value1.W;

            //-------------------------------------
            // Inverse part.
            var ls = value2.X * value2.X + value2.Y * value2.Y +
                     value2.Z * value2.Z + value2.W * value2.W;
            var invNorm = 1.0f / ls;

            var q2x = -value2.X * invNorm;
            var q2y = -value2.Y * invNorm;
            var q2z = -value2.Z * invNorm;
            var q2w = value2.W * invNorm;

            //-------------------------------------
            // Multiply part.

            // cross(av, bv)
            var cx = q1y * q2z - q1z * q2y;
            var cy = q1z * q2x - q1x * q2z;
            var cz = q1x * q2y - q1y * q2x;

            var dot = q1x * q2x + q1y * q2y + q1z * q2z;

            return new Quaternion(
                q1x * q2w + q2x * q1w + cx,
                q1y * q2w + q2y * q1w + cy,
                q1z * q2w + q2z * q1w + cz,
                q1w * q2w - dot);
        }

        /// <summary>Returns a value that indicates whether two quaternions are equal.</summary>
        /// <param name="value1">The first quaternion to compare.</param>
        /// <param name="value2">The second quaternion to compare.</param>
        /// <returns><see langword="true" /> if the two quaternions are equal; otherwise, <see langword="false" />.</returns>
        /// <remarks>Two quaternions are equal if each of their corresponding components is equal.
        /// The <see cref="System.Numerics.Quaternion.op_Equality" /> method defines the operation of the equality operator for <see cref="System.Numerics.Quaternion" /> objects.</remarks>
        public static bool operator ==(Quaternion value1, Quaternion value2)
        {
            return value1.X == value2.X
                   && value1.Y == value2.Y
                   && value1.Z == value2.Z
                   && value1.W == value2.W;
        }

        /// <summary>Returns a value that indicates whether two quaternions are not equal.</summary>
        /// <param name="value1">The first quaternion to compare.</param>
        /// <param name="value2">The second quaternion to compare.</param>
        /// <returns><see langword="true" /> if <paramref name="value1" /> and <paramref name="value2" /> are not equal; otherwise, <see langword="false" />.</returns>
        public static bool operator !=(Quaternion value1, Quaternion value2)
        {
            return !(value1 == value2);
        }

        /// <summary>Returns the quaternion that results from multiplying two quaternions together.</summary>
        /// <param name="value1">The first quaternion.</param>
        /// <param name="value2">The second quaternion.</param>
        /// <returns>The product quaternion.</returns>
        /// <remarks>The <see cref="System.Numerics.Quaternion.op_Multiply" /> method defines the operation of the multiplication operator for <see cref="System.Numerics.Quaternion" /> objects.</remarks>
        public static Quaternion operator *(Quaternion value1, Quaternion value2)
        {
            var q1x = value1.X;
            var q1y = value1.Y;
            var q1z = value1.Z;
            var q1w = value1.W;

            var q2x = value2.X;
            var q2y = value2.Y;
            var q2z = value2.Z;
            var q2w = value2.W;

            // cross(av, bv)
            var cx = q1y * q2z - q1z * q2y;
            var cy = q1z * q2x - q1x * q2z;
            var cz = q1x * q2y - q1y * q2x;

            var dot = q1x * q2x + q1y * q2y + q1z * q2z;

            return new Quaternion(
                q1x * q2w + q2x * q1w + cx,
                q1y * q2w + q2y * q1w + cy,
                q1z * q2w + q2z * q1w + cz,
                q1w * q2w - dot);
        }

        /// <summary>Returns the quaternion that results from scaling all the components of a specified quaternion by a scalar factor.</summary>
        /// <param name="value1">The source quaternion.</param>
        /// <param name="value2">The scalar value.</param>
        /// <returns>The scaled quaternion.</returns>
        /// <remarks>The <see cref="System.Numerics.Quaternion.op_Multiply" /> method defines the operation of the multiplication operator for <see cref="System.Numerics.Quaternion" /> objects.</remarks>
        public static Quaternion operator *(Quaternion value1, float value2)
        {
            return new Quaternion(
                value1.X * value2,
                value1.Y * value2,
                value1.Z * value2,
                value1.W * value2);
        }

        /// <summary>Subtracts each element in a second quaternion from its corresponding element in a first quaternion.</summary>
        /// <param name="value1">The first quaternion.</param>
        /// <param name="value2">The second quaternion.</param>
        /// <returns>The quaternion containing the values that result from subtracting each element in <paramref name="value2" /> from its corresponding element in <paramref name="value1" />.</returns>
        /// <remarks>The <see cref="System.Numerics.Quaternion.op_Subtraction" /> method defines the operation of the subtraction operator for <see cref="System.Numerics.Quaternion" /> objects.</remarks>
        public static Quaternion operator -(Quaternion value1, Quaternion value2)
        {
            return new Quaternion(
                value1.X - value2.X,
                value1.Y - value2.Y,
                value1.Z - value2.Z,
                value1.W - value2.W);
        }

        /// <summary>Reverses the sign of each component of the quaternion.</summary>
        /// <param name="value">The quaternion to negate.</param>
        /// <returns>The negated quaternion.</returns>
        /// <remarks>The <see cref="System.Numerics.Quaternion.op_UnaryNegation" /> method defines the operation of the unary negation operator for <see cref="System.Numerics.Quaternion" /> objects.</remarks>
        public static Quaternion operator -(Quaternion value)
        {
            return new Quaternion(
                -value.X,
                -value.Y,
                -value.Z,
                -value.W);
        }

        /// <summary>Adds each element in one quaternion with its corresponding element in a second quaternion.</summary>
        /// <param name="value1">The first quaternion.</param>
        /// <param name="value2">The second quaternion.</param>
        /// <returns>The quaternion that contains the summed values of <paramref name="value1" /> and <paramref name="value2" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Quaternion Add(Quaternion value1, Quaternion value2)
        {
            return value1 + value2;
        }

        /// <summary>Concatenates two quaternions.</summary>
        /// <param name="value1">The first quaternion rotation in the series.</param>
        /// <param name="value2">The second quaternion rotation in the series.</param>
        /// <returns>A new quaternion representing the concatenation of the <paramref name="value1" /> rotation followed by the <paramref name="value2" /> rotation.</returns>
        public static Quaternion Concatenate(Quaternion value1, Quaternion value2)
        {
            // Concatenate rotation is actually q2 * q1 instead of q1 * q2.
            // So that's why value2 goes q1 and value1 goes q2.
            var q1x = value2.X;
            var q1y = value2.Y;
            var q1z = value2.Z;
            var q1w = value2.W;

            var q2x = value1.X;
            var q2y = value1.Y;
            var q2z = value1.Z;
            var q2w = value1.W;

            // cross(av, bv)
            var cx = q1y * q2z - q1z * q2y;
            var cy = q1z * q2x - q1x * q2z;
            var cz = q1x * q2y - q1y * q2x;

            var dot = q1x * q2x + q1y * q2y + q1z * q2z;

            return new Quaternion(
                q1x * q2w + q2x * q1w + cx,
                q1y * q2w + q2y * q1w + cy,
                q1z * q2w + q2z * q1w + cz,
                q1w * q2w - dot);
        }

        /// <summary>Returns the conjugate of a specified quaternion.</summary>
        /// <param name="value">The quaternion.</param>
        /// <returns>A new quaternion that is the conjugate of <see langword="value" />.</returns>
        public static Quaternion Conjugate(Quaternion value)
        {
            return new Quaternion(-value.X, -value.Y, -value.Z, value.W);
        }

        /// <summary>Creates a quaternion from a unit vector and an angle to rotate around the vector.</summary>
        /// <param name="axis">The unit vector to rotate around.</param>
        /// <param name="angle">The angle, in radians, to rotate around the vector.</param>
        /// <returns>The newly created quaternion.</returns>
        /// <remarks><paramref name="axis" /> vector must be normalized before calling this method or the resulting <see cref="System.Numerics.Quaternion" /> will be incorrect.</remarks>
        public static Quaternion CreateFromAxisAngle(Float3 axis, float angle)
        {
            var halfAngle = angle * 0.5f;
            var s = MathF.Sin(halfAngle);
            var c = MathF.Cos(halfAngle);

            return new Quaternion(axis.X * s, axis.Y * s, axis.Z * s, c);
        }

        /// <summary>Creates a quaternion from the specified rotation matrix.</summary>
        /// <param name="matrix">The rotation matrix.</param>
        /// <returns>The newly created quaternion.</returns>
        public static Quaternion CreateFromRotationMatrix(Matrix matrix)
        {
            var trace = matrix.M11 + matrix.M22 + matrix.M33;

            float x, y, z, w;

            if (trace > 0.0f)
            {
                var s = MathF.Sqrt(trace + 1.0f);
                w = s * 0.5f;
                s = 0.5f / s;
                x = (matrix.M23 - matrix.M32) * s;
                y = (matrix.M31 - matrix.M13) * s;
                z = (matrix.M12 - matrix.M21) * s;
            }
            else
            {
                if (matrix.M11 >= matrix.M22 && matrix.M11 >= matrix.M33)
                {
                    var s = MathF.Sqrt(1.0f + matrix.M11 - matrix.M22 - matrix.M33);
                    var invS = 0.5f / s;
                    x = 0.5f * s;
                    y = (matrix.M12 + matrix.M21) * invS;
                    z = (matrix.M13 + matrix.M31) * invS;
                    w = (matrix.M23 - matrix.M32) * invS;
                }
                else if (matrix.M22 > matrix.M33)
                {
                    var s = MathF.Sqrt(1.0f + matrix.M22 - matrix.M11 - matrix.M33);
                    var invS = 0.5f / s;
                    x = (matrix.M21 + matrix.M12) * invS;
                    y = 0.5f * s;
                    z = (matrix.M32 + matrix.M23) * invS;
                    w = (matrix.M31 - matrix.M13) * invS;
                }
                else
                {
                    var s = MathF.Sqrt(1.0f + matrix.M33 - matrix.M11 - matrix.M22);
                    var invS = 0.5f / s;
                    x = (matrix.M31 + matrix.M13) * invS;
                    y = (matrix.M32 + matrix.M23) * invS;
                    z = 0.5f * s;
                    w = (matrix.M12 - matrix.M21) * invS;
                }
            }

            return new Quaternion(x, y, z, w);
        }

        /// <summary>Creates a new quaternion from the given yaw, pitch, and roll.</summary>
        /// <param name="yaw">The yaw angle, in radians, around the Y axis.</param>
        /// <param name="pitch">The pitch angle, in radians, around the X axis.</param>
        /// <param name="roll">The roll angle, in radians, around the Z axis.</param>
        /// <returns>The resulting quaternion.</returns>
        public static Quaternion CreateFromYawPitchRoll(float yaw, float pitch, float roll)
        {
            //  Roll first, about axis the object is facing, then
            //  pitch upward, then yaw to face into the new heading
            float sr, cr, sp, cp, sy, cy;

            var halfRoll = roll * 0.5f;
            sr = MathF.Sin(halfRoll);
            cr = MathF.Cos(halfRoll);

            var halfPitch = pitch * 0.5f;
            sp = MathF.Sin(halfPitch);
            cp = MathF.Cos(halfPitch);

            var halfYaw = yaw * 0.5f;
            sy = MathF.Sin(halfYaw);
            cy = MathF.Cos(halfYaw);

            return new Quaternion(
                cy * sp * cr + sy * cp * sr,
                sy * cp * cr - cy * sp * sr,
                cy * cp * sr - sy * sp * cr,
                cy * cp * cr + sy * sp * sr);
        }

        /// <summary>Divides one quaternion by a second quaternion.</summary>
        /// <param name="value1">The dividend.</param>
        /// <param name="value2">The divisor.</param>
        /// <returns>The quaternion that results from dividing <paramref name="value1" /> by <paramref name="value2" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Quaternion Divide(Quaternion value1, Quaternion value2)
        {
            return value1 / value2;
        }

        /// <summary>Calculates the dot product of two quaternions.</summary>
        /// <param name="quaternion1">The first quaternion.</param>
        /// <param name="quaternion2">The second quaternion.</param>
        /// <returns>The dot product.</returns>
        public static float Dot(Quaternion quaternion1, Quaternion quaternion2)
        {
            return quaternion1.X * quaternion2.X +
                   quaternion1.Y * quaternion2.Y +
                   quaternion1.Z * quaternion2.Z +
                   quaternion1.W * quaternion2.W;
        }

        /// <summary>Returns the inverse of a quaternion.</summary>
        /// <param name="value">The quaternion.</param>
        /// <returns>The inverted quaternion.</returns>
        public static Quaternion Inverse(Quaternion value)
        {
            //  -1   (       a              -v       )
            // q   = ( -------------   ------------- )
            //       (  a^2 + |v|^2  ,  a^2 + |v|^2  )

            var ls = value.X * value.X + value.Y * value.Y + value.Z * value.Z + value.W * value.W;
            var invNorm = 1.0f / ls;

            return new Quaternion(-value.X * invNorm, -value.Y * invNorm, -value.Z * invNorm, value.W * invNorm);
        }

        /// <summary>Performs a linear interpolation between two quaternions based on a value that specifies the weighting of the second quaternion.</summary>
        /// <param name="quaternion1">The first quaternion.</param>
        /// <param name="quaternion2">The second quaternion.</param>
        /// <param name="amount">The relative weight of <paramref name="quaternion2" /> in the interpolation.</param>
        /// <returns>The interpolated quaternion.</returns>
        public static Quaternion Lerp(Quaternion quaternion1, Quaternion quaternion2, float amount)
        {
            var t = amount;
            var t1 = 1.0f - t;

            var dot = quaternion1.X * quaternion2.X + quaternion1.Y * quaternion2.Y +
                      quaternion1.Z * quaternion2.Z + quaternion1.W * quaternion2.W;
            float x, y, z, w;
            if (dot >= 0.0f)
            {
                x = t1 * quaternion1.X + t * quaternion2.X;
                y = t1 * quaternion1.Y + t * quaternion2.Y;
                z = t1 * quaternion1.Z + t * quaternion2.Z;
                w = t1 * quaternion1.W + t * quaternion2.W;
            }
            else
            {
                x = t1 * quaternion1.X - t * quaternion2.X;
                y = t1 * quaternion1.Y - t * quaternion2.Y;
                z = t1 * quaternion1.Z - t * quaternion2.Z;
                w = t1 * quaternion1.W - t * quaternion2.W;
            }

            // Normalize it.
            var ls = x * x + y * y + z * z + w * w;
            var invNorm = 1.0f / MathF.Sqrt(ls);

            x *= invNorm;
            y *= invNorm;
            z *= invNorm;
            w *= invNorm;

            return new Quaternion(x, y, z, w);
        }

        /// <summary>Returns the quaternion that results from multiplying two quaternions together.</summary>
        /// <param name="value1">The first quaternion.</param>
        /// <param name="value2">The second quaternion.</param>
        /// <returns>The product quaternion.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Quaternion Multiply(Quaternion value1, Quaternion value2)
        {
            return value1 * value2;
        }

        /// <summary>Returns the quaternion that results from scaling all the components of a specified quaternion by a scalar factor.</summary>
        /// <param name="value1">The source quaternion.</param>
        /// <param name="value2">The scalar value.</param>
        /// <returns>The scaled quaternion.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Quaternion Multiply(Quaternion value1, float value2)
        {
            return value1 * value2;
        }

        /// <summary>Reverses the sign of each component of the quaternion.</summary>
        /// <param name="value">The quaternion to negate.</param>
        /// <returns>The negated quaternion.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Quaternion Negate(Quaternion value)
        {
            return -value;
        }

        /// <summary>Divides each component of a specified <see cref="System.Numerics.Quaternion" /> by its length.</summary>
        /// <param name="value">The quaternion to normalize.</param>
        /// <returns>The normalized quaternion.</returns>
        public static Quaternion Normalize(Quaternion value)
        {
            var ls = value.X * value.X + value.Y * value.Y + value.Z * value.Z + value.W * value.W;

            var invNorm = 1.0f / MathF.Sqrt(ls);

            return new Quaternion(value.X * invNorm, value.Y * invNorm, value.Z * invNorm, value.W * invNorm);
        }

        /// <summary>Interpolates between two quaternions, using spherical linear interpolation.</summary>
        /// <param name="quaternion1">The first quaternion.</param>
        /// <param name="quaternion2">The second quaternion.</param>
        /// <param name="amount">The relative weight of the second quaternion in the interpolation.</param>
        /// <returns>The interpolated quaternion.</returns>
        public static Quaternion Slerp(Quaternion quaternion1, Quaternion quaternion2, float amount)
        {
            var t = amount;

            var cosOmega = quaternion1.X * quaternion2.X + quaternion1.Y * quaternion2.Y +
                           quaternion1.Z * quaternion2.Z + quaternion1.W * quaternion2.W;

            var flip = false;

            if (cosOmega < 0.0f)
            {
                flip = true;
                cosOmega = -cosOmega;
            }

            float s1, s2;

            if (cosOmega > 1.0f - SlerpEpsilon)
            {
                // Too close, do straight linear interpolation.
                s1 = 1.0f - t;
                s2 = flip ? -t : t;
            }
            else
            {
                var omega = MathF.Acos(cosOmega);
                var invSinOmega = 1 / MathF.Sin(omega);

                s1 = MathF.Sin((1.0f - t) * omega) * invSinOmega;
                s2 = flip
                    ? -MathF.Sin(t * omega) * invSinOmega
                    : MathF.Sin(t * omega) * invSinOmega;
            }

            return new Quaternion(
                s1 * quaternion1.X + s2 * quaternion2.X,
                s1 * quaternion1.Y + s2 * quaternion2.Y,
                s1 * quaternion1.Z + s2 * quaternion2.Z,
                s1 * quaternion1.W + s2 * quaternion2.W);
        }

        /// <summary>Subtracts each element in a second quaternion from its corresponding element in a first quaternion.</summary>
        /// <param name="value1">The first quaternion.</param>
        /// <param name="value2">The second quaternion.</param>
        /// <returns>The quaternion containing the values that result from subtracting each element in <paramref name="value2" /> from its corresponding element in <paramref name="value1" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Quaternion Subtract(Quaternion value1, Quaternion value2)
        {
            return value1 - value2;
        }

        /// <summary>Returns a value that indicates whether this instance and a specified object are equal.</summary>
        /// <param name="obj">The object to compare with the current instance.</param>
        /// <returns><see langword="true" /> if the current instance and <paramref name="obj" /> are equal; otherwise, <see langword="false" />. If <paramref name="obj" /> is <see langword="null" />, the method returns <see langword="false" />.</returns>
        /// <remarks>The current instance and <paramref name="obj" /> are equal if <paramref name="obj" /> is a <see cref="System.Numerics.Quaternion" /> object and the corresponding components of each matrix are equal.</remarks>
        public override readonly bool Equals([NotNullWhen(true)] object? obj)
        {
            return obj is Quaternion other && Equals(other);
        }

        /// <summary>Returns a value that indicates whether this instance and another quaternion are equal.</summary>
        /// <param name="other">The other quaternion.</param>
        /// <returns><see langword="true" /> if the two quaternions are equal; otherwise, <see langword="false" />.</returns>
        /// <remarks>Two quaternions are equal if each of their corresponding components is equal.</remarks>
        public readonly bool Equals(Quaternion other)
        {
            return this == other;
        }

        /// <summary>Returns the hash code for this instance.</summary>
        /// <returns>The hash code.</returns>
        public override readonly int GetHashCode()
        {
            return unchecked(X.GetHashCode() + Y.GetHashCode() + Z.GetHashCode() + W.GetHashCode());
        }

        /// <summary>Calculates the length of the quaternion.</summary>
        /// <returns>The computed length of the quaternion.</returns>
        public readonly float Length() => MathF.Sqrt(X * X + Y * Y + Z * Z + W * W);

        /// <summary>Calculates the squared length of the quaternion.</summary>
        /// <returns>The length squared of the quaternion.</returns>
        public readonly float LengthSquared() => X * X + Y * Y + Z * Z + W * W;

        /// <summary>Returns a string that represents this quaternion.</summary>
        /// <returns>The string representation of this quaternion.</returns>
        /// <remarks>The numeric values in the returned string are formatted by using the conventions of the current culture. For example, for the en-US culture, the returned string might appear as <c>{X:1.1 Y:2.2 Z:3.3 W:4.4}</c>.</remarks>
        public override readonly string ToString() =>
            $"{{X:{X} Y:{Y} Z:{Z} W:{W}}}";

        public static Quaternion AngleAxis(float aAngle, Float3 aAxis)
        {
            aAxis.Normalize();
            var rad = aAngle * MathExtensions.Deg2Rad * 0.5f;
            aAxis *= MathF.Sin(rad);
            return new Quaternion(aAxis.X, aAxis.Y, aAxis.Z, MathF.Cos(rad));
        }

        public static Float3 Rotate(Quaternion rotation, Float3 point)
        {
            var x = rotation.X * 2F;
            var y = rotation.Y * 2F;
            var z = rotation.Z * 2F;
            var xx = rotation.X * x;
            var yy = rotation.Y * y;
            var zz = rotation.Z * z;
            var xy = rotation.X * y;
            var xz = rotation.X * z;
            var yz = rotation.Y * z;
            var wx = rotation.W * x;
            var wy = rotation.W * y;
            var wz = rotation.W * z;

            var res = new Float3((1F - (yy + zz)) * point.X + (xy - wz) * point.Y + (xz + wy) * point.Z,
                (xy + wz) * point.X + (1F - (xx + zz)) * point.Y + (yz - wx) * point.Z,
                (xz - wy) * point.X + (yz + wx) * point.Y + (1F - (xx + yy)) * point.Z);
            return res;
        }
    }
}