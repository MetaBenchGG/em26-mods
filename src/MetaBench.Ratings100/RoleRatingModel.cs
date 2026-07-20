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
    Entry,
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
    public const float SecondaryRoleShare = 0.30f;
    public const float IglRoleShare = 0.60f;

    private static readonly IReadOnlyDictionary<RatingRole, Weight[]> Profiles =
        new Dictionary<RatingRole, Weight[]>
        {
            [RatingRole.Rifler] = Weights(
                (RatingAttribute.Skill, .18f), (RatingAttribute.Rifle, .22f),
                (RatingAttribute.Pistol, .06f), (RatingAttribute.Grenades, .05f),
                (RatingAttribute.Reaction, .12f), (RatingAttribute.Eyesight, .10f),
                (RatingAttribute.Creativity, .04f), (RatingAttribute.Clutch, .07f),
                (RatingAttribute.Tactic, .06f), (RatingAttribute.Teamwork, .03f),
                (RatingAttribute.Productivity, .04f), (RatingAttribute.StressResistance, .03f)),
            [RatingRole.AWPer] = Weights(
                (RatingAttribute.Skill, .12f), (RatingAttribute.AWP, .28f),
                (RatingAttribute.Pistol, .03f), (RatingAttribute.Reaction, .14f),
                (RatingAttribute.Eyesight, .16f), (RatingAttribute.Creativity, .04f),
                (RatingAttribute.Clutch, .09f), (RatingAttribute.Tactic, .06f),
                (RatingAttribute.Productivity, .04f), (RatingAttribute.StressResistance, .04f)),
            [RatingRole.Support] = Weights(
                (RatingAttribute.Skill, .12f), (RatingAttribute.Rifle, .13f),
                (RatingAttribute.Pistol, .04f), (RatingAttribute.Grenades, .17f),
                (RatingAttribute.Reaction, .07f), (RatingAttribute.Eyesight, .06f),
                (RatingAttribute.Creativity, .09f), (RatingAttribute.Clutch, .05f),
                (RatingAttribute.Tactic, .10f), (RatingAttribute.Teamwork, .10f),
                (RatingAttribute.Productivity, .04f), (RatingAttribute.StressResistance, .03f)),
            [RatingRole.Lurker] = Weights(
                (RatingAttribute.Skill, .14f), (RatingAttribute.Rifle, .18f),
                (RatingAttribute.Pistol, .05f), (RatingAttribute.Grenades, .06f),
                (RatingAttribute.Reaction, .09f), (RatingAttribute.Eyesight, .08f),
                (RatingAttribute.Creativity, .12f), (RatingAttribute.Clutch, .12f),
                (RatingAttribute.Tactic, .08f), (RatingAttribute.Teamwork, .02f),
                (RatingAttribute.Productivity, .03f), (RatingAttribute.StressResistance, .03f)),
            [RatingRole.SecondAWPer] = Weights(
                (RatingAttribute.Skill, .13f), (RatingAttribute.Rifle, .13f),
                (RatingAttribute.AWP, .18f), (RatingAttribute.Pistol, .04f),
                (RatingAttribute.Grenades, .05f), (RatingAttribute.Reaction, .11f),
                (RatingAttribute.Eyesight, .11f), (RatingAttribute.Creativity, .06f),
                (RatingAttribute.Clutch, .08f), (RatingAttribute.Tactic, .05f),
                (RatingAttribute.Teamwork, .02f), (RatingAttribute.Productivity, .02f),
                (RatingAttribute.StressResistance, .02f)),
            [RatingRole.Entry] = Weights(
                (RatingAttribute.Skill, .16f), (RatingAttribute.Rifle, .20f),
                (RatingAttribute.Pistol, .06f), (RatingAttribute.Grenades, .06f),
                (RatingAttribute.Reaction, .16f), (RatingAttribute.Eyesight, .10f),
                (RatingAttribute.Strength, .03f), (RatingAttribute.Endurance, .03f),
                (RatingAttribute.Creativity, .05f), (RatingAttribute.Clutch, .03f),
                (RatingAttribute.Tactic, .03f), (RatingAttribute.Teamwork, .03f),
                (RatingAttribute.Productivity, .04f), (RatingAttribute.StressResistance, .02f)),
            [RatingRole.IGL] = Weights(
                (RatingAttribute.Skill, .08f), (RatingAttribute.Rifle, .08f),
                (RatingAttribute.Pistol, .03f), (RatingAttribute.Grenades, .09f),
                (RatingAttribute.Reaction, .04f), (RatingAttribute.Eyesight, .04f),
                (RatingAttribute.Creativity, .10f), (RatingAttribute.Clutch, .05f),
                (RatingAttribute.Tactic, .16f), (RatingAttribute.Leader, .18f),
                (RatingAttribute.Teamwork, .08f), (RatingAttribute.Productivity, .04f),
                (RatingAttribute.StressResistance, .03f))
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

        var weaponRoleScore = Score(value, primaryRole);
        if (secondaryRole.HasValue && secondaryRole.Value != primaryRole)
        {
            weaponRoleScore = Blend(weaponRoleScore, Score(value, secondaryRole.Value), SecondaryRoleShare);
        }

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
