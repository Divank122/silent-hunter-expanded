using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using USCE.Scripts.Cards;

namespace USCE.Scripts.Patches;

[HarmonyPatch(typeof(DustyTome))]
public static class DustyTomePatch
{
    [HarmonyPatch("SetupForPlayer")]
    [HarmonyPrefix]
    public static bool SetupForPlayerPrefix(DustyTome __instance, Player player)
    {
        var prospectorId = ModelDb.Card<Prospector>().Id;

        IEnumerable<CardModel> items = from c in player.Character.CardPool.GetUnlockedCards(player.UnlockState, player.RunState.CardMultiplayerConstraint)
            where c.Rarity == CardRarity.Ancient && !ArchaicTooth.TranscendenceCards.Contains(c) && c.Id != prospectorId
            select c;

        var selectedCard = player.PlayerRng.Rewards.NextItem(items);
        if (selectedCard != null)
        {
            __instance.AncientCard = selectedCard.Id;
        }

        return false;
    }
}
