using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace USCE.Scripts.Powers;

public class ReadyAndWaitingPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<Shiv>(false)
    ];

    public override string? CustomPackedIconPath => "res://UltimateSilentCardExpansion/images/powers/usce_ready_and_waiting_power.png";
    public override string? CustomBigIconPath => "res://UltimateSilentCardExpansion/images/powers/usce_ready_and_waiting_power.png";

    public override List<(string, string)>? Localization => LocManager.Instance.Language switch
    {
        "zhs" => new PowerLoc("有备而来", "每当你触发奇巧时，添加1张小刀到你的手牌。", "每当你触发[gold]奇巧[/gold]时，添加{Amount}张[gold]小刀[/gold]到你的[gold]手牌[/gold]。"),
        _ => new PowerLoc("Ready And Waiting", "Whenever you trigger Sly, add 1 Shiv to your Hand.", "Whenever you trigger [gold]Sly[/gold], add {Amount} [gold]Shivs[/gold] to your [gold]Hand[/gold].")
    };

    public override async Task BeforeCardAutoPlayed(CardModel card, Creature? target, AutoPlayType type)
    {
        if (type == AutoPlayType.SlyDiscard && card.Owner.Creature == Owner)
        {
            Flash();
            for (int i = 0; i < Amount; i++)
            {
                await Shiv.CreateInHand(Owner.Player!, CombatState);
            }
        }
    }
}

public class ReadyAndWaitingPowerPlus : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<Shiv>(true)
    ];

    public override string? CustomPackedIconPath => "res://UltimateSilentCardExpansion/images/powers/usce_ready_and_waiting_power.png";
    public override string? CustomBigIconPath => "res://UltimateSilentCardExpansion/images/powers/usce_ready_and_waiting_power.png";

    public override List<(string, string)>? Localization => LocManager.Instance.Language switch
    {
        "zhs" => new PowerLoc("有备而来+", "每当你触发奇巧时，添加1张小刀+到你的手牌。", "每当你触发[gold]奇巧[/gold]时，添加{Amount}张[gold]小刀+[/gold]到你的[gold]手牌[/gold]。"),
        _ => new PowerLoc("Ready And Waiting+", "Whenever you trigger Sly, add 1 Shiv+ to your Hand.", "Whenever you trigger [gold]Sly[/gold], add {Amount} [gold]Shivs+[/gold] to your [gold]Hand[/gold].")
    };

    public override async Task BeforeCardAutoPlayed(CardModel card, Creature? target, AutoPlayType type)
    {
        if (type == AutoPlayType.SlyDiscard && card.Owner.Creature == Owner)
        {
            Flash();
            for (int i = 0; i < Amount; i++)
            {
                var shiv = await Shiv.CreateInHand(Owner.Player!, CombatState);
                if (shiv != null)
                {
                    CardCmd.Upgrade(shiv);
                }
            }
        }
    }
}
