using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Rooms;
using USCE.Scripts.Powers;

namespace USCE.Scripts.Patches;

[HarmonyPatch(typeof(CombatManager), "SetUpCombat")]
public static class MirrorImageCombatPatch
{
    public static void Postfix(CombatState state)
    {
        var instance = CombatManager.Instance;
        if (instance != null)
        {
            instance.CombatEnded += OnCombatEnded;
        }
    }

    private static void OnCombatEnded(CombatRoom room)
    {
        MirrorImagePower.ClearAll();

        var instance = CombatManager.Instance;
        if (instance != null)
        {
            instance.CombatEnded -= OnCombatEnded;
        }
    }
}
