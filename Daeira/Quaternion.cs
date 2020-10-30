using System;
using System.Globalization;
using Daeira.Extensions;

namespace Daeira
{
    public readonly struct Quaternion : IEquatable<Quaternion>, IFormattable
    {
        // X component of the Quaternion. Don't modify this directly unless you know quaternions inside out.
        public readonly float X;

        // Y component of the Quaternion. Don't modify this directly unless you know quaternions inside out.
        public readonly float Y;

        // Z component of the Quaternion. Don't modify this directly unless you know quaternions inside out.
        public readonly float Z;

        // W component of the Quaternion. Don't modify this directly unless you know quaternions inside out.
        public readonly float W;

        public float this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return X;
                    case 1: return Y;
                    case 2: return Z;
                    case 3: return W;
                    default:
                        throw new IndexOutOfRangeException("Invalid Quaternion index!");
                }
            }
        }

        public Quaternion(float x, float y, float z, float w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }


        private const float KEpsilon = 0.000001F;

        // Is the dot product of two quaternions within tolerance for them to be considered equal?
        private static bool IsEqualUsingDot(float dot)
        {
            // Returns false in the presence of NaN values.
            return dot > 1.0f - KEpsilon;
        }

        // Are two quaternions equal to each other?
        public static bool operator ==(Quaternion lhs, Quaternion rhs)
        {
            return IsEqualUsingDot(Dot(lhs, rhs));
        }

        // Are two quaternions different from each other?
        public static bool operator !=(Quaternion lhs, Quaternion rhs)
        {
            // Returns true in the presence of NaN values.
            return !(lhs == rhs);
        }

        public static float Dot(Quaternion quaternion1, Quaternion quaternion2)
        {
            return quaternion1.X * quaternion2.X +
                   quaternion1.Y * quaternion2.Y +
                   quaternion1.Z * quaternion2.Z +
                   quaternion1.W * quaternion2.W;
        }

        public static float Angle(Quaternion a, Quaternion b)
        {
            var dot = Dot(a, b);
            return IsEqualUsingDot(dot)
                ? 0.0f
                : MathF.Acos(MathF.Min(MathF.Abs(dot), 1.0F)) * 2.0F * MathExtensions.Rad2Deg;
        }

        public static Quaternion operator +(Quaternion value1, Quaternion value2)
        {
            return new Quaternion(value1.X + value2.X, value1.Y + value2.Y, value1.Z + value2.Z, value1.W + value2.W);
        }

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

            var ansX = q1x * q2w + q2x * q1w + cx;
            var ansY = q1y * q2w + q2y * q1w + cy;
            var ansZ = q1z * q2w + q2z * q1w + cz;
            var ansW = q1w * q2w - dot;

            return new Quaternion(ansX, ansY, ansZ, ansW);
        }

        public static Quaternion operator *(Quaternion value1, Quaternion value2)
        {
            Quaternion ans;

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

            var ansX = q1x * q2w + q2x * q1w + cx;
            var ansY = q1y * q2w + q2y * q1w + cy;
            var ansZ = q1z * q2w + q2z * q1w + cz;
            var ansW = q1w * q2w - dot;

            return new Quaternion(ansX, ansY, ansZ, ansW);
        }

        public static Quaternion operator *(Quaternion value1, float value2)
        {
            Quaternion ans;

            var ansX = value1.X * value2;
            var ansY = value1.Y * value2;
            var ansZ = value1.Z * value2;
            var ansW = value1.W * value2;

            return new Quaternion(ansX, ansY, ansZ, ansW);
        }

        public static Quaternion operator -(Quaternion value1, Quaternion value2)
        {
            Quaternion ans;

            var ansX = value1.X - value2.X;
            var ansY = value1.Y - value2.Y;
            var ansZ = value1.Z - value2.Z;
            var ansW = value1.W - value2.W;

            return new Quaternion(ansX, ansY, ansZ, ansW);
        }

        public static Quaternion operator -(Quaternion value)
        {
            Quaternion ans;

            var ansX = -value.X;
            var ansY = -value.Y;
            var ansZ = -value.Z;
            var ansW = -value.W;

            return new Quaternion(ansX, ansY, ansZ, ansW);
        }

        /// <summary>Concatenates two Quaternions; the result represents the value1 rotation followed by the value2 rotation.</summary>
        /// <param name="value1">The first Quaternion rotation in the series.</param>
        /// <param name="value2">The second Quaternion rotation in the series.</param>
        /// <returns>A new Quaternion representing the concatenation of the value1 rotation followed by the value2 rotation.</returns>
        public static Quaternion Concatenate(Quaternion value1, Quaternion value2)
        {
            Quaternion ans;

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

            var ansX = q1x * q2w + q2x * q1w + cx;
            var ansY = q1y * q2w + q2y * q1w + cy;
            var ansZ = q1z * q2w + q2z * q1w + cz;
            var ansW = q1w * q2w - dot;

            return new Quaternion(ansX, ansY, ansZ, ansW);
        }

        /// <summary>Creates the conjugate of a specified Quaternion.</summary>
        /// <param name="value">The Quaternion of which to return the conjugate.</param>
        /// <returns>A new Quaternion that is the conjugate of the specified one.</returns>
        public static Quaternion Conjugate(Quaternion value)
        {
            Quaternion ans;

            var ansX = -value.X;
            var ansY = -value.Y;
            var ansZ = -value.Z;
            var ansW = value.W;

            return new Quaternion(ansX, ansY, ansZ, ansW);
        }

        /// <summary>Creates a Quaternion from a normalized vector axis and an angle to rotate about the vector.</summary>
        /// <param name="axis">The unit vector to rotate around.
        /// This vector must be normalized before calling this function or the resulting Quaternion will be incorrect.</param>
        /// <param name="angle">The angle, in radians, to rotate around the vector.</param>
        /// <returns>The created Quaternion.</returns>
        public static Quaternion CreateFromAxisAngle(Float3 axis, float angle)
        {
            Quaternion ans;

            var halfAngle = angle * 0.5f;
            var s = MathF.Sin(halfAngle);
            var c = MathF.Cos(halfAngle);

            var ansX = axis.X * s;
            var ansY = axis.Y * s;
            var ansZ = axis.Z * s;
            var ansW = c;

            return new Quaternion(ansX, ansY, ansZ, ansW);
        }

        /// <summary>Creates a new Quaternion from the given yaw, pitch, and roll, in radivar ans</summary>
        /// <param name="yaw">The yaw angle, in radians, around the Y-axis.</param>
        /// <param name="pitch">The pitch angle, in radians, around the X-axis.</param>
        /// <param name="roll">The roll angle, in radians, around the Z-axis.</param>
        /// <returns></returns>
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

            Quaternion result;

            var resultX = cy * sp * cr + sy * cp * sr;
            var resultY = sy * cp * cr - cy * sp * sr;
            var resultZ = cy * cp * sr - sy * sp * cr;
            var resultW = cy * cp * cr + sy * sp * sr;

            return new Quaternion(resultX, resultY, resultZ, resultW);
        }


        /// <summary>Returns the inverse of a Quaternion.</summary>
        /// <param name="value">The source Quaternion.</param>
        /// <returns>The inverted Quaternion.</returns>
        public static Quaternion Inverse(Quaternion value)
        {
            //  -1   (       a              -v       )
            // q   = ( -------------   ------------- )
            //       (  a^2 + |v|^2  ,  a^2 + |v|^2  )

            Quaternion ans;

            var ls = value.X * value.X + value.Y * value.Y + value.Z * value.Z + value.W * value.W;
            var invNorm = 1.0f / ls;

            var ansX = -value.X * invNorm;
            var ansY = -value.Y * invNorm;
            var ansZ = -value.Z * invNorm;
            var ansW = value.W * invNorm;

            return new Quaternion(ansX, ansY, ansZ, ansW);
        }


        /// <summary> Linearly interpolates between two quaternions.</summary>
        /// <param name="quaternion1">The first source Quaternion.</param>
        /// <param name="quaternion2">The second source Quaternion.</param>
        /// <param name="amount">The relative weight of the second source Quaternion in the interpolation.</param>
        /// <returns>The interpolated Quaternion.</returns>
        public static Quaternion Lerp(Quaternion quaternion1, Quaternion quaternion2, float amount)
        {
            var t = amount;
            var t1 = 1.0f - t;

            Quaternion r = default;

            var dot = quaternion1.X * quaternion2.X + quaternion1.Y * quaternion2.Y +
                      quaternion1.Z * quaternion2.Z + quaternion1.W * quaternion2.W;
            var rX = 0f;
            var rY = 0f;
            var rZ = 0f;
            var rW = 0f;
            if (dot >= 0.0f)
            {
                rX = t1 * quaternion1.X + t * quaternion2.X;
                rY = t1 * quaternion1.Y + t * quaternion2.Y;
                rZ = t1 * quaternion1.Z + t * quaternion2.Z;
                rW = t1 * quaternion1.W + t * quaternion2.W;
            }
            else
            {
                rX = t1 * quaternion1.X - t * quaternion2.X;
                rY = t1 * quaternion1.Y - t * quaternion2.Y;
                rZ = t1 * quaternion1.Z - t * quaternion2.Z;
                rW = t1 * quaternion1.W - t * quaternion2.W;
            }

            // Normalize it.
            var ls = rX * rX + rY * rY + rZ * rZ + rW * rW;
            var invNorm = 1.0f / MathF.Sqrt(ls);

            rX *= invNorm;
            rY *= invNorm;
            rZ *= invNorm;
            rW *= invNorm;

            return new Quaternion(rX, rY, rZ, rW);
        }

        /// <summary>Divides each component of the Quaternion by the length of the Quaternion.</summary>
        /// <param name="value">The source Quaternion.</param>
        /// <returns>The normalized Quaternion.</returns>
        public static Quaternion Normalize(Quaternion value)
        {
            Quaternion ans;

            var ls = value.X * value.X + value.Y * value.Y + value.Z * value.Z + value.W * value.W;

            var invNorm = 1.0f / MathF.Sqrt(ls);

            var ansX = value.X * invNorm;
            var ansY = value.Y * invNorm;
            var ansZ = value.Z * invNorm;
            var ansW = value.W * invNorm;

            return new Quaternion(ansX, ansY, ansZ, ansW);
        }

        private const float SlerpEpsilon = 1e-6f;

        /// <summary>Interpolates between two quaternions, using spherical linear interpolation.</summary>
        /// <param name="quaternion1">The first source Quaternion.</param>
        /// <param name="quaternion2">The second source Quaternion.</param>
        /// <param name="amount">The relative weight of the second source Quaternion in the interpolation.</param>
        /// <returns>The interpolated Quaternion.</returns>
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

            if (cosOmega > (1.0f - SlerpEpsilon))
            {
                // Too close, do straight linear interpolation.
                s1 = 1.0f - t;
                s2 = (flip) ? -t : t;
            }
            else
            {
                var omega = MathF.Acos(cosOmega);
                var invSinOmega = 1 / MathF.Sin(omega);

                s1 = MathF.Sin((1.0f - t) * omega) * invSinOmega;
                s2 = (flip)
                    ? -MathF.Sin(t * omega) * invSinOmega
                    : MathF.Sin(t * omega) * invSinOmega;
            }

            Quaternion ans;

            var ansX = s1 * quaternion1.X + s2 * quaternion2.X;
            var ansY = s1 * quaternion1.Y + s2 * quaternion2.Y;
            var ansZ = s1 * quaternion1.Z + s2 * quaternion2.Z;
            var ansW = s1 * quaternion1.W + s2 * quaternion2.W;

            return new Quaternion(ansX, ansY, ansZ, ansW);
        }


        /// <summary>Calculates the length of the Quaternion.</summary>
        /// <returns>The computed length of the Quaternion.</returns>
        public readonly float Length()
        {
            var lengthSquared = LengthSquared();
            return MathF.Sqrt(lengthSquared);
        }

        /// <summary>Calculates the length squared of the Quaternion. This operation is cheaper than Length().</summary>
        /// <returns>The length squared of the Quaternion.</returns>
        public readonly float LengthSquared()
        {
            return X * X + Y * Y + Z * Z + W * W;
        }

        // also required for being able to use Quaternions as keys in hash tables
        public override bool Equals(object? other)
        {
            return other is Quaternion quaternion && Equals(quaternion);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y, Z, W);
        }

        public bool Equals(Quaternion other)
        {
            return X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z) && W.Equals(other.W);
        }

        public override string ToString()
        {
            return ToString(null, CultureInfo.InvariantCulture.NumberFormat);
        }

        public string ToString(string format)
        {
            return ToString(format, CultureInfo.InvariantCulture.NumberFormat);
        }

        public string ToString(string? format, IFormatProvider? formatProvider)
        {
            if (string.IsNullOrEmpty(format))
                format = "F1";
            return
                $"Q({X.ToString(format, formatProvider)}, {Y.ToString(format, formatProvider)}, {Z.ToString(format, formatProvider)}, {W.ToString(format, formatProvider)})";
        }
    }
}