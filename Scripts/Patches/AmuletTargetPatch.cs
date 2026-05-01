using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Nodes.Combat;
using USCE.Scripts.Cards;

namespace USCE.Scripts.Patches;

[HarmonyPatch(typeof(NMouseCardPlay))]
public static class AmuletMouseTargetPatch
{
    [HarmonyPatch("TargetSelection")]
    [HarmonyPrefix]
    public static bool TargetSelectionPrefix(NMouseCardPlay __instance, ref Task __result, TargetMode targetMode)
    {
        var card = __instance.Holder?.CardNode?.Model;
        if (card == null) return true;

        if (card.GetType() != typeof(Amulet)) return true;

        var combatState = card.CombatState;
        if (combatState == null) return true;

        var hittableEnemies = combatState.HittableEnemies;
        if (hittableEnemies.Count > 0) return true;

        __result = Task.CompletedTask;
        return false;
    }
}

[HarmonyPatch(typeof(NControllerCardPlay))]
public static class AmuletControllerTargetPatch
{
    private static readonly MethodInfo? TryPlayCardMethod = typeof(NCardPlay).GetMethod("TryPlayCard", BindingFlags.Instance | BindingFlags.NonPublic);

    [HarmonyPatch("SingleCreatureTargeting")]
    [HarmonyPrefix]
    public static bool SingleCreatureTargetingPrefix(NControllerCardPlay __instance, ref Task __result, TargetType targetType)
    {
        var card = __instance.Holder?.CardNode?.Model;
        if (card == null) return true;

        if (card.GetType() != typeof(Amulet)) return true;

        var combatState = card.CombatState;
        if (combatState == null) return true;

        var owner = card.Owner?.Creature;
        if (owner == null) return true;

        List<Creature> list = new List<Creature>();
        switch (targetType)
        {
            case TargetType.AnyEnemy:
                list = owner.CombatState?.GetOpponentsOf(owner)?.Where(c => c.IsHittable)?.ToList() ?? new List<Creature>();
                break;
            case TargetType.AnyAlly:
                list = card.CombatState?.PlayerCreatures?.Where(c => c.IsHittable && c != owner)?.ToList() ?? new List<Creature>();
                break;
        }
        if (list.Count > 0) return true;

        TryPlayCardMethod?.Invoke(__instance, new object?[] { null });
        __result = Task.CompletedTask;
        return false;
    }
}
