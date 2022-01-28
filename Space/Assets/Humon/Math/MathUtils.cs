﻿using System;
using UnityEngine;

public static class MathUtils
{
    public static Vector3 Wrap(Vector3 value, Vector3 size)
    {
        return new Vector3(MathUtils.Wrap(value.x, size.x), MathUtils.Wrap(value.y, size.y), MathUtils.Wrap(value.z, size.z));
    }

    public static float Wrap(float value, float size)
    {
        return value - Mathf.Floor(value / size) * size;
    }

    public static Vector3 WrapSigned(Vector3 value, Vector3 size)
    {
        return new Vector3(MathUtils.WrapSigned(value.x, size.x), MathUtils.WrapSigned(value.y, size.y), MathUtils.WrapSigned(value.z, size.z));
    }

    public static float WrapSigned(float value, float size)
    {
        return value - Mathf.Floor(value / size + 0.5f) * size;
    }
}