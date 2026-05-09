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

public class MirrorImagePower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<MegaCrit.Sts2.Core.Models.Cards.Shiv>(false)
    ];

    public override string? CustomPackedIconPath => "res://UltimateSilentCardExpansion/images/powers/usce_mirror_image_power.png";
    public override string? CustomBigIconPath => "res://UltimateSilentCardExpansion/images/powers/usce_mirror_image_power.png";

    public override List<(string, string)>? Localization => LocManager.Instance.Language switch
    {
        "zhs" => new PowerLoc("镜中倒影", "每当你打出一张不是[gold]小刀[/gold]的攻击牌时，将一张[gold]小刀[/gold]添加到你的[gold]手牌[/gold]。", "每当你打出一张不是[gold]小刀[/gold]的攻击牌时，将[blue]{Amount}[/blue]张[gold]小刀[/gold]添加到你的[gold]手牌[/gold]。"),
        _ => new PowerLoc("Mirror Image", "Whenever you play a non-[gold]Shiv[/gold] Attack, add a [gold]Shiv[/gold] to your [gold]hand[/gold].", "Whenever you play a non-[gold]Shiv[/gold] Attack, add [blue]{Amount}[/blue] [gold]Shiv(s)[/gold] to your [gold]hand[/gold].")
    };

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature != Owner)
            return;

        if (cardPlay.Card.Type != CardType.Attack)
            return;

        if (cardPlay.Card is MegaCrit.Sts2.Core.Models.Cards.Shiv)
            return;

        Flash();

        int shivsToAdd = (int)Amount;
        await MegaCrit.Sts2.Core.Models.Cards.Shiv.CreateInHand(Owner.Player!, shivsToAdd, CombatState);
    }
}

public class MirrorImagePowerPlus : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<MegaCrit.Sts2.Core.Models.Cards.Shiv>(true)
    ];

    public override string? CustomPackedIconPath => "res://UltimateSilentCardExpansion/images/powers/usce_mirror_image_power.png";
    public override string? CustomBigIconPath => "res://UltimateSilentCardExpansion/images/powers/usce_mirror_image_power.png";

    public override List<(string, string)>? Localization => LocManager.Instance.Language switch
    {
        "zhs" => new PowerLoc("镜中倒影+", "每当你打出一张不是[gold]小刀[/gold]的攻击牌时，将一张[gold]小刀+[/gold]添加到你的[gold]手牌[/gold]。", "每当你打出一张不是[gold]小刀[/gold]的攻击牌时，将[blue]{Amount}[/blue]张[gold]小刀+[/gold]添加到你的[gold]手牌[/gold]。"),
        _ => new PowerLoc("Mirror Image+", "Whenever you play a non-[gold]Shiv[/gold] Attack, add a [gold]Shiv+[/gold] to your [gold]hand[/gold].", "Whenever you play a non-[gold]Shiv[/gold] Attack, add [blue]{Amount}[/blue] [gold]Shiv(s)+[/gold] to your [gold]hand[/gold].")
    };

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature != Owner)
            return;

        if (cardPlay.Card.Type != CardType.Attack)
            return;

        if (cardPlay.Card is MegaCrit.Sts2.Core.Models.Cards.Shiv)
            return;

        Flash();

        int shivsToAdd = (int)Amount;
        var shivs = await MegaCrit.Sts2.Core.Models.Cards.Shiv.CreateInHand(Owner.Player!, shivsToAdd, CombatState);
        foreach (var shiv in shivs)
        {
            CardCmd.Upgrade(shiv);
        }
    }
}
