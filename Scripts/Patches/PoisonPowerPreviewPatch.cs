using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace USCE.Scripts.Patches;

[HarmonyPatch(typeof(PoisonPower), nameof(PoisonPower.CalculateTotalDamageNextTurn))]
public static class PoisonPowerPreviewPatch
{
    static bool Prefix(PoisonPower __instance, ref int __result)
    {
        var owner = __instance.Owner;
        if (owner == null || owner.Side != CombatSide.Player)
        {
            return true;
        }

        var intangiblePower = owner.GetPower<IntangiblePower>();
        if (intangiblePower == null)
        {
            return true;
        }

        int poisonAmount = __instance.Amount;
        int intangibleAmount = intangiblePower.Amount;
        
        int remainingIntangible = intangibleAmount - 1;
        
        if (remainingIntangible <= 0)
        {
            __result = CalculateRawPoisonDamage(__instance, poisonAmount);
            return false;
        }
        
        int triggerCount = GetTriggerCount(__instance, owner);
        int iterations = System.Math.Min(poisonAmount, triggerCount);
        
        __result = iterations;
        return false;
    }

    private static int GetTriggerCount(PoisonPower power, Creature owner)
    {
        IEnumerable<Creature> source = from c in owner.CombatState.GetOpponentsOf(owner)
            where c.IsAlive
            select c;
        return System.Math.Min(power.Amount, 1 + source.Sum(a => a.GetPowerAmount<AccelerantPower>()));
    }

    private static int CalculateRawPoisonDamage(PoisonPower power, int poisonAmount)
    {
        var owner = power.Owner;
        int triggerCount = GetTriggerCount(power, owner);
        int iterations = System.Math.Min(poisonAmount, triggerCount);
        int totalDamage = 0;
        
        for (int i = 0; i < iterations; i++)
        {
            int damage = poisonAmount - i;
            totalDamage += damage;
        }
        
        return totalDamage;
    }
}
