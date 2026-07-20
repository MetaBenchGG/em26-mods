using System;
using System.Collections.Generic;
using System.Reflection;
using BepInEx.Logging;
using HarmonyLib;

namespace MetaBench.Ratings100;

internal static class RatingPatchInstaller
{
    internal static int Install(Harmony harmony, ManualLogSource log)
    {
        var installed = 0;

        installed += Patch(harmony, log, typeof(SquadPlayerRow), "Init", new[] { typeof(DataPlayer) }, nameof(RatingUiPatches.SquadPlayerRowPostfix));
        installed += Patch(harmony, log, typeof(SquadStaffRow), "Init", new[] { typeof(DataStaff) }, nameof(RatingUiPatches.SquadStaffRowPostfix));
        installed += Patch(harmony, log, typeof(PlayerOverview), "Init", new[] { typeof(DataUser) }, nameof(RatingUiPatches.PlayerOverviewPostfix));
        installed += Patch(harmony, log, typeof(PlayerOverview), "Renew", new[] { typeof(DataUser) }, nameof(RatingUiPatches.PlayerOverviewPostfix));
        installed += Patch(harmony, log, typeof(StaffOverview), "Init", new[] { typeof(DataUser) }, nameof(RatingUiPatches.StaffOverviewPostfix));
        installed += Patch(harmony, log, typeof(ScoutingRow), "Init", new[] { typeof(DataUser) }, nameof(RatingUiPatches.ScoutingRowPostfix));
        installed += Patch(harmony, log, typeof(TransferListMemberRow), "Setup", new[] { typeof(DataUser) }, nameof(RatingUiPatches.TransferListMemberRowPostfix));
        installed += Patch(harmony, log, typeof(TransferRow), "Init", new[] { typeof(ContractOffer) }, nameof(RatingUiPatches.TransferRowPostfix));
        installed += Patch(harmony, log, typeof(OrganizationCEOView), "Setup", new[] { typeof(DataStaff) }, nameof(RatingUiPatches.OrganizationCeoPostfix));
        installed += Patch(harmony, log, typeof(MemberItemView), "Setup", new[] { typeof(bool), typeof(string), typeof(string), typeof(string), typeof(string), typeof(float), typeof(string) }, nameof(RatingUiPatches.MemberItemPostfix));
        installed += Patch(harmony, log, typeof(HomePlayerRow), "Init", new[] { typeof(int), typeof(string), typeof(string), typeof(int), typeof(string) }, nameof(RatingUiPatches.HomePlayerRowPostfix));
        installed += Patch(harmony, log, typeof(PlayerInScouting), "Init", null, nameof(RatingUiPatches.LegacyScoutingPostfix));

        installed += Patch(harmony, log, typeof(AttributeLine), "Setup", new[] { typeof(float), typeof(AttributeType), typeof(string), typeof(float), typeof(bool), typeof(bool) }, nameof(RatingUiPatches.AttributeLinePostfix));
        installed += Patch(harmony, log, typeof(AttributeLine), "SetupComparison", new[] { typeof(float), typeof(float), typeof(AttributeType) }, nameof(RatingUiPatches.AttributeLinePostfix));
        installed += Patch(harmony, log, typeof(AttributeLineTrainingShort), "Init", new[] { typeof(string), typeof(float), typeof(float) }, nameof(RatingUiPatches.AttributeTrainingShortPostfix));
        installed += Patch(harmony, log, typeof(AttributeLineTrainingPopup), "Init", new[] { typeof(AttributeContext), typeof(TrainingPopup) }, nameof(RatingUiPatches.AttributeTrainingPopupPostfix));
        installed += Patch(harmony, log, typeof(AttributeComparsion), "Init", null, nameof(RatingUiPatches.AttributeComparisonPostfix));

        return installed;
    }

    internal static int InstallGameplay(Harmony harmony, ManualLogSource log)
    {
        var installed = 0;
        installed += PatchMethod(harmony, log, AccessTools.PropertyGetter(typeof(DataPlayer), nameof(DataPlayer.Rating)), nameof(GameplayRatingPatches.PlayerRatingPrefix));
        installed += PatchMethod(harmony, log, AccessTools.Method(typeof(RatingEngine), nameof(RatingEngine.GetRoleRating), new[] { typeof(DataPlayer) }), nameof(GameplayRatingPatches.RoleRatingPrefix));
        installed += PatchMethod(harmony, log, AccessTools.Method(typeof(DataPlayer), nameof(DataPlayer.CalculatePowerAsRole), new[] { typeof(PlayerMainRole), typeof(bool) }), nameof(GameplayRatingPatches.PowerAsRolePrefix));
        installed += PatchPostfix(harmony, log, AccessTools.Method(typeof(DataPlayer), nameof(DataPlayer.GetSkillComp), new[] { typeof(DataPlayer) }), nameof(GameplayRatingPatches.MarketSkillPostfix));
        return installed;
    }

    private static int Patch(
        Harmony harmony,
        ManualLogSource log,
        Type type,
        string methodName,
        Type[]? parameterTypes,
        string postfixName)
    {
        try
        {
            MethodInfo? original;
            if (parameterTypes == null)
            {
                var matches = new List<MethodInfo>();
                foreach (var method in AccessTools.GetDeclaredMethods(type))
                {
                    if (method.Name == methodName)
                    {
                        matches.Add(method);
                    }
                }

                if (matches.Count != 1)
                {
                    log.LogWarning($"[MetaBench Ratings100] {type.Name}.{methodName}: expected one overload, found {matches.Count}; hook skipped.");
                    return 0;
                }

                original = matches[0];
            }
            else
            {
                original = AccessTools.Method(type, methodName, parameterTypes);
            }

            var postfix = AccessTools.Method(typeof(RatingUiPatches), postfixName);
            if (original == null || postfix == null)
            {
                log.LogWarning($"[MetaBench Ratings100] missing hook target {type.Name}.{methodName}; hook skipped.");
                return 0;
            }

            harmony.Patch(original, postfix: new HarmonyMethod(postfix));
            return 1;
        }
        catch (Exception exception)
        {
            log.LogWarning($"[MetaBench Ratings100] failed to hook {type.Name}.{methodName}: {exception.Message}");
            return 0;
        }
    }

    private static int PatchMethod(Harmony harmony, ManualLogSource log, MethodInfo? original, string prefixName)
    {
        try
        {
            var prefix = AccessTools.Method(typeof(GameplayRatingPatches), prefixName);
            if (original == null || prefix == null)
            {
                log.LogWarning($"[MetaBench Ratings100] missing gameplay hook for {prefixName}; hook skipped.");
                return 0;
            }

            harmony.Patch(original, prefix: new HarmonyMethod(prefix));
            return 1;
        }
        catch (Exception exception)
        {
            log.LogWarning($"[MetaBench Ratings100] failed to install gameplay hook {prefixName}: {exception.Message}");
            return 0;
        }
    }

    private static int PatchPostfix(Harmony harmony, ManualLogSource log, MethodInfo? original, string postfixName)
    {
        try
        {
            var postfix = AccessTools.Method(typeof(GameplayRatingPatches), postfixName);
            if (original == null || postfix == null)
            {
                log.LogWarning($"[MetaBench Ratings100] missing gameplay hook for {postfixName}; hook skipped.");
                return 0;
            }

            harmony.Patch(original, postfix: new HarmonyMethod(postfix));
            return 1;
        }
        catch (Exception exception)
        {
            log.LogWarning($"[MetaBench Ratings100] failed to install gameplay hook {postfixName}: {exception.Message}");
            return 0;
        }
    }
}
