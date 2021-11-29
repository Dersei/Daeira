using System;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.Arm;
using System.Runtime.Intrinsics.X86;

// Following methods are based on source code of .NET provided by .NET Foundation

namespace Daeira.Extensions
{
    public static class SimdExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Equal(Vector128<float> vector1, Vector128<float> vector2)
        {
            // This implementation is based on the DirectX Math Library XMVector4Equal method
            // https://github.com/microsoft/DirectXMath/blob/master/Inc/DirectXMathVector.inl
 
            if (AdvSimd.Arm64.IsSupported)
            {
                var vResult = AdvSimd.CompareEqual(vector1, vector2).AsUInt32();
 
                var vResult0 = vResult.GetLower().AsByte();
                var vResult1 = vResult.GetUpper().AsByte();
 
                var vTemp10 = AdvSimd.Arm64.ZipLow(vResult0, vResult1);
                var vTemp11 = AdvSimd.Arm64.ZipHigh(vResult0, vResult1);
 
                var vTemp21 = AdvSimd.Arm64.ZipHigh(vTemp10.AsUInt16(), vTemp11.AsUInt16());
                return vTemp21.AsUInt32().GetElement(1) == 0xFFFFFFFF;
            }

            if (Sse.IsSupported)
            {
                return Sse.MoveMask(Sse.CompareNotEqual(vector1, vector2)) == 0;
            }
            // Redundant test so we won't prejit remainder of this method on platforms without AdvSimd.
            throw new PlatformNotSupportedException();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool NotEqual(Vector128<float> vector1, Vector128<float> vector2)
        {
            // This implementation is based on the DirectX Math Library XMVector4NotEqual method
            // https://github.com/microsoft/DirectXMath/blob/master/Inc/DirectXMathVector.inl
 
            if (AdvSimd.IsSupported)
            {
                var vResult = AdvSimd.CompareEqual(vector1, vector2).AsUInt32();
 
                var vResult0 = vResult.GetLower().AsByte();
                var vResult1 = vResult.GetUpper().AsByte();
 
                var vTemp10 = AdvSimd.Arm64.ZipLow(vResult0, vResult1);
                var vTemp11 = AdvSimd.Arm64.ZipHigh(vResult0, vResult1);
 
                var vTemp21 = AdvSimd.Arm64.ZipHigh(vTemp10.AsUInt16(), vTemp11.AsUInt16());
                return vTemp21.AsUInt32().GetElement(1) != 0xFFFFFFFF;
            }

            if (Sse.IsSupported)
            {
                return Sse.MoveMask(Sse.CompareNotEqual(vector1, vector2)) != 0;
            }
            // Redundant test so we won't prejit remainder of this method on platforms without AdvSimd.
            throw new PlatformNotSupportedException();
        }
    }
}