using System;
using System.Collections.Generic;

namespace MetaBench.Ratings100;

public enum RatingRole
{
    Rifler,
    AWPer,
    Support,
    Lurker,
    SecondAWPer,
    IGL
}

public enum RatingAttribute
{
    Skill,
    Rifle,
    AWP,
    Pistol,
    Grenades,
    Reaction,
    Eyesight,
    Strength,
    Endurance,
    Creativity,
    Clutch,
    Tactic,
    Leader,
    Teamwork,
    Productivity,
    StressResistance
}

public static class RoleRatingModel
{
    public const float IglRoleShare = 0.70f;

    private static readonly IReadOnlyDictionary<RatingRole, Weight[]> Profiles =
        new Dictionary<RatingRole, Weight[]>
        {
            [RatingRole.Rifler] = Weights(
                (RatingAttribute.Skill, .18f), (RatingAttribute.Rifle, .24f),
                (RatingAttribute.Pistol, .05f), (RatingAttribute.Grenades, .05f),
                (RatingAttribute.Reaction, .12f), (RatingAttribute.Eyesight, .10f),
                (RatingAttribute.Strength, .02f), (RatingAttribute.Endurance, .03f),
                (RatingAttribute.Creativity, .04f), (RatingAttribute.Clutch, .06f),
                (RatingAttribute.Tactic, .05f), (RatingAttribute.Teamwork, .02f),
                (RatingAttribute.Productivity, .02f), (RatingAttribute.StressResistance, .02f)),
            [RatingRole.AWPer] = Weights(
                (RatingAttribute.Skill, .14f), (RatingAttribute.AWP, .29f),
                (RatingAttribute.Pistol, .04f), (RatingAttribute.Grenades, .03f),
                (RatingAttribute.Reaction, .13f), (RatingAttribute.Eyesight, .15f),
                (RatingAttribute.Strength, .01f), (RatingAttribute.Endurance, .02f),
                (RatingAttribute.Creativity, .04f), (RatingAttribute.Clutch, .08f),
                (RatingAttribute.Tactic, .04f), (RatingAttribute.Productivity, .01f),
                (RatingAttribute.StressResistance, .02f)),
            [RatingRole.Support] = Weights(
                (RatingAttribute.Skill, .13f), (RatingAttribute.Rifle, .14f),
                (RatingAttribute.Pistol, .04f), (RatingAttribute.Grenades, .18f),
                (RatingAttribute.Reaction, .08f), (RatingAttribute.Eyesight, .07f),
                (RatingAttribute.Strength, .02f), (RatingAttribute.Endurance, .03f),
                (RatingAttribute.Creativity, .09f), (RatingAttribute.Clutch, .03f),
                (RatingAttribute.Tactic, .10f), (RatingAttribute.Teamwork, .07f),
                (RatingAttribute.Productivity, .01f), (RatingAttribute.StressResistance, .01f)),
            [RatingRole.Lurker] = Weights(
                (RatingAttribute.Skill, .15f), (RatingAttribute.Rifle, .19f),
                (RatingAttribute.Pistol, .04f), (RatingAttribute.Grenades, .05f),
                (RatingAttribute.Reaction, .09f), (RatingAttribute.Eyesight, .08f),
                (RatingAttribute.Strength, .01f), (RatingAttribute.Endurance, .02f),
                (RatingAttribute.Creativity, .12f), (RatingAttribute.Clutch, .12f),
                (RatingAttribute.Tactic, .08f), (RatingAttribute.Teamwork, .02f),
                (RatingAttribute.Productivity, .01f), (RatingAttribute.StressResistance, .02f)),
            [RatingRole.SecondAWPer] = Weights(
                (RatingAttribute.Skill, .14f), (RatingAttribute.Rifle, .14f),
                (RatingAttribute.AWP, .20f), (RatingAttribute.Pistol, .04f),
                (RatingAttribute.Grenades, .04f), (RatingAttribute.Reaction, .11f),
                (RatingAttribute.Eyesight, .12f), (RatingAttribute.Strength, .01f),
                (RatingAttribute.Endurance, .02f), (RatingAttribute.Creativity, .05f),
                (RatingAttribute.Clutch, .07f), (RatingAttribute.Tactic, .03f),
                (RatingAttribute.Teamwork, .01f), (RatingAttribute.Productivity, .01f),
                (RatingAttribute.StressResistance, .01f)),
            [RatingRole.IGL] = Weights(
                (RatingAttribute.Leader, .32f), (RatingAttribute.Tactic, .23f),
                (RatingAttribute.Creativity, .14f), (RatingAttribute.Teamwork, .13f),
                (RatingAttribute.StressResistance, .08f), (RatingAttribute.Productivity, .05f),
                (RatingAttribute.Clutch, .05f))
        };

    public static float Calculate(
        Func<RatingAttribute, float> value,
        RatingRole primaryRole,
        RatingRole? secondaryRole = null,
        bool isIgl = false)
    {
        if (value == null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        // Secondary roles in EM26 are complete rifler specializations, not a
        // small bonus added to a generic rifler profile. AWPers have no second role.
        var effectiveRole = primaryRole == RatingRole.Rifler && secondaryRole.HasValue
            ? secondaryRole.Value
            : primaryRole;
        var weaponRoleScore = Score(value, effectiveRole);

        var result = isIgl
            ? Blend(weaponRoleScore, Score(value, RatingRole.IGL), IglRoleShare)
            : weaponRoleScore;

        return Clamp(result, 0f, 20f);
    }

    public static float Score(Func<RatingAttribute, float> value, RatingRole role)
    {
        var total = 0f;
        foreach (var weight in Profiles[role])
        {
            total += Clamp(value(weight.Attribute), 0f, 20f) * weight.Value;
        }

        return total;
    }

    public static float GetWeightTotal(RatingRole role)
    {
        var total = 0f;
        foreach (var weight in Profiles[role])
        {
            total += weight.Value;
        }

        return total;
    }

    private static float Blend(float primary, float secondary, float secondaryShare) =>
        primary * (1f - secondaryShare) + secondary * secondaryShare;

    private static float Clamp(float value, float min, float max) =>
        Math.Max(min, Math.Min(max, value));

    private static Weight[] Weights(params (RatingAttribute Attribute, float Value)[] values)
    {
        var result = new Weight[values.Length];
        for (var i = 0; i < values.Length; i++)
        {
            result[i] = new Weight(values[i].Attribute, values[i].Value);
        }

        return result;
    }

    private readonly struct Weight
    {
        internal Weight(RatingAttribute attribute, float value)
        {
            Attribute = attribute;
            Value = value;
        }

        internal RatingAttribute Attribute { get; }
        internal float Value { get; }
    }
}
