using System;
using UnityEngine;

public static class ConstantAccelerationControl
{
    private static float aOff;

    private static float aOn;

    private const float BigTime = 10000f;

    public static float AccelerationFromTime(float v, float x, float t)
    {
        if (t == 0f)
        {
            throw new ArgumentException("t can't be 0", "t");
        }
        float single = Mathf.Sqrt((t * v - 2f * x) * (t * v - 2f * x) + t * t * v * v);
        float single1 = (-(t * v - 2f * x) + single) / t / t;
        float single2 = (-(t * v - 2f * x) - single) / t / t;
        float single3 = t / 2f - v / 2f / single1;
        float single4 = t / 2f - v / 2f / single2;
        if (single3 >= 0f && single4 >= 0f)
        {
            if (Mathf.Abs(single1) < Mathf.Abs(single2))
            {
                return single1;
            }
            return single2;
        }
        if (single3 >= 0f)
        {
            return single1;
        }
        if (single4 >= 0f)
        {
            return single2;
        }
        Debug.LogError(string.Format("did not find possitive solutions AccelerationFromTime({0} {1} {2})", v, x, t));
        return 10000f;
    }

    public static Vector3 Solve(Vector3 offset, Vector3 velocity, float maxAcceleration, float deadZone)
    {
        if (offset == Vector3.zero)
        {
            return Vector3.ClampMagnitude(-velocity / Time.fixedDeltaTime, maxAcceleration);
        }
        if (deadZone == 0f)
        {
            throw new ArgumentException("deadZone can't be 0", "deadZone");
        }
        if (maxAcceleration == 0f)
        {
            throw new ArgumentException("maxAcceleration can't be 0", "maxAcceleration");
        }
        float _magnitude = offset.magnitude;
        float single = Vector3.Dot(offset.normalized, velocity);
        Vector3 vector3 = velocity - (single * offset.normalized);
        float _magnitude1 = vector3.magnitude;
        maxAcceleration *= Mathf.Lerp(0f, 1f, offset.magnitude / deadZone);
        float single1 = velocity.magnitude / maxAcceleration;
        for (int i = 0; i < 5; i++)
        {
            if (single1 == 0f || _magnitude1 == 0f)
            {
                ConstantAccelerationControl.aOff = 0f;
            }
            else if (_magnitude1 >= maxAcceleration * single1)
            {
                ConstantAccelerationControl.aOff = ConstantAccelerationControl.AccelerationFromTime(_magnitude1, 0f, single1);
            }
            else
            {
                ConstantAccelerationControl.aOff = -_magnitude1 / single1;
            }
            ConstantAccelerationControl.aOff = Mathf.Clamp(ConstantAccelerationControl.aOff, -maxAcceleration * 0.99f, maxAcceleration * 0.99f);
            ConstantAccelerationControl.aOn = Mathf.Sqrt(maxAcceleration * maxAcceleration - ConstantAccelerationControl.aOff * ConstantAccelerationControl.aOff);
            float single2 = ConstantAccelerationControl.TimeFromAcceleration(single, _magnitude, ConstantAccelerationControl.aOn);
            float single3 = ConstantAccelerationControl.TimeFromAcceleration(single, _magnitude, -ConstantAccelerationControl.aOn);
            if (single2 < 0f || single2 >= single3)
            {
                single1 = single3;
                ConstantAccelerationControl.aOn = -ConstantAccelerationControl.aOn;
            }
            else
            {
                single1 = single2;
            }
        }
        if (single1 < Time.fixedDeltaTime)
        {
            ConstantAccelerationControl.aOn = ConstantAccelerationControl.aOn * (single1 / Time.fixedDeltaTime);
            ConstantAccelerationControl.aOff = ConstantAccelerationControl.aOff * (single1 / Time.fixedDeltaTime);
        }
        return (offset.normalized * ConstantAccelerationControl.aOn) + (vector3.normalized * ConstantAccelerationControl.aOff);
    }

    public static float TimeFromAcceleration(float v, float x, float a)
    {
        if (a == 0f)
        {
            if (x == 0f)
            {
                return 0f;
            }
            if (x * v <= 0f)
            {
                return 10000f;
            }
            return x / v;
        }
        float single = 2f * v * v + 4f * a * x;
        if (single < 0f)
        {
            return 10000f;
        }
        float single1 = Mathf.Sqrt(single);
        float single2 = (-v + single1) / a;
        float single3 = (-v - single1) / a;
        float single4 = single2 / 2f - v / 2f / a;
        float single5 = single3 / 2f - v / 2f / a;
        if (single4 >= 0f && single5 >= 0f && single2 >= 0f && single3 >= 0f)
        {
            return Mathf.Min(single2, single3);
        }
        if (single4 >= 0f && single2 >= 0f)
        {
            return single2;
        }
        if (single5 >= 0f && single3 >= 0f)
        {
            return single3;
        }
        return 10000f;
    }
}