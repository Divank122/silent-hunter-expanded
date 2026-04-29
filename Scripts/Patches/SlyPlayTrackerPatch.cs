using System.Collections.Generic;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;

namespace USCE.Scripts.Patches;

public static class SlyPlayTracker
{
    private static readonly HashSet<CardModel> _slyPlayedCards = new();

    public static bool IsSlyPlay(CardModel card)
    {
        return _slyPlayedCards.Contains(card);
    }

    public static void MarkAsSlyPlay(CardModel card)
    {
        _slyPlayedCards.Add(card);
    }

    public static void ClearSlyPlay(CardModel card)
    {
        _slyPlayedCards.Remove(card);
    }
}

[HarmonyPatch(typeof(Hook))]
public static class SlyPlayTrackerPatch
{
    [HarmonyPatch(nameof(Hook.BeforeCardAutoPlayed))]
    [HarmonyPrefix]
    public static void BeforeCardAutoPlayed(CombatState combatState, CardModel card, Creature? target, AutoPlayType type)
    {
        if (type == AutoPlayType.SlyDiscard)
        {
            SlyPlayTracker.MarkAsSlyPlay(card);
        }
    }

    [HarmonyPatch(nameof(Hook.BeforeCardPlayed))]
    [HarmonyPrefix]
    public static void BeforeCardPlayed(CombatState combatState, CardPlay cardPlay)
    {
        if (!cardPlay.IsAutoPlay)
        {
            SlyPlayTracker.ClearSlyPlay(cardPlay.Card);
        }
    }

    [HarmonyPatch(nameof(Hook.AfterCardPlayed))]
    [HarmonyPostfix]
    public static void AfterCardPlayed(CombatState combatState, PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        SlyPlayTracker.ClearSlyPlay(cardPlay.Card);
    }
}
