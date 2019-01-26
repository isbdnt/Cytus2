using UnityEngine;

namespace Cytus2
{
    public static class MathUtility
    {
        public static float Tween(TweenEaseType tweenEaseType, float elapsed, float duration)
        {
            if (duration <= 0f) return 1f;
            return TweenInternal(tweenEaseType, Mathf.Min(elapsed, duration) / duration);
        }

        private static float TweenInternal(TweenEaseType tweenEaseType, float t)
        {
            switch (tweenEaseType)
            {
                case TweenEaseType.Linear:
                    return t;

                case TweenEaseType.InQuad:
                    return t * t;

                case TweenEaseType.OutQuad:
                    return -t * (t - 2f);

                case TweenEaseType.InOutQuad:
                    t *= 2f;
                    if (t < 1f) return 0.5f * t * t;
                    t--;
                    return -0.5f * (t * (t - 2f) - 1f);

                case TweenEaseType.InCubic:
                    return t * t * t;

                case TweenEaseType.OutCubic:
                    t--;
                    return (t * t * t + 1f);

                case TweenEaseType.InOutCubic:
                    t *= 2f;
                    if (t < 1f) return 0.5f * t * t * t;
                    t -= 2f;
                    return 0.5f * (t * t * t + 2f);

                case TweenEaseType.InQuart:
                    return -t * t * t * t;

                case TweenEaseType.OutQuart:
                    t--;
                    return -(t * t * t * t - 1f);

                case TweenEaseType.InOutQuart:
                    t *= 2f;
                    if (t < 1f) return 0.5f * t * t * t * t;
                    t -= 2f;
                    return -0.5f * (t * t * t * t - 2f);

                case TweenEaseType.InQuint:
                    return t * t * t * t * t;

                case TweenEaseType.OutQuint:
                    t--;
                    return (t * t * t * t * t + 1f);

                case TweenEaseType.InOutQuint:
                    t *= 2f;
                    if (t < 1f) return 0.5f * t * t * t * t * t;
                    t -= 2f;
                    return 0.5f * (t * t * t * t * t + 2f);

                case TweenEaseType.InSine:
                    return -Mathf.Cos(t * (Mathf.PI / 2f)) + 1f;

                case TweenEaseType.OutSine:
                    return Mathf.Sin(t * (Mathf.PI / 2f));

                case TweenEaseType.InOutSine:
                    return -0.5f * (Mathf.Cos(Mathf.PI * t) - 1f);

                case TweenEaseType.InExpo:
                    return Mathf.Pow(2f, 10f * (t - 1f));

                case TweenEaseType.OutExpo:
                    return (-Mathf.Pow(2f, -10f * t) + 1f);

                case TweenEaseType.InOutExpo:
                    t *= 2f;
                    if (t < 1f) return 0.5f * Mathf.Pow(2, 10f * (t - 1f));
                    t--;
                    return 0.5f * (-Mathf.Pow(2f, -10f * t) + 2f);

                case TweenEaseType.InCirc:
                    return -(Mathf.Sqrt(1f - t * t) - 1f);

                case TweenEaseType.OutCirc:
                    t--;
                    return Mathf.Sqrt(1f - t * t);

                case TweenEaseType.InOutCirc:
                    t *= 2f;
                    if (t < 1f) return -0.5f * (Mathf.Sqrt(1f - t * t) - 1f);
                    t -= 2f;
                    return 0.5f * (Mathf.Sqrt(1f - t * t) + 1f);

                case TweenEaseType.InBack:
                    {
                        float s = 1.70158f;
                        return t * t * ((s + 1f) * t - 1.70158f);
                    }
                case TweenEaseType.OutBack:
                    {
                        float s = 1.70158f;
                        return 1 + (--t) * t * ((s + 1f) * t + s);
                    }
                case TweenEaseType.InOutBack:
                    if (t < 0.5f) return t * t * (7f * t - 2.5f) * 2f;
                    return 1f + (--t) * t * 2f * (7f * t + 2.5f);

                case TweenEaseType.InBounce:
                    return Mathf.Pow(2f, 6f * (t - 1f)) * Mathf.Abs(Mathf.Sin(t * Mathf.PI * 3.5f));

                case TweenEaseType.OutBounce:
                    {
                        if (t < 1f / 2.75f)
                        {
                            return (7.5625f * t * t);
                        }
                        else if (t < 2f / 2.75f)
                        {
                            return (7.5625f * (t -= 1.5f / 2.75f) * t + 0.75f);
                        }
                        else if (t < 2.5f / 2.75f)
                        {
                            return (7.5625f * (t -= 2.25f / 2.75f) * t + 0.9375f);
                        }
                        else
                        {
                            return (7.5625f * (t -= 2.625f / 2.75f) * t + 0.984375f);
                        }
                    }
                case TweenEaseType.InOutBounce:
                    if (t < 0.5f) return 8f * Mathf.Pow(2f, 8f * (t - 1)) * Mathf.Abs(Mathf.Sin(t * Mathf.PI * 7f));
                    return 1f - 8f * Mathf.Pow(2f, -8f * t) * Mathf.Abs(Mathf.Sin(t * Mathf.PI * 7f));

                case TweenEaseType.InElastic:
                    {
                        float t2 = t * t;
                        return t2 * t2 * Mathf.Sin(t * Mathf.PI * 4.5f);
                    }
                case TweenEaseType.OutElastic:
                    {
                        float t2 = (t - 1f) * (t - 1f);
                        return 1f - t2 * t2 * Mathf.Cos(t * Mathf.PI * 4.5f);
                    }
                case TweenEaseType.InOutElastic:
                    {
                        float t2;
                        if (t < 0.45f)
                        {
                            t2 = t * t;
                            return 8f * t2 * t2 * Mathf.Sin(t * Mathf.PI * 9f);
                        }
                        else if (t < 0.55f)
                        {
                            return 0.5f + 0.75f * Mathf.Sin(t * Mathf.PI * 4f);
                        }
                        else
                        {
                            t2 = (t - 1f) * (t - 1f);
                            return 1f - 8f * t2 * t2 * Mathf.Sin(t * Mathf.PI * 9f);
                        }
                    }
                default:
                    return 1f;
            }
        }
    }
}