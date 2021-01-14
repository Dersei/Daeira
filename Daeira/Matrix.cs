using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.Arm;
using System.Runtime.Intrinsics.X86;
using Daeira.Extensions;

namespace Daeira
{
    public struct Matrix : IEquatable<Matrix>, IFormattable
    {
        public float M11;
        public float M21;
        public float M31;
        public float M41;
        /// <summary>
        /// First row, second column
        /// </summary>
        public float M12;
        public float M22;
        public float M32;
        public float M42;
        public float M13;
        public float M23;
        public float M33;
        public float M43;
        public float M14;
        public float M24;
        public float M34;
        public float M44;

        public Matrix(Float4 column1, Float4 column2, Float4 column3, Float4 column4)
        {
            M11 = column1.X;
            M12 = column2.X;
            M13 = column3.X;
            M14 = column4.X;
            M21 = column1.Y;
            M22 = column2.Y;
            M23 = column3.Y;
            M24 = column4.Y;
            M31 = column1.Z;
            M32 = column2.Z;
            M33 = column3.Z;
            M34 = column4.Z;
            M41 = column1.W;
            M42 = column2.W;
            M43 = column3.W;
            M44 = column4.W;
        }

        public Matrix(float m11, float m12, float m13, float m14, float m21, float m22, float m23, float m24, float m31,
            float m32, float m33, float m34, float m41, float m42, float m43, float m44)
        {
            M11 = m11;
            M21 = m21;
            M31 = m31;
            M41 = m41;
            M12 = m12;
            M22 = m22;
            M32 = m32;
            M42 = m42;
            M13 = m13;
            M23 = m23;
            M33 = m33;
            M43 = m43;
            M14 = m14;
            M24 = m24;
            M34 = m34;
            M44 = m44;
        }

        public Float4 GetColumn(int index)
        {
            switch (index)
            {
                case 0: return new Float4(M11, M21, M31, M41);
                case 1: return new Float4(M12, M22, M32, M42);
                case 2: return new Float4(M13, M23, M33, M43);
                case 3: return new Float4(M14, M24, M34, M44);
                default:
                    throw new IndexOutOfRangeException("Invalid column index!");
            }
        }

        // Returns a row of the matrix.
        public Float4 GetRow(int index)
        {
            switch (index)
            {
                case 0: return new Float4(M11, M12, M13, M14);
                case 1: return new Float4(M21, M22, M23, M24);
                case 2: return new Float4(M31, M32, M33, M34);
                case 3: return new Float4(M41, M42, M43, M44);
                default:
                    throw new IndexOutOfRangeException("Invalid row index!");
            }
        }

        public void SetColumn(int index, Float4 column)
        {
            this[0, index] = column.X;
            this[1, index] = column.Y;
            this[2, index] = column.Z;
            this[3, index] = column.W;
        }

        // Sets a row of the matrix.
        public void SetRow(int index, Float4 row)
        {
            this[index, 0] = row.X;
            this[index, 1] = row.Y;
            this[index, 2] = row.Z;
            this[index, 3] = row.W;
        }

        public override bool Equals(object? other)
        {
            return other is Matrix matrix && Equals(matrix);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(GetColumn(0).GetHashCode(), GetColumn(1).GetHashCode(), GetColumn(2).GetHashCode(),
                GetColumn(3).GetHashCode());
        }

        public float this[int index]
        {
            get
            {
                return index switch
                {
                    0 => M11,
                    1 => M21,
                    2 => M31,
                    3 => M41,
                    4 => M12,
                    5 => M22,
                    6 => M32,
                    7 => M42,
                    8 => M13,
                    9 => M23,
                    10 => M33,
                    11 => M43,
                    12 => M14,
                    13 => M24,
                    14 => M34,
                    15 => M44,
                    _ => throw new IndexOutOfRangeException("Invalid matrix index!")
                };
            }
            set
            {
                switch (index)
                {
                    case 0:
                        M11 = value;
                        break;
                    case 1:
                        M21 = value;
                        break;
                    case 2:
                        M31 = value;
                        break;
                    case 3:
                        M41 = value;
                        break;
                    case 4:
                        M12 = value;
                        break;
                    case 5:
                        M22 = value;
                        break;
                    case 6:
                        M32 = value;
                        break;
                    case 7:
                        M42 = value;
                        break;
                    case 8:
                        M13 = value;
                        break;
                    case 9:
                        M23 = value;
                        break;
                    case 10:
                        M33 = value;
                        break;
                    case 11:
                        M43 = value;
                        break;
                    case 12:
                        M14 = value;
                        break;
                    case 13:
                        M24 = value;
                        break;
                    case 14:
                        M34 = value;
                        break;
                    case 15:
                        M44 = value;
                        break;

                    default:
                        throw new IndexOutOfRangeException("Invalid matrix index!");
                }
            }
        }

        public float this[int row, int column]
        {
            get => this[row + column * 4];
            set => this[row + column * 4] = value;
        }

        public bool Equals(Matrix other)
        {
            return GetColumn(0).Equals(other.GetColumn(0))
                   && GetColumn(1).Equals(other.GetColumn(1))
                   && GetColumn(2).Equals(other.GetColumn(2))
                   && GetColumn(3).Equals(other.GetColumn(3));
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
                format = "F5";
            return
                @$"{M11.ToString(format, formatProvider)}    {M12.ToString(format, formatProvider)}    {M13.ToString(format, formatProvider)}    {M14.ToString(format, formatProvider)}
{M21.ToString(format, formatProvider)}        {M22.ToString(format, formatProvider)}    {M23.ToString(format, formatProvider)}    {M24.ToString(format, formatProvider)}
{M31.ToString(format, formatProvider)}    {M32.ToString(format, formatProvider)}    {M33.ToString(format, formatProvider)}    {M34.ToString(format, formatProvider)}
{M41.ToString(format, formatProvider)}    {M42.ToString(format, formatProvider)}    {M43.ToString(format, formatProvider)}    {M44.ToString(format, formatProvider)}{Environment.NewLine}";
        }

        public static unsafe Matrix operator *(Matrix left, Matrix right)
        {
            if (AdvSimd.Arm64.IsSupported)
            {
                Unsafe.SkipInit(out Matrix result);

                // Perform the operation on the first row

                var m11 = AdvSimd.LoadVector128(&left.M11);

                var vX = AdvSimd.MultiplyBySelectedScalar(AdvSimd.LoadVector128(&right.M11), m11, 0);
                var vY = AdvSimd.MultiplyBySelectedScalar(AdvSimd.LoadVector128(&right.M12), m11, 1);
                var vZ =
                    AdvSimd.Arm64.FusedMultiplyAddBySelectedScalar(vX, AdvSimd.LoadVector128(&right.M13), m11, 2);
                var vW =
                    AdvSimd.Arm64.FusedMultiplyAddBySelectedScalar(vY, AdvSimd.LoadVector128(&right.M14), m11, 3);

                AdvSimd.Store(&result.M11, AdvSimd.Add(vZ, vW));

                // Repeat for the other 3 rows

                var m12 = AdvSimd.LoadVector128(&left.M12);

                vX = AdvSimd.MultiplyBySelectedScalar(AdvSimd.LoadVector128(&right.M11), m12, 0);
                vY = AdvSimd.MultiplyBySelectedScalar(AdvSimd.LoadVector128(&right.M12), m12, 1);
                vZ = AdvSimd.Arm64.FusedMultiplyAddBySelectedScalar(vX, AdvSimd.LoadVector128(&right.M13), m12, 2);
                vW = AdvSimd.Arm64.FusedMultiplyAddBySelectedScalar(vY, AdvSimd.LoadVector128(&right.M14), m12, 3);

                AdvSimd.Store(&result.M21, AdvSimd.Add(vZ, vW));

                var m13 = AdvSimd.LoadVector128(&left.M13);

                vX = AdvSimd.MultiplyBySelectedScalar(AdvSimd.LoadVector128(&right.M11), m13, 0);
                vY = AdvSimd.MultiplyBySelectedScalar(AdvSimd.LoadVector128(&right.M12), m13, 1);
                vZ = AdvSimd.Arm64.FusedMultiplyAddBySelectedScalar(vX, AdvSimd.LoadVector128(&right.M13), m13, 2);
                vW = AdvSimd.Arm64.FusedMultiplyAddBySelectedScalar(vY, AdvSimd.LoadVector128(&right.M14), m13, 3);

                AdvSimd.Store(&result.M13, AdvSimd.Add(vZ, vW));

                var m14 = AdvSimd.LoadVector128(&left.M14);

                vX = AdvSimd.MultiplyBySelectedScalar(AdvSimd.LoadVector128(&right.M11), m14, 0);
                vY = AdvSimd.MultiplyBySelectedScalar(AdvSimd.LoadVector128(&right.M12), m14, 1);
                vZ = AdvSimd.Arm64.FusedMultiplyAddBySelectedScalar(vX, AdvSimd.LoadVector128(&right.M13), m14, 2);
                vW = AdvSimd.Arm64.FusedMultiplyAddBySelectedScalar(vY, AdvSimd.LoadVector128(&right.M14), m14, 3);

                AdvSimd.Store(&result.M14, AdvSimd.Add(vZ, vW));

                return result;
            }

            if (Sse.IsSupported)
            {
                var row = Sse.LoadVector128(&left.M11);
                Sse.Store(&left.M11,
                    Sse.Add(Sse.Add(Sse.Multiply(Sse.Shuffle(row, row, 0x00), Sse.LoadVector128(&right.M11)),
                            Sse.Multiply(Sse.Shuffle(row, row, 0x55), Sse.LoadVector128(&right.M12))),
                        Sse.Add(Sse.Multiply(Sse.Shuffle(row, row, 0xAA), Sse.LoadVector128(&right.M13)),
                            Sse.Multiply(Sse.Shuffle(row, row, 0xFF), Sse.LoadVector128(&right.M14)))));

                // 0x00 is _MM_SHUFFLE(0,0,0,0), 0x55 is _MM_SHUFFLE(1,1,1,1), etc.
                // TODO: Replace with a method once it's added to the API.

                row = Sse.LoadVector128(&left.M12);
                Sse.Store(&left.M12,
                    Sse.Add(Sse.Add(Sse.Multiply(Sse.Shuffle(row, row, 0x00), Sse.LoadVector128(&right.M11)),
                            Sse.Multiply(Sse.Shuffle(row, row, 0x55), Sse.LoadVector128(&right.M12))),
                        Sse.Add(Sse.Multiply(Sse.Shuffle(row, row, 0xAA), Sse.LoadVector128(&right.M13)),
                            Sse.Multiply(Sse.Shuffle(row, row, 0xFF), Sse.LoadVector128(&right.M14)))));

                row = Sse.LoadVector128(&left.M13);
                Sse.Store(&left.M13,
                    Sse.Add(Sse.Add(Sse.Multiply(Sse.Shuffle(row, row, 0x00), Sse.LoadVector128(&right.M11)),
                            Sse.Multiply(Sse.Shuffle(row, row, 0x55), Sse.LoadVector128(&right.M12))),
                        Sse.Add(Sse.Multiply(Sse.Shuffle(row, row, 0xAA), Sse.LoadVector128(&right.M13)),
                            Sse.Multiply(Sse.Shuffle(row, row, 0xFF), Sse.LoadVector128(&right.M14)))));

                row = Sse.LoadVector128(&left.M14);
                Sse.Store(&left.M14,
                    Sse.Add(Sse.Add(Sse.Multiply(Sse.Shuffle(row, row, 0x00), Sse.LoadVector128(&right.M11)),
                            Sse.Multiply(Sse.Shuffle(row, row, 0x55), Sse.LoadVector128(&right.M12))),
                        Sse.Add(Sse.Multiply(Sse.Shuffle(row, row, 0xAA), Sse.LoadVector128(&right.M13)),
                            Sse.Multiply(Sse.Shuffle(row, row, 0xFF), Sse.LoadVector128(&right.M14)))));
                return left;
            }

            Matrix res;
            res.M11 = left.M11 * right.M11 + left.M12 * right.M21 + left.M13 * right.M31 + left.M14 * right.M41;
            res.M12 = left.M11 * right.M12 + left.M12 * right.M22 + left.M13 * right.M32 + left.M14 * right.M42;
            res.M13 = left.M11 * right.M13 + left.M12 * right.M23 + left.M13 * right.M33 + left.M14 * right.M43;
            res.M14 = left.M11 * right.M14 + left.M12 * right.M24 + left.M13 * right.M34 + left.M14 * right.M44;

            res.M21 = left.M21 * right.M11 + left.M22 * right.M21 + left.M23 * right.M31 + left.M24 * right.M41;
            res.M22 = left.M21 * right.M12 + left.M22 * right.M22 + left.M23 * right.M32 + left.M24 * right.M42;
            res.M23 = left.M21 * right.M13 + left.M22 * right.M23 + left.M23 * right.M33 + left.M24 * right.M43;
            res.M24 = left.M21 * right.M14 + left.M22 * right.M24 + left.M23 * right.M34 + left.M24 * right.M44;

            res.M31 = left.M31 * right.M11 + left.M32 * right.M21 + left.M33 * right.M31 + left.M34 * right.M41;
            res.M32 = left.M31 * right.M12 + left.M32 * right.M22 + left.M33 * right.M32 + left.M34 * right.M42;
            res.M33 = left.M31 * right.M13 + left.M32 * right.M23 + left.M33 * right.M33 + left.M34 * right.M43;
            res.M34 = left.M31 * right.M14 + left.M32 * right.M24 + left.M33 * right.M34 + left.M34 * right.M44;

            res.M41 = left.M41 * right.M11 + left.M42 * right.M21 + left.M43 * right.M31 + left.M44 * right.M41;
            res.M42 = left.M41 * right.M12 + left.M42 * right.M22 + left.M43 * right.M32 + left.M44 * right.M42;
            res.M43 = left.M41 * right.M13 + left.M42 * right.M23 + left.M43 * right.M33 + left.M44 * right.M43;
            res.M44 = left.M41 * right.M14 + left.M42 * right.M24 + left.M43 * right.M34 + left.M44 * right.M44;

            return res;
        }

        public static unsafe Float4 operator *(Matrix left, Float4 vector)
        {
            
            if (AdvSimd.IsSupported)
            {
                Unsafe.SkipInit(out Float4 result);
                var valueX = Vector128.Create(vector.X);
                var valueY = Vector128.Create(vector.Y);
                var valueZ = Vector128.Create(vector.Z);
                var valueW = Vector128.Create(vector.W);
                AdvSimd.Store(&result.X, AdvSimd.Add(Sse.Add(Sse.Add(
                            AdvSimd.Multiply(AdvSimd.LoadVector128(&left.M11), valueX), 
                            AdvSimd.Multiply(AdvSimd.LoadVector128(&left.M12), valueY)),
                        AdvSimd.Multiply(AdvSimd.LoadVector128(&left.M13), valueZ)),
                    AdvSimd.Multiply(AdvSimd.LoadVector128(&left.M14), valueW)));
                return result;
            }
            
            if (Sse.IsSupported)
            {
                Unsafe.SkipInit(out Float4 result);
                var valueX = Vector128.Create(vector.X);
                var valueY = Vector128.Create(vector.Y);
                var valueZ = Vector128.Create(vector.Z);
                var valueW = Vector128.Create(vector.W);
                Sse.Store(&result.X, Sse.Add(Sse.Add(Sse.Add(
                    Sse.Multiply(Sse.LoadVector128(&left.M11), valueX), 
                    Sse.Multiply(Sse.LoadVector128(&left.M12), valueY)),
                    Sse.Multiply(Sse.LoadVector128(&left.M13), valueZ)),
                    Sse.Multiply(Sse.LoadVector128(&left.M14), valueW)));
                return result;
            }
            
            var resultX = left.M11 * vector.X + left.M12 * vector.Y + left.M13 * vector.Z + left.M14 * vector.W;
            var resultY = left.M21 * vector.X + left.M22 * vector.Y + left.M23 * vector.Z + left.M24 * vector.W;
            var resultZ = left.M31 * vector.X + left.M32 * vector.Y + left.M33 * vector.Z + left.M34 * vector.W;
            var resultW = left.M41 * vector.X + left.M42 * vector.Y + left.M43 * vector.Z + left.M44 * vector.W;
            return new Float4(resultX, resultY, resultZ, resultW);
        }
        
        
        public static unsafe Matrix operator *(Matrix value1, float value2)
        {
            if (AdvSimd.IsSupported)
            {
                var value2Vec = Vector128.Create(value2);
                AdvSimd.Store(&value1.M11, AdvSimd.Multiply(AdvSimd.LoadVector128(&value1.M11), value2Vec));
                AdvSimd.Store(&value1.M12, AdvSimd.Multiply(AdvSimd.LoadVector128(&value1.M12), value2Vec));
                AdvSimd.Store(&value1.M13, AdvSimd.Multiply(AdvSimd.LoadVector128(&value1.M13), value2Vec));
                AdvSimd.Store(&value1.M14, AdvSimd.Multiply(AdvSimd.LoadVector128(&value1.M14), value2Vec));
                return value1;
            }

            if (Sse.IsSupported)
            {
                var value2Vec = Vector128.Create(value2);
                Sse.Store(&value1.M11, Sse.Multiply(Sse.LoadVector128(&value1.M11), value2Vec));
                Sse.Store(&value1.M12, Sse.Multiply(Sse.LoadVector128(&value1.M12), value2Vec));
                Sse.Store(&value1.M13, Sse.Multiply(Sse.LoadVector128(&value1.M13), value2Vec));
                Sse.Store(&value1.M14, Sse.Multiply(Sse.LoadVector128(&value1.M14), value2Vec));
                return value1;
            }

            Matrix m;
 
            m.M11 = value1.M11 * value2;
            m.M12 = value1.M12 * value2;
            m.M13 = value1.M13 * value2;
            m.M14 = value1.M14 * value2;
            m.M21 = value1.M21 * value2;
            m.M22 = value1.M22 * value2;
            m.M23 = value1.M23 * value2;
            m.M24 = value1.M24 * value2;
            m.M31 = value1.M31 * value2;
            m.M32 = value1.M32 * value2;
            m.M33 = value1.M33 * value2;
            m.M34 = value1.M34 * value2;
            m.M41 = value1.M41 * value2;
            m.M42 = value1.M42 * value2;
            m.M43 = value1.M43 * value2;
            m.M44 = value1.M44 * value2;
            return m;
        }

        public static unsafe bool operator ==(Matrix left, Matrix right)
        {
            if (AdvSimd.Arm64.IsSupported)
            {
                return SimdExtensions.Equal(AdvSimd.LoadVector128(&left.M11), AdvSimd.LoadVector128(&right.M11)) &&
                       SimdExtensions.Equal(AdvSimd.LoadVector128(&left.M12), AdvSimd.LoadVector128(&right.M12)) &&
                       SimdExtensions.Equal(AdvSimd.LoadVector128(&left.M13), AdvSimd.LoadVector128(&right.M13)) &&
                       SimdExtensions.Equal(AdvSimd.LoadVector128(&left.M14), AdvSimd.LoadVector128(&right.M14));
            }

            if (Sse.IsSupported)
            {
                return SimdExtensions.Equal(Sse.LoadVector128(&left.M11), Sse.LoadVector128(&right.M11)) &&
                       SimdExtensions.Equal(Sse.LoadVector128(&left.M12), Sse.LoadVector128(&right.M12)) &&
                       SimdExtensions.Equal(Sse.LoadVector128(&left.M13), Sse.LoadVector128(&right.M13)) &&
                       SimdExtensions.Equal(Sse.LoadVector128(&left.M14), Sse.LoadVector128(&right.M14));
            }

            // return left.M11 == right.M11 && left.M22 == right.M22 && left.M33 == right.M33 && left.M44 == right.M44 && // Check diagonal element first for early out.
            //        left.M12 == right.M12 && left.M13 == right.M13 && left.M14 == right.M14 && left.M21 == right.M21 &&
            //        left.M23 == right.M23 && left.M24 == right.M24 && left.M31 == right.M31 && left.M32 == right.M32 &&
            //        left.M34 == right.M34 && left.M41 == right.M41 && left.M42 == right.M42 && left.M43 == right.M43;
            // // Returns false in the presence of NaN values.
            return left.GetColumn(0) == right.GetColumn(0)
                   && left.GetColumn(1) == right.GetColumn(1)
                   && left.GetColumn(2) == right.GetColumn(2)
                   && left.GetColumn(3) == right.GetColumn(3);
        }

        //*undoc*
        public static unsafe bool operator !=(Matrix left, Matrix right)
        {
            if (AdvSimd.Arm64.IsSupported)
            {
                return SimdExtensions.NotEqual(AdvSimd.LoadVector128(&left.M11), AdvSimd.LoadVector128(&right.M11)) ||
                       SimdExtensions.NotEqual(AdvSimd.LoadVector128(&left.M12), AdvSimd.LoadVector128(&right.M12)) ||
                       SimdExtensions.NotEqual(AdvSimd.LoadVector128(&left.M13), AdvSimd.LoadVector128(&right.M13)) ||
                       SimdExtensions.NotEqual(AdvSimd.LoadVector128(&left.M14), AdvSimd.LoadVector128(&right.M14));
            }

            if (Sse.IsSupported)
            {
                return
                    SimdExtensions.NotEqual(Sse.LoadVector128(&left.M11), Sse.LoadVector128(&right.M11)) ||
                    SimdExtensions.NotEqual(Sse.LoadVector128(&left.M12), Sse.LoadVector128(&right.M12)) ||
                    SimdExtensions.NotEqual(Sse.LoadVector128(&left.M13), Sse.LoadVector128(&right.M13)) ||
                    SimdExtensions.NotEqual(Sse.LoadVector128(&left.M14), Sse.LoadVector128(&right.M14));
            }

            // Returns true in the presence of NaN values.
            return !(left == right);
        }

        public Float3 MultiplyPoint(Float3 point)
        {
            var resultX = M11 * point.X + M12 * point.Y + M13 * point.Z + M14;
            var resultY = M21 * point.X + M22 * point.Y + M23 * point.Z + M24;
            var resultZ = M31 * point.X + M32 * point.Y + M33 * point.Z + M34;
            var w = M41 * point.X + M42 * point.Y + M43 * point.Z + M44;

            w = 1F / w;
            resultX *= w;
            resultY *= w;
            resultZ *= w;
            return new Float3(resultX, resultY, resultZ);
        }

        public Float3 MultiplyPoint3X4(Float3 point)
        {
            var resultX = M11 * point.X + M12 * point.Y + M13 * point.Z + M14;
            var resultY = M21 * point.X + M22 * point.Y + M23 * point.Z + M24;
            var resultZ = M31 * point.X + M32 * point.Y + M33 * point.Z + M34;
            return new Float3(resultX, resultY, resultZ);
        }

        public Float3 MultiplyVector(Float3 vector)
        {
            var resultX = M11 * vector.X + M12 * vector.Y + M13 * vector.Z;
            var resultY = M21 * vector.X + M22 * vector.Y + M23 * vector.Z;
            var resultZ = M31 * vector.X + M32 * vector.Y + M33 * vector.Z;
            return new Float3(resultX, resultY, resultZ);
        }

        public static Matrix Rotate(float angle, Float3 axis)
        {
            var s = MathF.Sin(angle * MathF.PI / 180);
            var c = MathF.Cos(angle * MathF.PI / 180);

            axis = axis.Normalize();

            return new Matrix(
                axis.X * axis.X * (1 - c) + c,
                axis.Y * axis.X * (1 - c) + axis.Z * s,
                axis.X * axis.Z * (1 - c) - axis.Y * s,
                0,
                axis.X * axis.Y * (1 - c) - axis.Z * s,
                axis.Y * axis.Y * (1 - c) + c,
                axis.Y * axis.Z * (1 - c) + axis.X * s,
                0,
                axis.X * axis.Z * (1 - c) + axis.Y * s,
                axis.Y * axis.Z * (1 - c) - axis.X * s,
                axis.Z * axis.Z * (1 - c) + c,
                0,
                0, 0, 0, 1);
        }

        public static Matrix Scale(Float3 vector)
        {
            Matrix m;
            m.M11 = vector.X;
            m.M12 = 0F;
            m.M13 = 0F;
            m.M14 = 0F;
            m.M21 = 0F;
            m.M22 = vector.Y;
            m.M23 = 0F;
            m.M24 = 0F;
            m.M31 = 0F;
            m.M32 = 0F;
            m.M33 = vector.Z;
            m.M34 = 0F;
            m.M41 = 0F;
            m.M42 = 0F;
            m.M43 = 0F;
            m.M44 = 1F;
            return m;
        }


        public static Matrix Translate(Float3 vector)
        {
            Matrix m;
            m.M11 = 1F;
            m.M12 = 0F;
            m.M13 = 0F;
            m.M14 = vector.X;
            m.M21 = 0F;
            m.M22 = 1F;
            m.M23 = 0F;
            m.M24 = vector.Y;
            m.M31 = 0F;
            m.M32 = 0F;
            m.M33 = 1F;
            m.M34 = vector.Z;
            m.M41 = 0F;
            m.M42 = 0F;
            m.M43 = 0F;
            m.M44 = 1F;
            return m;
        }

        public static Matrix CreateLookAt(Float3 cameraPosition, Float3 cameraTarget, Float3 cameraUpVector)
        {
            var zAxis = (cameraPosition - cameraTarget).Normalize();
            var xAxis = cameraUpVector.Cross(zAxis).Normalize();
            var yAxis = zAxis.Cross(xAxis);

            var result = Identity;

            result.M11 = xAxis.X;
            result.M21 = yAxis.X;
            result.M31 = zAxis.X;

            result.M12 = xAxis.Y;
            result.M22 = yAxis.Y;
            result.M32 = zAxis.Y;

            result.M13 = xAxis.Z;
            result.M23 = yAxis.Z;
            result.M33 = zAxis.Z;

            result.M14 = -Float3.Dot(xAxis, cameraPosition);
            result.M24 = -Float3.Dot(yAxis, cameraPosition);
            result.M34 = -Float3.Dot(zAxis, cameraPosition);

            return result;
        }

        public static Matrix CreatePerspective(float fieldOfView, float aspectRatio, float nearPlaneDistance,
            float farPlaneDistance)
        {
            fieldOfView *= MathExtensions.Deg2Rad;
            var yScale = 1.0f / MathF.Tan(fieldOfView * 0.5f);
            var xScale = yScale / aspectRatio;

            Matrix result;

            result.M11 = xScale;
            result.M21 = result.M31 = result.M41 = 0.0f;

            result.M22 = yScale;
            result.M12 = result.M32 = result.M42 = 0.0f;

            result.M13 = result.M23 = 0.0f;
            var negFarRange = float.IsPositiveInfinity(farPlaneDistance)
                ? -1.0f
                : farPlaneDistance / (nearPlaneDistance - farPlaneDistance);
            result.M33 = negFarRange;
            result.M43 = -1.0f;

            result.M14 = result.M24 = result.M44 = 0.0f;
            result.M34 = nearPlaneDistance * negFarRange;

            return result;
        }

        /// <summary>Transposes the rows and columns of a matrix.</summary>
        /// <param name="matrix">The source matrix.</param>
        /// <returns>The transposed matrix.</returns>
        public static unsafe Matrix Transpose(Matrix matrix)
        {
            if (AdvSimd.Arm64.IsSupported)
            {
                // This implementation is based on the DirectX Math Library XMMatrixTranspose method
                // https://github.com/microsoft/DirectXMath/blob/master/Inc/DirectXMathMatrix.inl

                var m11 = AdvSimd.LoadVector128(&matrix.M11);
                var m13 = AdvSimd.LoadVector128(&matrix.M13);

                var p00 = AdvSimd.Arm64.ZipLow(m11, m13);
                var p01 = AdvSimd.Arm64.ZipHigh(m11, m13);

                var m12 = AdvSimd.LoadVector128(&matrix.M12);
                var m14 = AdvSimd.LoadVector128(&matrix.M14);

                var p10 = AdvSimd.Arm64.ZipLow(m12, m14);
                var p11 = AdvSimd.Arm64.ZipHigh(m12, m14);

                AdvSimd.Store(&matrix.M11, AdvSimd.Arm64.ZipLow(p00, p10));
                AdvSimd.Store(&matrix.M12, AdvSimd.Arm64.ZipHigh(p00, p10));
                AdvSimd.Store(&matrix.M13, AdvSimd.Arm64.ZipLow(p01, p11));
                AdvSimd.Store(&matrix.M14, AdvSimd.Arm64.ZipHigh(p01, p11));

                return matrix;
            }

            if (Sse.IsSupported)
            {
                var row1 = Sse.LoadVector128(&matrix.M11);
                var row2 = Sse.LoadVector128(&matrix.M12);
                var row3 = Sse.LoadVector128(&matrix.M13);
                var row4 = Sse.LoadVector128(&matrix.M14);

                var l12 = Sse.UnpackLow(row1, row2);
                var l34 = Sse.UnpackLow(row3, row4);
                var h12 = Sse.UnpackHigh(row1, row2);
                var h34 = Sse.UnpackHigh(row3, row4);

                Sse.Store(&matrix.M11, Sse.MoveLowToHigh(l12, l34));
                Sse.Store(&matrix.M12, Sse.MoveHighToLow(l34, l12));
                Sse.Store(&matrix.M13, Sse.MoveLowToHigh(h12, h34));
                Sse.Store(&matrix.M14, Sse.MoveHighToLow(h34, h12));

                return matrix;
            }

            Matrix result;

            result.M11 = matrix.M11;
            result.M12 = matrix.M21;
            result.M13 = matrix.M31;
            result.M14 = matrix.M41;
            result.M21 = matrix.M12;
            result.M22 = matrix.M22;
            result.M23 = matrix.M32;
            result.M24 = matrix.M42;
            result.M31 = matrix.M13;
            result.M32 = matrix.M23;
            result.M33 = matrix.M33;
            result.M34 = matrix.M43;
            result.M41 = matrix.M14;
            result.M42 = matrix.M24;
            result.M43 = matrix.M34;
            result.M44 = matrix.M44;

            return result;
        }


        public static readonly Matrix Zero = new Matrix(new Float4(0, 0, 0, 0),
            new Float4(0, 0, 0, 0),
            new Float4(0, 0, 0, 0),
            new Float4(0, 0, 0, 0));

        public static readonly Matrix Identity = new Matrix(new Float4(1, 0, 0, 0),
            new Float4(0, 1, 0, 0),
            new Float4(0, 0, 1, 0),
            new Float4(0, 0, 0, 1));
    }
}