using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace USCE.Scripts.Powers;

public class ThirstyPower : CustomPowerModel
{
    private static readonly HashSet<CardModel> _thirstyCards = new();

    public static bool IsThirsty(CardModel card) => _thirstyCards.Contains(card);
    
    public static void SetThirsty(CardModel card, bool thirsty)
    {
        if (thirsty)
            _thirstyCards.Add(card);
        else
            _thirstyCards.Remove(card);
    }

    public static void ClearAll() => _thirstyCards.Clear();

    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;
    
    protected override bool IsVisibleInternal => false;

    public override List<(string, string)>? Localization => LocManager.Instance.Language switch
    {
        "zhs" => new PowerLoc("饥渴", "", ""),
        _ => new PowerLoc("Thirsty", "", "")
    };

    public override bool ShouldPlay(CardModel card, AutoPlayType autoPlayType)
    {
        if (card.Owner.Creature != Owner)
            return true;

        if (!card.Keywords.Contains(USCEKeywords.Thirsty))
            return true;

        return !IsThirsty(card);
    }

    public override Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card != null && cardPlay.Card.Keywords.Contains(USCEKeywords.Thirsty))
        {
            SetThirsty(cardPlay.Card, true);
        }
        return Task.CompletedTask;
    }

    public override Task AfterDeath(PlayerChoiceContext choiceContext, Creature creature, bool wasRemovalPrevented, float deathAnimLength)
    {
        if (wasRemovalPrevented)
            return Task.CompletedTask;

        if (creature.Side == CombatSide.Enemy)
        {
            bool isMinion = creature.Powers.Any(p => p is MinionPower);
            if (!isMinion)
            {
                ClearAll();
            }
        }
        return Task.CompletedTask;
    }
}
