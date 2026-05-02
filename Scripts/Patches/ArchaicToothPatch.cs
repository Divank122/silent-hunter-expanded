using System.Linq;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Relics;
using USCE.Scripts.Cards;

namespace USCE.Scripts.Patches;

[HarmonyPatch(typeof(ArchaicTooth))]
public static class ArchaicToothPatch
{
    [HarmonyPatch("GetTranscendenceStarterCard")]
    [HarmonyPrefix]
    public static bool GetTranscendenceStarterCardPrefix(ref CardModel? __result, Player player)
    {
        var neutralizeId = ModelDb.Card<Neutralize>().Id;
        var survivorId = ModelDb.Card<Survivor>().Id;

        bool hasNeutralize = player.Deck.Cards.Any(c => c.Id == neutralizeId);
        bool hasSurvivor = player.Deck.Cards.Any(c => c.Id == survivorId);

        if (hasNeutralize && hasSurvivor)
        {
            __result = GD.Randf() < 0.5f
                ? player.Deck.Cards.First(c => c.Id == neutralizeId)
                : player.Deck.Cards.First(c => c.Id == survivorId);
        }
        else if (hasNeutralize)
        {
            __result = player.Deck.Cards.First(c => c.Id == neutralizeId);
        }
        else if (hasSurvivor)
        {
            __result = player.Deck.Cards.First(c => c.Id == survivorId);
        }
        else
        {
            __result = null;
        }

        return false;
    }

    [HarmonyPatch("GetTranscendenceTransformedCard")]
    [HarmonyPrefix]
    public static bool GetTranscendenceTransformedCardPrefix(ArchaicTooth __instance, ref CardModel __result, CardModel starterCard)
    {
        var survivorId = ModelDb.Card<Survivor>().Id;

        if (starterCard.Id == survivorId)
        {
            var prospectorModel = ModelDb.Card<Prospector>();
            CardModel cardModel = starterCard.Owner.RunState.CreateCard(prospectorModel, starterCard.Owner);
            if (starterCard.IsUpgraded)
            {
                CardCmd.Upgrade(cardModel);
            }
            if (starterCard.Enchantment != null)
            {
                var enchantmentModel = (EnchantmentModel)starterCard.Enchantment.MutableClone();
                CardCmd.Enchant(enchantmentModel, cardModel, enchantmentModel.Amount);
            }
            __result = cardModel;
            return false;
        }

        return true;
    }
}
