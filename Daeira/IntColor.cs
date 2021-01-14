using System;
using System.Globalization;
using System.Runtime.InteropServices;
using Daeira.Extensions;

namespace Daeira
{
    [StructLayout(LayoutKind.Explicit)]
    public readonly struct IntColor : IEquatable<IntColor>, IFormattable
    {
        [FieldOffset(0)] public readonly int Rgba;
        [FieldOffset(0)] public readonly byte R;
        [FieldOffset(sizeof(byte) * 1)] public readonly byte G;
        [FieldOffset(sizeof(byte) * 2)] public readonly byte B;
        [FieldOffset(sizeof(byte) * 3)] public readonly byte A;

        public IntColor(byte r, byte g, byte b, byte a)
        {
            Rgba = 0;
            R = r;
            G = g;
            B = b;
            A = a;
        }
        
        public static readonly IntColor Black = new IntColor(0, 0, 0, 255);
        public static readonly IntColor White = new IntColor(255, 255, 255, 255);
        public static readonly IntColor Grey = new IntColor(128, 128, 128, 255);
        public static readonly IntColor Red = new IntColor(255, 0, 0, 255);
        public static readonly IntColor Blue = new IntColor(0, 0, 255, 255);
        public static readonly IntColor Green = new IntColor(0, 255, 0, 255);
        public static readonly IntColor Yellow = new IntColor(255, 255, 0, 255);
        public static readonly IntColor UnityYellow = new IntColor(255, 235, 4, 255);
        public static readonly IntColor Cyan = new IntColor(0, 255, 255, 255);
        public static readonly IntColor Magenta = new IntColor(255, 0, 255, 255);
        public static readonly IntColor Purple = new IntColor(255, 0, 255, 255);
        public static readonly IntColor Error = new IntColor(255, 0, 144, 255);
        public static readonly IntColor Clear = new IntColor(0, 0, 0, 0);

        public static implicit operator IntColor(FloatColor c)
        {
            return new IntColor((byte) MathF.Round(MathExtensions.Clamp01(c.R) * 255f),
                (byte) MathF.Round(MathExtensions.Clamp01(c.G) * 255f),
                (byte) MathF.Round(MathExtensions.Clamp01(c.B) * 255f),
                (byte) MathF.Round(MathExtensions.Clamp01(c.A) * 255f));
        }

        public static implicit operator FloatColor(IntColor c)
        {
            return new FloatColor(c.R / 255f, c.G / 255f, c.B / 255f, c.A / 255f);
        }

        public static IntColor Lerp(IntColor a, IntColor b, float t)
        {
            t = MathExtensions.Clamp01(t);
            return new IntColor(
                (byte) (a.R + (b.R - a.R) * t),
                (byte) (a.G + (b.G - a.G) * t),
                (byte) (a.B + (b.B - a.B) * t),
                (byte) (a.A + (b.A - a.A) * t)
            );
        }

        public static IntColor LerpUnclamped(IntColor a, IntColor b, float t)
        {
            return new IntColor(
                (byte) (a.R + (b.R - a.R) * t),
                (byte) (a.G + (b.G - a.G) * t),
                (byte) (a.B + (b.B - a.B) * t),
                (byte) (a.A + (b.A - a.A) * t)
            );
        }

        public byte this[int index] =>
            index switch
            {
                0 => R,
                1 => G,
                2 => B,
                3 => A,
                _ => throw new IndexOutOfRangeException("Invalid IntColor index(" + index + ")!")
            };

        public override string ToString()
        {
            return ToString(null, CultureInfo.InvariantCulture.NumberFormat);
        }

        public string ToString(string? format)
        {
            return ToString(format, CultureInfo.InvariantCulture.NumberFormat);
        }

        public string ToString(string? format, IFormatProvider? formatProvider)
        {
            return
                $"RGBA({R.ToString(format, formatProvider)}, {G.ToString(format, formatProvider)}, {B.ToString(format, formatProvider)}, {A.ToString(format, formatProvider)})";
        }

        public bool Equals(IntColor other)
        {
            return Rgba == other.Rgba;
        }

        public override bool Equals(object? obj)
        {
            return obj is IntColor other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Rgba;
        }

        public static bool operator ==(IntColor left, IntColor right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(IntColor left, IntColor right)
        {
            return !(left == right);
        }
    }
}