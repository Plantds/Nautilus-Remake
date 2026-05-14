using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;

namespace CustomMath
{
    public static class Lerp
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Quaternion ApproxSlerp(Quaternion _start, Quaternion _end, float _responseSpeed, float _deltaTime)
        {
            return math.slerp(_start, _end, ApproxTime(_responseSpeed, _deltaTime));
        }

        // Unity Engine
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 ApproxLerp(Vector4 _start, Vector4 _end, float _responseSpeed, float _deltaTime)
        {
            return Vector4.Lerp(_start, _end, ApproxTime(_responseSpeed, _deltaTime));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 ApproxLerp(Vector3 _start, Vector3 _end, float _responseSpeed, float _deltaTime)
        {
            return Vector3.Lerp(_start, _end, ApproxTime(_responseSpeed, _deltaTime));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 ApproxLerp(Vector2 _start, Vector2 _end, float _responseSpeed, float _deltaTime)
        {
            return Vector2.Lerp(_start, _end, ApproxTime(_responseSpeed, _deltaTime));
        }

        // Shared
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ApproxLerp(float _start, float _end, float _responseSpeed, float _deltaTime)
        {
            return Mathf.Lerp(_start, _end, ApproxTime(_responseSpeed, _deltaTime));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ApproxTime(float _responseSpeed, float _deltaTime)
        {
            return 1.0f - math.exp(-_responseSpeed * _deltaTime);
        }
    }

}
