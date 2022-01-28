using System;
using UnityEngine;

public static class Ease
{
    public static float clerp(float start, float end, float value)
    {
        float single = 0f;
        float single1 = 360f;
        float single2 = Mathf.Abs((single1 - single) / 2f);
        float single3 = 0f;
        float single4 = 0f;
        if (end - start < -single2)
        {
            single4 = (single1 - start + end) * value;
            single3 = start + single4;
        }
        else if (end - start <= single2)
        {
            single3 = start + (end - start) * value;
        }
        else
        {
            single4 = -(single1 - end + start) * value;
            single3 = start + single4;
        }
        return single3;
    }

    public static float easeInBack(float start, float end, float value)
    {
        end -= start;
        value /= 1f;
        float single = 1.70158f;
        return end * value * value * ((single + 1f) * value - single) + start;
    }

    public static float easeInBounce(float start, float end, float value)
    {
        end -= start;
        float single = 1f;
        return end - Ease.easeOutBounce(0f, end, single - value) + start;
    }

    public static float easeInCirc(float start, float end, float value)
    {
        end -= start;
        return -end * (Mathf.Sqrt(1f - value * value) - 1f) + start;
    }

    public static float easeInCubic(float start, float end, float value)
    {
        end -= start;
        return end * value * value * value + start;
    }

    public static float easeInElastic(float start, float end, float value)
    {
        end -= start;
        float single = 1f;
        float single1 = single * 0.3f;
        float single2 = 0f;
        float single3 = 0f;
        if (value == 0f)
        {
            return start;
        }
        float single4 = value / single;
        value = single4;
        if (single4 == 1f)
        {
            return start + end;
        }
        if (single3 == 0f || single3 < Mathf.Abs(end))
        {
            single3 = end;
            single2 = single1 / 4f;
        }
        else
        {
            single2 = single1 / 6.28318548f * Mathf.Asin(end / single3);
        }
        float single5 = value - 1f;
        value = single5;
        return -(single3 * Mathf.Pow(2f, 10f * single5) * Mathf.Sin((value * single - single2) * 6.28318548f / single1)) + start;
    }

    public static float easeInExpo(float start, float end, float value)
    {
        end -= start;
        return end * Mathf.Pow(2f, 10f * (value / 1f - 1f)) + start;
    }

    public static float easeInOutBack(float start, float end, float value)
    {
        float single = 1.70158f;
        end -= start;
        value /= 0.5f;
        if (value < 1f)
        {
            single *= 1.525f;
            return end / 2f * (value * value * ((single + 1f) * value - single)) + start;
        }
        value -= 2f;
        single *= 1.525f;
        return end / 2f * (value * value * ((single + 1f) * value + single) + 2f) + start;
    }

    public static float easeInOutBounce(float start, float end, float value)
    {
        end -= start;
        float single = 1f;
        if (value < single / 2f)
        {
            return Ease.easeInBounce(0f, end, value * 2f) * 0.5f + start;
        }
        return Ease.easeOutBounce(0f, end, value * 2f - single) * 0.5f + end * 0.5f + start;
    }

    public static float easeInOutCirc(float start, float end, float value)
    {
        value /= 0.5f;
        end -= start;
        if (value < 1f)
        {
            return -end / 2f * (Mathf.Sqrt(1f - value * value) - 1f) + start;
        }
        value -= 2f;
        return end / 2f * (Mathf.Sqrt(1f - value * value) + 1f) + start;
    }

    public static float easeInOutCubic(float start, float end, float value)
    {
        value /= 0.5f;
        end -= start;
        if (value < 1f)
        {
            return end / 2f * value * value * value + start;
        }
        value -= 2f;
        return end / 2f * (value * value * value + 2f) + start;
    }

    public static float easeInOutElastic(float start, float end, float value)
    {
        end -= start;
        float single = 1f;
        float single1 = single * 0.3f;
        float single2 = 0f;
        float single3 = 0f;
        if (value == 0f)
        {
            return start;
        }
        float single4 = value / (single / 2f);
        value = single4;
        if (single4 == 2f)
        {
            return start + end;
        }
        if (single3 == 0f || single3 < Mathf.Abs(end))
        {
            single3 = end;
            single2 = single1 / 4f;
        }
        else
        {
            single2 = single1 / 6.28318548f * Mathf.Asin(end / single3);
        }
        if (value < 1f)
        {
            float single5 = value - 1f;
            value = single5;
            return -0.5f * (single3 * Mathf.Pow(2f, 10f * single5) * Mathf.Sin((value * single - single2) * 6.28318548f / single1)) + start;
        }
        float single6 = value - 1f;
        value = single6;
        return single3 * Mathf.Pow(2f, -10f * single6) * Mathf.Sin((value * single - single2) * 6.28318548f / single1) * 0.5f + end + start;
    }

    public static float easeInOutExpo(float start, float end, float value)
    {
        value /= 0.5f;
        end -= start;
        if (value < 1f)
        {
            return end / 2f * Mathf.Pow(2f, 10f * (value - 1f)) + start;
        }
        value -= 1f;
        return end / 2f * (-Mathf.Pow(2f, -10f * value) + 2f) + start;
    }

    public static float easeInOutQuad(float start, float end, float value)
    {
        value /= 0.5f;
        end -= start;
        if (value < 1f)
        {
            return end / 2f * value * value + start;
        }
        value -= 1f;
        return -end / 2f * (value * (value - 2f) - 1f) + start;
    }

    public static float easeInOutQuart(float start, float end, float value)
    {
        value /= 0.5f;
        end -= start;
        if (value < 1f)
        {
            return end / 2f * value * value * value * value + start;
        }
        value -= 2f;
        return -end / 2f * (value * value * value * value - 2f) + start;
    }

    public static float easeInOutQuint(float start, float end, float value)
    {
        value /= 0.5f;
        end -= start;
        if (value < 1f)
        {
            return end / 2f * value * value * value * value * value + start;
        }
        value -= 2f;
        return end / 2f * (value * value * value * value * value + 2f) + start;
    }

    public static float easeInOutSine(float start, float end, float value)
    {
        end -= start;
        return -end / 2f * (Mathf.Cos(3.14159274f * value / 1f) - 1f) + start;
    }

    public static float easeInQuad(float start, float end, float value)
    {
        end -= start;
        return end * value * value + start;
    }

    public static float easeInQuart(float start, float end, float value)
    {
        end -= start;
        return end * value * value * value * value + start;
    }

    public static float easeInQuint(float start, float end, float value)
    {
        end -= start;
        return end * value * value * value * value * value + start;
    }

    public static float easeInSine(float start, float end, float value)
    {
        end -= start;
        return -end * Mathf.Cos(value / 1f * 1.57079637f) + end + start;
    }

    public static float easeOutBack(float start, float end, float value)
    {
        float single = 1.70158f;
        end -= start;
        value = value / 1f - 1f;
        return end * (value * value * ((single + 1f) * value + single) + 1f) + start;
    }

    public static float easeOutBounce(float start, float end, float value)
    {
        value /= 1f;
        end -= start;
        if (value < 0.363636374f)
        {
            return end * (7.5625f * value * value) + start;
        }
        if (value < 0.727272749f)
        {
            value -= 0.545454562f;
            return end * (7.5625f * value * value + 0.75f) + start;
        }
        if ((double)value < 0.909090909090909)
        {
            value -= 0.8181818f;
            return end * (7.5625f * value * value + 0.9375f) + start;
        }
        value -= 0.954545438f;
        return end * (7.5625f * value * value + 0.984375f) + start;
    }

    public static float easeOutCirc(float start, float end, float value)
    {
        value -= 1f;
        end -= start;
        return end * Mathf.Sqrt(1f - value * value) + start;
    }

    public static float easeOutCubic(float start, float end, float value)
    {
        value -= 1f;
        end -= start;
        return end * (value * value * value + 1f) + start;
    }

    public static float easeOutElastic(float start, float end, float value)
    {
        end -= start;
        float single = 1f;
        float single1 = single * 0.3f;
        float single2 = 0f;
        float single3 = 0f;
        if (value == 0f)
        {
            return start;
        }
        float single4 = value / single;
        value = single4;
        if (single4 == 1f)
        {
            return start + end;
        }
        if (single3 == 0f || single3 < Mathf.Abs(end))
        {
            single3 = end;
            single2 = single1 / 4f;
        }
        else
        {
            single2 = single1 / 6.28318548f * Mathf.Asin(end / single3);
        }
        return single3 * Mathf.Pow(2f, -10f * value) * Mathf.Sin((value * single - single2) * 6.28318548f / single1) + end + start;
    }

    public static float easeOutExpo(float start, float end, float value)
    {
        end -= start;
        return end * (-Mathf.Pow(2f, -10f * value / 1f) + 1f) + start;
    }

    public static float easeOutQuad(float start, float end, float value)
    {
        end -= start;
        return -end * value * (value - 2f) + start;
    }

    public static float easeOutQuart(float start, float end, float value)
    {
        value -= 1f;
        end -= start;
        return -end * (value * value * value * value - 1f) + start;
    }

    public static float easeOutQuint(float start, float end, float value)
    {
        value -= 1f;
        end -= start;
        return end * (value * value * value * value * value + 1f) + start;
    }

    public static float easeOutSine(float start, float end, float value)
    {
        end -= start;
        return end * Mathf.Sin(value / 1f * 1.57079637f) + start;
    }

    public static float linear(float start, float end, float value)
    {
        return Mathf.Lerp(start, end, value);
    }

    public static float punch(float amplitude, float value)
    {
        float single = 9f;
        if (value == 0f)
        {
            return 0f;
        }
        if (value == 1f)
        {
            return 0f;
        }
        float single1 = 0.3f;
        single = single1 / 6.28318548f * Mathf.Asin(0f);
        return amplitude * Mathf.Pow(2f, -10f * value) * Mathf.Sin((value * 1f - single) * 6.28318548f / single1);
    }

    public static float spring(float start, float end, float value)
    {
        value = Mathf.Clamp01(value);
        value = (Mathf.Sin(value * 3.14159274f * (0.2f + 2.5f * value * value * value)) * Mathf.Pow(1f - value, 2.2f) + value) * (1f + 1.2f * (1f - value));
        return start + (end - start) * value;
    }
}