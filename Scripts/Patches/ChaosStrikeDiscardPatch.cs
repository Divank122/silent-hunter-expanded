using System.Linq;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models.Cards;
using USCE.Scripts.Cards;

namespace USCE.Scripts.Patches;

[HarmonyPatch(typeof(Hook))]
public static class ChaosStrikeDiscardPatch
{
    [HarmonyPatch(nameof(Hook.BeforeCardPlayed))]
    [HarmonyPrefix]
    public static async void BeforeCardPlayed(CombatState combatState, CardPlay cardPlay)
    {
        var card = cardPlay.Card;
        if (card == null)
        {
            return;
        }

        if (card is ChaosStrike)
        {
            return;
        }

        var player = card.Owner;
        if (player == null)
        {
            return;
        }

        var hand = PileType.Hand.GetPile(player);
        if (hand == null)
        {
            return;
        }

        var chaosStrikes = hand.Cards.OfType<ChaosStrike>().ToList();
        if (chaosStrikes.Count == 0)
        {
            return;
        }

        foreach (var chaosStrike in chaosStrikes)
        {
            await CardPileCmd.Add(chaosStrike, PileType.Discard);
        }
    }
}
