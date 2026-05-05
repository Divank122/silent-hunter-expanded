using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace USCE.Scripts.Powers;

public class FreeSlyPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(CardKeyword.Sly)
    ];

    public override string? CustomPackedIconPath => "res://UltimateSilentCardExpansion/images/powers/usce_free_sly_power.png";
    public override string? CustomBigIconPath => "res://UltimateSilentCardExpansion/images/powers/usce_free_sly_power.png";

    public override List<(string, string)>? Localization => LocManager.Instance.Language switch
    {
        "zhs" => new PowerLoc("免费奇巧", "你的下一张[gold]奇巧[/gold]牌耗能为0{energyPrefix:energyIcons(1)}。", "你的下[blue]{Amount}[/blue]张[gold]奇巧[/gold]牌耗能为0{energyPrefix:energyIcons(1)}。"),
        _ => new PowerLoc("Free Sly", "Your next [gold]Sly[/gold] card costs 0 {energyPrefix:energyIcons(1)}.", "Your next [blue]{Amount}[/blue] [gold]Sly[/gold] cards cost 0 {energyPrefix:energyIcons(1)}.")
    };

    public override bool TryModifyEnergyCostInCombat(CardModel card, decimal originalCost, out decimal modifiedCost)
    {
        modifiedCost = originalCost;
        if (card.Owner.Creature != Owner)
        {
            return false;
        }
        if (!card.IsSlyThisTurn)
        {
            return false;
        }
        bool flag;
        switch (card.Pile?.Type)
        {
        case PileType.Hand:
        case PileType.Play:
            flag = true;
            break;
        default:
            flag = false;
            break;
        }
        if (!flag)
        {
            return false;
        }
        modifiedCost = default(decimal);
        return true;
    }

    public override async Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature == Owner && cardPlay.Card.IsSlyThisTurn)
        {
            bool flag;
            switch (cardPlay.Card.Pile?.Type)
            {
            case PileType.Hand:
            case PileType.Play:
                flag = true;
                break;
            default:
                flag = false;
                break;
            }
            if (flag)
            {
                await PowerCmd.Decrement(this);
            }
        }
    }
}
