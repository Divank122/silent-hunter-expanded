using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Hooks;
using USCE.Scripts.Cards;
using USCE.Scripts.Powers;

namespace USCE.Scripts.Patches;

[HarmonyPatch(typeof(Shiv))]
public static class ShivCreateInHandPatches
{
    private static CardModel? _currentPlayingCard = null;
    private static readonly HashSet<Player> _playersWhoGotGreatBladeThisCard = new();

    public static void SetCurrentCard(CardModel? card)
    {
        _currentPlayingCard = card;
        _playersWhoGotGreatBladeThisCard.Clear();
    }

    [HarmonyPatch(nameof(Shiv.CreateInHand), typeof(Player), typeof(CombatState))]
    [HarmonyPrefix]
    public static bool PrefixSingle(Player owner, CombatState combatState, ref Task<CardModel?> __result)
    {
        if (owner == null || combatState == null)
        {
            return true;
        }

        var bladeMountainPower = owner.Creature.GetPower<BladeMountainPower>();
        if (bladeMountainPower == null)
        {
            return true;
        }

        if (_playersWhoGotGreatBladeThisCard.Contains(owner))
        {
            __result = Task.FromResult<CardModel?>(null);
            return false;
        }

        _playersWhoGotGreatBladeThisCard.Add(owner);
        __result = CreateOneGreatBlade(owner, bladeMountainPower);
        return false;
    }

    [HarmonyPatch(nameof(Shiv.CreateInHand), typeof(Player), typeof(int), typeof(CombatState))]
    [HarmonyPrefix]
    public static bool PrefixMultiple(Player owner, int count, CombatState combatState, ref Task<IEnumerable<CardModel>> __result)
    {
        if (owner == null || combatState == null)
        {
            return true;
        }

        var bladeMountainPower = owner.Creature.GetPower<BladeMountainPower>();
        if (bladeMountainPower == null)
        {
            return true;
        }

        if (_playersWhoGotGreatBladeThisCard.Contains(owner))
        {
            __result = Task.FromResult<IEnumerable<CardModel>>(System.Array.Empty<CardModel>());
            return false;
        }

        _playersWhoGotGreatBladeThisCard.Add(owner);
        
        if (_currentPlayingCard != null)
        {
            __result = CreateGreatBladesInstead(owner, bladeMountainPower, 1);
        }
        else
        {
            __result = CreateGreatBladesInstead(owner, bladeMountainPower, count);
        }
        return false;
    }

    private static async Task<CardModel?> CreateOneGreatBlade(Player owner, BladeMountainPower bladeMountainPower)
    {
        var blades = await bladeMountainPower.CreateGreatBladesInstead(owner, 1);
        foreach (var blade in blades)
        {
            return blade;
        }
        return null;
    }

    private static async Task<IEnumerable<CardModel>> CreateGreatBladesInstead(Player owner, BladeMountainPower bladeMountainPower, int count)
    {
        return await bladeMountainPower.CreateGreatBladesInstead(owner, count);
    }
}

[HarmonyPatch(typeof(Hook))]
public static class HookCardPlayPatch
{
    [HarmonyPatch(nameof(Hook.BeforeCardPlayed))]
    [HarmonyPrefix]
    public static void BeforeCardPlayed(CombatState combatState, CardPlay cardPlay)
    {
        ShivCreateInHandPatches.SetCurrentCard(cardPlay?.Card);
    }

    [HarmonyPatch(nameof(Hook.AfterCardPlayed))]
    [HarmonyPostfix]
    public static void AfterCardPlayed()
    {
        ShivCreateInHandPatches.SetCurrentCard(null);
    }
}
