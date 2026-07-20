using System;
using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;

namespace MetaBench.Ratings100;

[BepInPlugin(PluginGuid, PluginName, PluginVersion)]
public sealed class Ratings100Plugin : BasePlugin
{
    public const string PluginGuid = "gg.metabench.em26.ratings100";
    public const string PluginName = "MetaBench Ratings100";
    public const string PluginVersion = "0.2.0-beta.2";

    private Harmony? _harmony;
    private static bool _loggedGameplayFallback;

    internal static ManualLogSource ModLog { get; private set; } = null!;

    internal static void LogGameplayFallback(Exception exception)
    {
        if (_loggedGameplayFallback)
        {
            return;
        }

        _loggedGameplayFallback = true;
        ModLog.LogWarning($"[{PluginName}] role-aware calculation failed; native EM26 calculation will be used. First error: {exception.Message}");
    }

    public override void Load()
    {
        ModLog = Log;
        _harmony = new Harmony(PluginGuid);

        GameplayRatingPatches.Takeover = Config.Bind(
            "Gameplay",
            "RoleAwareRatings",
            true,
            "Replace EM26 player ratings with role-aware ratings. Disable to keep the UI-only 0-100 conversion.").Value;

        var uiHooks = RatingPatchInstaller.Install(_harmony, Log);
        var gameplayHooks = RatingPatchInstaller.InstallGameplay(_harmony, Log);
        Log.LogInfo($"[{PluginName}] loaded: {uiHooks} UI hooks, {gameplayHooks} gameplay hooks; role-aware takeover={GameplayRatingPatches.Takeover}.");
    }

    public override bool Unload()
    {
        try
        {
            _harmony?.UnpatchSelf();
            return true;
        }
        catch (Exception exception)
        {
            Log.LogWarning($"[{PluginName}] failed to remove Harmony patches: {exception.Message}");
            return false;
        }
    }
}
