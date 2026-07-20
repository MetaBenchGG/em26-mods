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
    public const string PluginVersion = "0.1.2";

    private Harmony? _harmony;

    internal static ManualLogSource ModLog { get; private set; } = null!;

    public override void Load()
    {
        ModLog = Log;
        _harmony = new Harmony(PluginGuid);

        var installed = RatingPatchInstaller.Install(_harmony, Log);
        Log.LogInfo($"[{PluginName}] loaded: 1–20 ratings are displayed as 5–100; {installed} UI hooks installed.");
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
