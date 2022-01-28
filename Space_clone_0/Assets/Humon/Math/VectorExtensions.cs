using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public static class VectorExtensions
{
    public static Vector3 InvertZ(this Vector3 v2)
    {
        return new Vector3(v2.x, v2.y, -v2.z);
    }

    public static Vector2 Rotate(this Vector2 p, float angle)
    {
        float single = Mathf.Cos(angle);
        float single1 = Mathf.Sin(angle);
        return new Vector2(p.x * single - p.y * single1, p.x * single1 + p.y * single);
    }

    public static Vector3 Rotate(this Vector3 p, Vector3 axis, float angle)
    {
        return Quaternion.AngleAxis(angle, axis) * p;
    }

    public static Vector2 RotateCW90(this Vector2 p)
    {
        return new Vector2(p.y, -p.x);
    }

    public static Vector3 RotateY(this Vector3 p, float angle)
    {
        float single = Mathf.Cos(angle);
        float single1 = Mathf.Sin(angle);
        return new Vector3(p.x * single - p.z * single1, p.y, p.x * single1 + p.z * single);
    }

    public static Vector3 RotateYCW90(this Vector3 p)
    {
        return new Vector3(p.z, p.y, -p.x);
    }

    public static Vector3 RotateZCW90(this Vector3 p)
    {
        return new Vector3(p.y, -p.x, p.z);
    }

    public static Vector3 SetX(this Vector3 v2, float x)
    {
        return new Vector3(x, v2.y, v2.z);
    }

    public static Vector3 SetY(this Vector3 v2, float y)
    {
        return new Vector3(v2.x, y, v2.z);
    }

    public static Vector3 SetZ(this Vector3 v2, float z)
    {
        return new Vector3(v2.x, v2.y, z);
    }

    public static Vector2 To2D(this Vector3 v3)
    {
        return new Vector2(v3.x, v3.z);
    }

    public static Vector3 To3D(this Vector2 v2)
    {
        return new Vector3(v2.x, 0f, v2.y);
    }

    public static Vector3 To3D(this Vector2 v2, float y)
    {
        return new Vector3(v2.x, y, v2.y);
    }

    public static Vector3 XYtoXZ(this Vector3 v3)
    {
        return new Vector3(v3.x, -v3.z, v3.y);
    }

    public static Vector3 XZtoXY(this Vector3 v3)
    {
        return new Vector3(v3.x, v3.z, -v3.y);
    }

    public static Vector3 ZeroY(this Vector3 v2)
    {
        return new Vector3(v2.x, 0f, v2.z);
    }
}