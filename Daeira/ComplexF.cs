using System;
using System.Diagnostics;
using System.Globalization;
using System.Numerics;

namespace Daeira
{
    public readonly struct ComplexF : IEquatable<ComplexF>, IFormattable
    {
        public static readonly ComplexF Zero = new ComplexF(0.0f, 0.0f);
        public static readonly ComplexF One = new ComplexF(1.0f, 0.0f);
        public static readonly ComplexF ImaginaryOne = new ComplexF(0.0f, 1.0f);
        public static readonly ComplexF NaN = new ComplexF(float.NaN, float.NaN);
        public static readonly ComplexF Infinity = new ComplexF(float.PositiveInfinity, float.PositiveInfinity);

        private const float InverseOfLog10 = 0.43429448190325f; // 1 / Log(10)

        // This is the largest x for which (Hypot(x,x) + x) will not overflow. It is used for branching inside Sqrt.
        private static readonly float SqrtRescaleThreshold = float.MaxValue / (MathF.Sqrt(2.0f) + 1.0f);

        // This is the largest x for which 2 x^2 will not overflow. It is used for branching inside Asin and Acos.
        private static readonly float AsinOverflowThreshold = MathF.Sqrt(float.MaxValue) / 2.0f;

        // This value is used inside Asin and Acos.
        private static readonly float Log2 = MathF.Log(2.0f);

        public readonly float Real;
        public readonly float Imaginary;

        public ComplexF(float real, float imaginary)
        {
            Real = real;
            Imaginary = imaginary;
        }

        public static ComplexF FromPolarCoordinates(float magnitude, float phase)
        {
            return new ComplexF(magnitude * MathF.Cos(phase), magnitude * MathF.Sin(phase));
        }

        public static ComplexF operator -(ComplexF value) /* Unary negation of a ComplexF number */
        {
            return new ComplexF(-value.Real, -value.Imaginary);
        }

        public static ComplexF operator +(ComplexF left, ComplexF right)
        {
            return new ComplexF(left.Real + right.Real, left.Imaginary + right.Imaginary);
        }

        public static ComplexF operator +(ComplexF left, float right)
        {
            return new ComplexF(left.Real + right, left.Imaginary);
        }

        public static ComplexF operator +(float left, ComplexF right)
        {
            return new ComplexF(left + right.Real, right.Imaginary);
        }

        public static ComplexF operator -(ComplexF left, ComplexF right)
        {
            return new ComplexF(left.Real - right.Real, left.Imaginary - right.Imaginary);
        }

        public static ComplexF operator -(ComplexF left, float right)
        {
            return new ComplexF(left.Real - right, left.Imaginary);
        }

        public static ComplexF operator -(float left, ComplexF right)
        {
            return new ComplexF(left - right.Real, -right.Imaginary);
        }

        public static ComplexF operator *(ComplexF left, ComplexF right)
        {
            // Multiplication:  (a + bi)(c + di) = (ac -bd) + (bc + ad)i
            var resultRealPart = left.Real * right.Real - left.Imaginary * right.Imaginary;
            var resultImaginaryPart = left.Imaginary * right.Real + left.Real * right.Imaginary;
            return new ComplexF(resultRealPart, resultImaginaryPart);
        }

        public static ComplexF operator *(ComplexF left, float right)
        {
            if (!float.IsFinite(left.Real))
            {
                if (!float.IsFinite(left.Imaginary))
                {
                    return new ComplexF(float.NaN, float.NaN);
                }

                return new ComplexF(left.Real * right, float.NaN);
            }

            if (!float.IsFinite(left.Imaginary))
            {
                return new ComplexF(float.NaN, left.Imaginary * right);
            }

            return new ComplexF(left.Real * right, left.Imaginary * right);
        }

        public static ComplexF operator *(float left, ComplexF right)
        {
            if (!float.IsFinite(right.Real))
            {
                if (!float.IsFinite(right.Imaginary))
                {
                    return new ComplexF(float.NaN, float.NaN);
                }

                return new ComplexF(left * right.Real, float.NaN);
            }

            if (!float.IsFinite(right.Imaginary))
            {
                return new ComplexF(float.NaN, left * right.Imaginary);
            }

            return new ComplexF(left * right.Real, left * right.Imaginary);
        }

        public static ComplexF operator /(ComplexF left, ComplexF right)
        {
            // Division : Smith's formula.
            var a = left.Real;
            var b = left.Imaginary;
            var c = right.Real;
            var d = right.Imaginary;

            // Computing c * c + d * d will overflow even in cases where the actual result of the division does not overflow.
            if (MathF.Abs(d) < MathF.Abs(c))
            {
                var doc = d / c;
                return new ComplexF((a + b * doc) / (c + d * doc), (b - a * doc) / (c + d * doc));
            }

            var cod = c / d;
            return new ComplexF((b + a * cod) / (d + c * cod), (-a + b * cod) / (d + c * cod));
        }

        public static ComplexF operator /(ComplexF left, float right)
        {
            // IEEE prohibit optimizations which are value changing
            // so we make sure that behaviour for the simplified version exactly match
            // full version.
            if (right == 0)
            {
                return new ComplexF(float.NaN, float.NaN);
            }

            if (!float.IsFinite(left.Real))
            {
                if (!float.IsFinite(left.Imaginary))
                {
                    return new ComplexF(float.NaN, float.NaN);
                }

                return new ComplexF(left.Real / right, float.NaN);
            }

            if (!float.IsFinite(left.Imaginary))
            {
                return new ComplexF(float.NaN, left.Imaginary / right);
            }

            // Here the actual optimized version of code.
            return new ComplexF(left.Real / right, left.Imaginary / right);
        }

        public static ComplexF operator /(float left, ComplexF right)
        {
            // Division : Smith's formula.
            var a = left;
            var c = right.Real;
            var d = right.Imaginary;

            // Computing c * c + d * d will overflow even in cases where the actual result of the division does not overflow.
            if (MathF.Abs(d) < MathF.Abs(c))
            {
                var doc = d / c;
                return new ComplexF(a / (c + d * doc), -a * doc / (c + d * doc));
            }

            var cod = c / d;
            return new ComplexF(a * cod / (d + c * cod), -a / (d + c * cod));
        }

        public static float Abs(ComplexF value)
        {
            return Hypot(value.Real, value.Imaginary);
        }

        private static float Hypot(float a, float b)
        {
            // Using
            //   sqrt(a^2 + b^2) = |a| * sqrt(1 + (b/a)^2)
            // we can factor out the larger component to dodge overflow even when a * a would overflow.

            a = MathF.Abs(a);
            b = MathF.Abs(b);

            float small, large;
            if (a < b)
            {
                small = a;
                large = b;
            }
            else
            {
                small = b;
                large = a;
            }

            if (small == 0.0f)
            {
                return large;
            }

            if (float.IsPositiveInfinity(large) && !float.IsNaN(small))
            {
                // The NaN test is necessary so we don't return +inf when small=NaN and large=+inf.
                // NaN in any other place returns NaN without any special handling.
                return float.PositiveInfinity;
            }
            var ratio = small / large;
            return large * MathF.Sqrt(1.0f + ratio * ratio);
        }

        private static float Log1P(float x)
        {
            // Compute log(1 + x) without loss of accuracy when x is small.

            // Our only use case so far is for positive values, so this isn't coded to handle negative values.
            Debug.Assert(x >= 0.0 || float.IsNaN(x));

            var xp1 = 1.0f + x;
            if (xp1 == 1.0f)
            {
                return x;
            }

            if (x < 0.75f)
            {
                // This is accurate to within 5 ulp with any floating-point system that uses a guard digit,
                // as proven in Theorem 4 of "What Every Computer Scientist Should Know About Floating-Point
                // Arithmetic" (https://docs.oracle.com/cd/E19957-01/806-3568/ncg_goldberg.html)
                return x * MathF.Log(xp1) / (xp1 - 1.0f);
            }

            return MathF.Log(xp1);
        }

        public static ComplexF Conjugate(ComplexF value)
        {
            // Conjugate of a ComplexF number: the conjugate of x+i*y is x-i*y
            return new ComplexF(value.Real, -value.Imaginary);
        }

        public static ComplexF Reciprocal(ComplexF value)
        {
            // Reciprocal of a ComplexF number : the reciprocal of x+i*y is 1/(x+i*y)
            if (value.Real == 0 && value.Imaginary == 0)
            {
                return Zero;
            }

            return One / value;
        }

        public static bool operator ==(ComplexF left, ComplexF right)
        {
            return left.Real == right.Real && left.Imaginary == right.Imaginary;
        }

        public static bool operator !=(ComplexF left, ComplexF right)
        {
            return left.Real != right.Real || left.Imaginary != right.Imaginary;
        }

        public override bool Equals(object? obj)
        {
            return obj is ComplexF f && Equals(f);
        }

        public bool Equals(ComplexF value)
        {
            return Real.Equals(value.Real) && Imaginary.Equals(value.Imaginary);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Real, Imaginary);
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, "({0}, {1})", Real, Imaginary);
        }

        public string ToString(string? format)
        {
            return string.Format(CultureInfo.CurrentCulture, "({0}, {1})",
                Real.ToString(format, CultureInfo.CurrentCulture),
                Imaginary.ToString(format, CultureInfo.CurrentCulture));
        }

        public string ToString(IFormatProvider? provider)
        {
            return string.Format(provider, "({0}, {1})", Real, Imaginary);
        }

        public string ToString(string? format, IFormatProvider? provider)
        {
            return string.Format(provider, "({0}, {1})", Real.ToString(format, provider),
                Imaginary.ToString(format, provider));
        }

        public static ComplexF Sin(ComplexF value)
        {
            // We need both sinh and cosh of imaginary part. To avoid multiple calls to MathF.Exp with the same value,
            // we compute them both here from a single call to MathF.Exp.
            var p = MathF.Exp(value.Imaginary);
            var q = 1.0f / p;
            var sinh = (p - q) * 0.5f;
            var cosh = (p + q) * 0.5f;
            return new ComplexF(MathF.Sin(value.Real) * cosh, MathF.Cos(value.Real) * sinh);
            // There is a known limitation with this algorithm: inputs that cause sinh and cosh to overflow, but for
            // which sin or cos are small enough that sin * cosh or cos * sinh are still representable, nonetheless
            // produce overflow. For example, Sin((0.01, 711.0)) should produce (~3.0E306, PositiveInfinity), but
            // instead produces (PositiveInfinity, PositiveInfinity).
        }

        public static ComplexF Sinh(ComplexF value)
        {
            // Use sinh(z) = -i sin(iz) to compute via sin(z).
            var sin = Sin(new ComplexF(-value.Imaginary, value.Real));
            return new ComplexF(sin.Imaginary, -sin.Real);
        }

        public static ComplexF Asin(ComplexF value)
        {
            AsinInternal(MathF.Abs(value.Real), MathF.Abs(value.Imaginary), out var b, out var bPrime, out var v);

            var u = bPrime < 0.0 ? MathF.Asin(b) : MathF.Atan(bPrime);

            if (value.Real < 0.0) u = -u;
            if (value.Imaginary < 0.0) v = -v;

            return new ComplexF(u, v);
        }

        public static ComplexF Cos(ComplexF value)
        {
            var p = MathF.Exp(value.Imaginary);
            var q = 1.0f / p;
            var sinh = (p - q) * 0.5f;
            var cosh = (p + q) * 0.5f;
            return new ComplexF(MathF.Cos(value.Real) * cosh, -MathF.Sin(value.Real) * sinh);
        }

        public static ComplexF Cosh(ComplexF value)
        {
            // Use cosh(z) = cos(iz) to compute via cos(z).
            return Cos(new ComplexF(-value.Imaginary, value.Real));
        }

        public static ComplexF Acos(ComplexF value)
        {
            AsinInternal(MathF.Abs(value.Real), MathF.Abs(value.Imaginary), out var b, out var bPrime, out var v);

            var u = bPrime < 0.0f ? MathF.Acos(b) : MathF.Atan(1.0f / bPrime);

            if (value.Real < 0.0f) u = MathF.PI - u;
            if (value.Imaginary > 0.0f) v = -v;

            return new ComplexF(u, v);
        }

        public static ComplexF Tan(ComplexF value)
        {
            // tan z = sin z / cos z, but to avoid unnecessary repeated trig computations, use
            //   tan z = (sin(2x) + i sinh(2y)) / (cos(2x) + cosh(2y))
            // (see Abramowitz & Stegun 4.3.57 or derive by hand), and compute trig functions here.

            // This approach does not work for |y| > ~355, because sinh(2y) and cosh(2y) overflow,
            // even though their ratio does not. In that case, divide through by cosh to get:
            //   tan z = (sin(2x) / cosh(2y) + i \tanh(2y)) / (1 + cos(2x) / cosh(2y))
            // which correctly computes the (tiny) real part and the (normal-sized) imaginary part.

            var x2 = 2.0f * value.Real;
            var y2 = 2.0f * value.Imaginary;
            var p = MathF.Exp(y2);
            var q = 1.0f / p;
            var cosh = (p + q) * 0.5f;
            if (MathF.Abs(value.Imaginary) <= 4.0f)
            {
                var sinh = (p - q) * 0.5f;
                var d = MathF.Cos(x2) + cosh;
                return new ComplexF(MathF.Sin(x2) / d, sinh / d);
            }
            else
            {
                var d = 1.0f + MathF.Cos(x2) / cosh;
                return new ComplexF(MathF.Sin(x2) / cosh / d, MathF.Tanh(y2) / d);
            }
        }

        public static ComplexF Tanh(ComplexF value)
        {
            // Use tanh(z) = -i tan(iz) to compute via tan(z).
            var tan = Tan(new ComplexF(-value.Imaginary, value.Real));
            return new ComplexF(tan.Imaginary, -tan.Real);
        }

        public static ComplexF Atan(ComplexF value)
        {
            var two = new ComplexF(2.0f, 0.0f);
            return ImaginaryOne / two * (Log(One - ImaginaryOne * value) - Log(One + ImaginaryOne * value));
        }

        private static void AsinInternal(float x, float y, out float b, out float bPrime, out float v)
        {
            // This method for the inverse ComplexF sine (and cosine) is described in Hull, Fairgrieve,
            // and Tang, "Implementing the ComplexF Arcsine and Arccosine Functions Using Exception Handling",
            // ACM Transactions on Mathematical Software (1997)
            // (https://www.researchgate.net/profile/Ping_Tang3/publication/220493330_Implementing_the_Complex_Arcsine_and_Arccosine_Functions_Using_Exception_Handling/links/55b244b208ae9289a085245d.pdf)

            // First, the basics: start with sin(w) = (e^{iw} - e^{-iw}) / (2i) = z. Here z is the input
            // and w is the output. To solve for w, define t = e^{i w} and multiply through by t to
            // get the quadratic equation t^2 - 2 i z t - 1 = 0. The solution is t = i z + sqrt(1 - z^2), so
            //   w = arcsin(z) = - i log( i z + sqrt(1 - z^2) )
            // Decompose z = x + i y, multiply out i z + sqrt(1 - z^2), use log(s) = |s| + i arg(s), and do a
            // bunch of algebra to get the components of w = arcsin(z) = u + i v
            //   u = arcsin(beta)  v = sign(y) log(alpha + sqrt(alpha^2 - 1))
            // where
            //   alpha = (rho + sigma) / 2      beta = (rho - sigma) / 2
            //   rho = sqrt((x + 1)^2 + y^2)    sigma = sqrt((x - 1)^2 + y^2)
            // These formulas appear in DLMF section 4.23. (http://dlmf.nist.gov/4.23), along with the analogous
            //   arccos(w) = arccos(beta) - i sign(y) log(alpha + sqrt(alpha^2 - 1))
            // So alpha and beta together give us arcsin(w) and arccos(w).

            // As written, alpha is not susceptible to cancelation errors, but beta is. To avoid cancelation, note
            //   beta = (rho^2 - sigma^2) / (rho + sigma) / 2 = (2 x) / (rho + sigma) = x / alpha
            // which is not subject to cancellation. Note alpha >= 1 and |beta| <= 1.

            // For alpha ~ 1, the argument of the log is near unity, so we compute (alpha - 1) instead,
            // write the argument as 1 + (alpha - 1) + sqrt((alpha - 1)(alpha + 1)), and use the log1p function
            // to compute the log without loss of accuracy.
            // For beta ~ 1, arccos does not accurately resolve small angles, so we compute the tangent of the angle
            // instead.
            // Hull, Fairgrieve, and Tang derive formulas for (alpha - 1) and beta' = tan(u) that do not suffer
            // from cancellation in these cases.

            // For simplicity, we assume all positive inputs and return all positive outputs. The caller should
            // assign signs appropriate to the desired cut conventions. We return v directly since its magnitude
            // is the same for both arcsin and arccos. Instead of u, we usually return beta and sometimes beta'.
            // If beta' is not computed, it is set to -1; if it is computed, it should be used instead of beta
            // to determine u. Compute u = arcsin(beta) or u = arctan(beta') for arcsin, u = arccos(beta)
            // or arctan(1/beta') for arccos.

            Debug.Assert(x >= 0.0 || float.IsNaN(x));
            Debug.Assert(y >= 0.0 || float.IsNaN(y));

            // For x or y large enough to overflow alpha^2, we can simplify our formulas and avoid overflow.
            if (x > AsinOverflowThreshold || y > AsinOverflowThreshold)
            {
                b = -1.0f;
                bPrime = x / y;

                float small, big;
                if (x < y)
                {
                    small = x;
                    big = y;
                }
                else
                {
                    small = y;
                    big = x;
                }

                var ratio = small / big;
                v = Log2 + MathF.Log(big) + 0.5f * Log1P(ratio * ratio);
            }
            else
            {
                var r = Hypot(x + 1.0f, y);
                var s = Hypot(x - 1.0f, y);

                var a = (r + s) * 0.5f;
                b = x / a;

                if (b > 0.75f)
                {
                    if (x <= 1.0f)
                    {
                        var amx = (y * y / (r + (x + 1.0f)) + (s + (1.0f - x))) * 0.5f;
                        bPrime = x / MathF.Sqrt((a + x) * amx);
                    }
                    else
                    {
                        // In this case, amx ~ y^2. Since we take the square root of amx, we should
                        // pull y out from under the square root so we don't lose its contribution
                        // when y^2 underflows.
                        var t = (1.0f / (r + (x + 1.0f)) + 1.0f / (s + (x - 1.0f))) * 0.5f;
                        bPrime = x / y / MathF.Sqrt((a + x) * t);
                    }
                }
                else
                {
                    bPrime = -1.0f;
                }

                if (a < 1.5f)
                {
                    if (x < 1.0f)
                    {
                        // This is another case where our expression is proportional to y^2 and
                        // we take its square root, so again we pull out a factor of y from
                        // under the square root.
                        var t = (1.0f / (r + (x + 1.0f)) + 1.0f / (s + (1.0f - x))) * 0.5f;
                        var am1 = y * y * t;
                        v = Log1P(am1 + y * MathF.Sqrt(t * (a + 1.0f)));
                    }
                    else
                    {
                        var am1 = (y * y / (r + (x + 1.0f)) + (s + (x - 1.0f))) * 0.5f;
                        v = Log1P(am1 + MathF.Sqrt(am1 * (a + 1.0f)));
                    }
                }
                else
                {
                    // Because of the test above, we can be sure that a * a will not overflow.
                    v = MathF.Log(a + MathF.Sqrt((a - 1.0f) * (a + 1.0f)));
                }
            }
        }

        public static bool IsFinite(ComplexF value) => float.IsFinite(value.Real) && float.IsFinite(value.Imaginary);

        public static bool IsInfinity(ComplexF value) =>
            float.IsInfinity(value.Real) || float.IsInfinity(value.Imaginary);

        public static bool IsNaN(ComplexF value) => !IsInfinity(value) && !IsFinite(value);

        public static ComplexF Log(ComplexF value)
        {
            return new ComplexF(MathF.Log(Abs(value)), MathF.Atan2(value.Imaginary, value.Real));
        }

        public static ComplexF Log(ComplexF value, float baseValue)
        {
            return Log(value) / Log(baseValue);
        }

        public static ComplexF Log10(ComplexF value)
        {
            var tempLog = Log(value);
            return Scale(tempLog, InverseOfLog10);
        }

        public static ComplexF Exp(ComplexF value)
        {
            var expReal = MathF.Exp(value.Real);
            var cosImaginary = expReal * MathF.Cos(value.Imaginary);
            var sinImaginary = expReal * MathF.Sin(value.Imaginary);
            return new ComplexF(cosImaginary, sinImaginary);
        }

        public static ComplexF Sqrt(ComplexF value)
        {
            if (value.Imaginary == 0.0f)
            {
                // Handle the trivial case quickly.
                if (value.Real < 0.0f)
                {
                    return new ComplexF(0.0f, MathF.Sqrt(-value.Real));
                }

                return new ComplexF(MathF.Sqrt(value.Real), 0.0f);
            }
            // One way to compute Sqrt(z) is just to call Pow(z, 0.5), which coverts to polar coordinates
            // (sqrt + atan), halves the phase, and reconverts to cartesian coordinates (cos + sin).
            // Not only is this more expensive than necessary, it also fails to preserve certain expected
            // symmetries, such as that the square root of a pure negative is a pure imaginary, and that the
            // square root of a pure imaginary has exactly equal real and imaginary parts. This all goes
            // back to the fact that MathF.PI is not stored with infinite precision, so taking half of MathF.PI
            // does not land us on an argument with cosine exactly equal to zero.

            // To find a fast and symmetry-respecting formula for ComplexF square root,
            // note x + i y = \sqrt{a + i b} implies x^2 + 2 i x y - y^2 = a + i b,
            // so x^2 - y^2 = a and 2 x y = b. Cross-substitute and use the quadratic formula to obtain
            //   x = \sqrt{\frac{\sqrt{a^2 + b^2} + a}{2}}  y = \pm \sqrt{\frac{\sqrt{a^2 + b^2} - a}{2}}
            // There is just one complication: depending on the sign on a, either x or y suffers from
            // cancellation when |b| << |a|. We can get around this by noting that our formulas imply
            // x^2 y^2 = b^2 / 4, so |x| |y| = |b| / 2. So after computing the one that doesn't suffer
            // from cancellation, we can compute the other with just a division. This is basically just
            // the right way to evaluate the quadratic formula without cancellation.

            // All this reduces our total cost to two sqrts and a few flops, and it respects the desired
            // symmetries. Much better than atan + cos + sin!

            // The signs are a matter of choice of branch cut, which is traditionally taken so x > 0 and sign(y) = sign(b).

            // If the components are too large, Hypot will overflow, even though the subsequent sqrt would
            // make the result representable. To avoid this, we re-scale (by exact powers of 2 for accuracy)
            // when we encounter very large components to avoid intermediate infinities.
            var rescale = false;
            var valueReal = value.Real;
            var valueImaginary = value.Imaginary;
            if (MathF.Abs(valueReal) >= SqrtRescaleThreshold || MathF.Abs(valueImaginary) >= SqrtRescaleThreshold)
            {
                if (float.IsInfinity(valueImaginary) && !float.IsNaN(valueReal))
                {
                    // We need to handle infinite imaginary parts specially because otherwise
                    // our formulas below produce inf/inf = NaN. The NaN test is necessary
                    // so that we return NaN rather than (+inf,inf) for (NaN,inf).
                    return new ComplexF(float.PositiveInfinity, valueImaginary);
                }

                valueReal *= 0.25f;
                valueImaginary *= 0.25f;
                rescale = true;
            }

            // This is the core of the algorithm. Everything else is special case handling.
            float x, y;
            if (valueReal >= 0.0f)
            {
                x = MathF.Sqrt((Hypot(valueReal, valueImaginary) + valueReal) * 0.5f);
                y = valueImaginary / (2.0f * x);
            }
            else
            {
                y = MathF.Sqrt((Hypot(valueReal, valueImaginary) - valueReal) * 0.5f);
                if (valueImaginary < 0.0f) y = -y;
                x = valueImaginary / (2.0f * y);
            }

            if (rescale)
            {
                x *= 2.0f;
                y *= 2.0f;
            }

            return new ComplexF(x, y);
        }

        public static ComplexF Pow(ComplexF value, ComplexF power)
        {
            if (power == Zero)
            {
                return One;
            }

            if (value == Zero)
            {
                return Zero;
            }

            var valueReal = value.Real;
            var valueImaginary = value.Imaginary;
            var powerReal = power.Real;
            var powerImaginary = power.Imaginary;

            var rho = Abs(value);
            var theta = MathF.Atan2(valueImaginary, valueReal);
            var newRho = powerReal * theta + powerImaginary * MathF.Log(rho);

            var t = MathF.Pow(rho, powerReal) * MathF.Pow(MathF.E, -powerImaginary * theta);

            return new ComplexF(t * MathF.Cos(newRho), t * MathF.Sin(newRho));
        }

        public static ComplexF Pow(ComplexF value, float power)
        {
            return Pow(value, new ComplexF(power, 0));
        }

        private static ComplexF Scale(ComplexF value, float factor)
        {
            var realResult = factor * value.Real;
            var imaginaryResult = factor * value.Imaginary;
            return new ComplexF(realResult, imaginaryResult);
        }

        public static implicit operator ComplexF(short value)
        {
            return new ComplexF(value, 0.0f);
        }

        public static implicit operator ComplexF(int value)
        {
            return new ComplexF(value, 0.0f);
        }

        public static implicit operator ComplexF(long value)
        {
            return new ComplexF(value, 0.0f);
        }

        public static implicit operator ComplexF(ushort value)
        {
            return new ComplexF(value, 0.0f);
        }

        public static implicit operator ComplexF(uint value)
        {
            return new ComplexF(value, 0.0f);
        }

        public static implicit operator ComplexF(ulong value)
        {
            return new ComplexF(value, 0.0f);
        }

        public static implicit operator ComplexF(sbyte value)
        {
            return new ComplexF(value, 0.0f);
        }

        public static implicit operator ComplexF(byte value)
        {
            return new ComplexF(value, 0.0f);
        }

        public static implicit operator ComplexF(float value)
        {
            return new ComplexF(value, 0.0f);
        }

        public static implicit operator ComplexF(double value)
        {
            return new ComplexF((float) value, 0.0f);
        }

        public static explicit operator ComplexF(BigInteger value)
        {
            return new ComplexF((float) value, 0.0f);
        }

        public static explicit operator ComplexF(decimal value)
        {
            return new ComplexF((float) value, 0.0f);
        }
    }
}