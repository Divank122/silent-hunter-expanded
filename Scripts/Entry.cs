using BaseLib.Patches.Localization;
using Godot.Bridge;
using HarmonyLib;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;

namespace USCE.Scripts;

[ModInitializer("Init")]
public class Entry
{
    public static void Init()
    {
        var harmony = new Harmony("sts2.usce");
        harmony.PatchAll();
        ScriptManagerBridge.LookupScriptsInAssembly(typeof(Entry).Assembly);
        SimpleLoc.EnableSimpleLoc("UltimateSilentCardExpansion");

        Log.Info("[USCE] Initialized!");
    }
}
