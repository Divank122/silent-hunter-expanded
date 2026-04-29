using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using USCE.Scripts.Powers;

namespace USCE.Scripts.Patches;

[HarmonyPatch(typeof(Hook))]
public static class ThirstyTrackerPatch
{
    [HarmonyPatch(nameof(Hook.BeforeCardPlayed))]
    [HarmonyPrefix]
    public static void BeforeCardPlayed(CombatState combatState, CardPlay cardPlay)
    {
        if (cardPlay.Card != null && cardPlay.Card.Keywords.Contains(USCEKeywords.Thirsty))
        {
            var owner = cardPlay.Card.Owner;
            if (owner != null && owner.Creature.GetPower<ThirstyPower>() == null)
            {
                PowerCmd.Apply<ThirstyPower>(owner.Creature, 1, null, null).GetAwaiter().GetResult();
            }
        }
    }

    [HarmonyPatch(nameof(Hook.AfterCombatEnd))]
    [HarmonyPostfix]
    public static void AfterCombatEnd(IRunState runState, CombatState? combatState, CombatRoom room)
    {
        ThirstyPower.ClearAll();
    }
}
