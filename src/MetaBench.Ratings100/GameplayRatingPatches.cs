namespace MetaBench.Ratings100;

internal static class GameplayRatingPatches
{
    internal static bool Takeover { get; set; }
    internal static float MarketExponent { get; set; } = MarketRatingModel.DefaultExponent;
    internal static float MarketMinMultiplier { get; set; } = MarketRatingModel.DefaultMinMultiplier;
    internal static float MarketMaxMultiplier { get; set; } = MarketRatingModel.DefaultMaxMultiplier;

    internal static bool PlayerRatingPrefix(DataPlayer __instance, ref float __result)
    {
        if (!Takeover || __instance == null)
        {
            return true;
        }

        return TryReplace(() => GameplayRating.Calculate(__instance), ref __result);
    }

    internal static bool RoleRatingPrefix(DataPlayer player, ref float __result)
    {
        if (!Takeover || player == null)
        {
            return true;
        }

        return TryReplace(() => GameplayRating.Calculate(player), ref __result);
    }

    internal static bool PowerAsRolePrefix(DataPlayer __instance, PlayerMainRole role, bool asIGL, ref float __result)
    {
        if (!Takeover || __instance == null)
        {
            return true;
        }

        return TryReplace(() => GameplayRating.CalculateAsRole(__instance, role, asIGL), ref __result);
    }

    internal static void MarketSkillPostfix(DataPlayer p, ref float __result)
    {
        if (!Takeover || p == null)
        {
            return;
        }

        try
        {
            __result = MarketRatingModel.ReplaceSkillComponent(
                __result,
                GameplayRating.CalculateNative(p),
                GameplayRating.Calculate(p),
                MarketExponent,
                MarketMinMultiplier,
                MarketMaxMultiplier);
        }
        catch (System.Exception exception)
        {
            Ratings100Plugin.LogGameplayFallback(exception);
        }
    }

    private static bool TryReplace(System.Func<float> calculate, ref float result)
    {
        try
        {
            result = calculate();
            return false;
        }
        catch (System.Exception exception)
        {
            Ratings100Plugin.LogGameplayFallback(exception);
            return true;
        }
    }
}
