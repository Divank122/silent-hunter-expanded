using System.Collections.Generic;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace USCE.Scripts.Patches;

public static class SluggishPatch
{
    public static bool IsSluggish(CardModel card)
    {
        return card.Keywords.Contains(USCEKeywords.Sluggish);
    }
}

[HarmonyPatch(typeof(CardCmd), nameof(CardCmd.DiscardAndDraw))]
public static class CardCmdDiscardAndDrawPatch
{
    static void Prefix(ref IEnumerable<CardModel> cardsToDiscard)
    {
        var list = new List<CardModel>();
        foreach (var card in cardsToDiscard)
        {
            if (!SluggishPatch.IsSluggish(card))
            {
                list.Add(card);
            }
        }
        cardsToDiscard = list;
    }
}
