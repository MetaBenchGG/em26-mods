using System;

namespace MetaBench.Ratings100;

internal static class GameplayRating
{
    internal static float Calculate(DataPlayer player)
    {
        if (player == null)
        {
            return 0f;
        }

        return Calculate(player, ToPrimaryRole(player.MainRole), ToSecondaryRole(player.SecondaryRole), player.IsIGL);
    }

    internal static float CalculateAsRole(DataPlayer player, PlayerMainRole role, bool asIgl)
    {
        if (player == null)
        {
            return 0f;
        }

        return Calculate(player, ToPrimaryRole(role), ToSecondaryRole(player.SecondaryRole), asIgl);
    }

    internal static float CalculateNative(DataPlayer player)
    {
        if (player == null)
        {
            return 0f;
        }

        return RatingEngine.ComputeRating(player, RatingEngine.ResolveRole(player));
    }

    private static float Calculate(DataPlayer player, RatingRole primary, RatingRole? secondary, bool isIgl) =>
        RoleRatingModel.Calculate(attribute => player.GetEffectiveAttribute(ToGameAttribute(attribute)), primary, secondary, isIgl);

    private static RatingRole ToPrimaryRole(PlayerMainRole role) => role switch
    {
        PlayerMainRole.AWPer => RatingRole.AWPer,
        _ => RatingRole.Rifler
    };

    private static RatingRole? ToSecondaryRole(PlayerSecondaryRole role) => role switch
    {
        PlayerSecondaryRole.Support => RatingRole.Support,
        PlayerSecondaryRole.Lurker => RatingRole.Lurker,
        PlayerSecondaryRole.Sniper => RatingRole.SecondAWPer,
        _ => null
    };

    private static AttributeType ToGameAttribute(RatingAttribute attribute) => attribute switch
    {
        RatingAttribute.Skill => AttributeType.Skill,
        RatingAttribute.Rifle => AttributeType.Rifle,
        RatingAttribute.AWP => AttributeType.Awp,
        RatingAttribute.Pistol => AttributeType.Pistol,
        RatingAttribute.Grenades => AttributeType.Grenades,
        RatingAttribute.Reaction => AttributeType.Reaction,
        RatingAttribute.Eyesight => AttributeType.Eyesight,
        RatingAttribute.Strength => AttributeType.Strength,
        RatingAttribute.Endurance => AttributeType.Endurance,
        RatingAttribute.Creativity => AttributeType.Creativity,
        RatingAttribute.Clutch => AttributeType.Clutch,
        RatingAttribute.Tactic => AttributeType.Tactic,
        RatingAttribute.Leader => AttributeType.Leader,
        RatingAttribute.Teamwork => AttributeType.Teamwork,
        RatingAttribute.Productivity => AttributeType.Productivity,
        RatingAttribute.StressResistance => AttributeType.StressResistance,
        _ => throw new ArgumentOutOfRangeException(nameof(attribute), attribute, null)
    };
}
