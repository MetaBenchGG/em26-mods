using System;

namespace MetaBench.Ratings100;

public static class MarketRatingModel
{
    public const float DefaultExponent = 3f;
    public const float DefaultMinMultiplier = 0.4f;
    public const float DefaultMaxMultiplier = 2.5f;

    public static float ReplaceSkillComponent(
        float nativeSkillComponent,
        float nativeRating,
        float roleAwareRating,
        float exponent = DefaultExponent,
        float minMultiplier = DefaultMinMultiplier,
        float maxMultiplier = DefaultMaxMultiplier)
    {
        if (!IsFinite(nativeSkillComponent) || nativeSkillComponent < 0f ||
            !IsFinite(nativeRating) || nativeRating <= 0f ||
            !IsFinite(roleAwareRating) || roleAwareRating < 0f)
        {
            return nativeSkillComponent;
        }

        exponent = Clamp(exponent, 0.1f, 8f);
        minMultiplier = Clamp(minMultiplier, 0.01f, 1f);
        maxMultiplier = Math.Max(1f, maxMultiplier);

        var ratio = roleAwareRating / nativeRating;
        var multiplier = (float)Math.Pow(ratio, exponent);
        multiplier = Clamp(multiplier, minMultiplier, maxMultiplier);
        return nativeSkillComponent * multiplier;
    }

    private static bool IsFinite(float value) => !float.IsNaN(value) && !float.IsInfinity(value);

    private static float Clamp(float value, float min, float max) => Math.Max(min, Math.Min(max, value));
}
