using System.Linq;
using System.Reflection;
using BaseLib.Abstracts;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using USCE.Scripts.Powers;

namespace USCE.Scripts.Patches;

[HarmonyPatch(typeof(AccuracyPower), nameof(AccuracyPower.ModifyDamageAdditive))]
public static class AccuracyPowerEagleEyePatch
{
    private static readonly MethodInfo? CardTagsMethod = typeof(CardModel).GetProperty("Tags")?.GetMethod;

    public static bool Prefix(AccuracyPower __instance, Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? card, ref decimal __result)
    {
        var owner = __instance.Owner;
        
        if (owner != dealer)
        {
            __result = 0m;
            return false;
        }
        if (!props.IsPoweredAttack())
        {
            __result = 0m;
            return false;
        }
        if (card == null)
        {
            __result = 0m;
            return false;
        }

        var eagleEyePower = owner.GetPower<EagleEyeVisionPower>();
        if (eagleEyePower != null)
        {
            if (!card.Tags.Contains(CardTag.Shiv) && !card.Tags.Contains(CardTag.Strike))
            {
                __result = 0m;
                return false;
            }
        }
        else
        {
            if (!card.Tags.Contains(CardTag.Shiv))
            {
                __result = 0m;
                return false;
            }
        }

        __result = __instance.Amount;
        return false;
    }
}
