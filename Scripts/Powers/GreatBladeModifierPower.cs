using System.Linq;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;
using USCE.Scripts.Cards;

namespace USCE.Scripts.Powers;

public sealed class GreatBladeModifierPower : PowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    protected override bool IsVisibleInternal => false;

    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (!props.IsPoweredAttack())
        {
            return 1m;
        }
        if (cardSource is not GreatBlade)
        {
            return 1m;
        }
        if (cardSource.Owner.Creature != base.Owner)
        {
            return 1m;
        }

        var hand = PileType.Hand.GetPile(cardSource.Owner);
        int shivCount = hand.Cards.Count(c => c.Tags.Contains(CardTag.Shiv));

        var pile = cardSource.Pile;
        bool isInHand = pile != null && pile.Type == PileType.Hand;

        if (!isInHand)
        {
            shivCount++;
        }

        if (shivCount <= 1)
        {
            return 1m;
        }

        int halvingTimes = shivCount - 1;
        decimal multiplier = 1m;
        for (int i = 0; i < halvingTimes; i++)
        {
            multiplier /= 2m;
        }

        return multiplier;
    }
}
