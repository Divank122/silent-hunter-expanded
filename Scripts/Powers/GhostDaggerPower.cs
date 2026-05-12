using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using USCE.Scripts.Cards;

namespace USCE.Scripts.Powers;

public class GhostDaggerPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override string? CustomPackedIconPath => "res://UltimateSilentCardExpansion/images/powers/usce_ghost_dagger_power.png";
    public override string? CustomBigIconPath => "res://UltimateSilentCardExpansion/images/powers/usce_ghost_dagger_power.png";

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<GhostDagger>()
    ];

    public override List<(string, string)>? Localization => LocManager.Instance.Language switch
    {
        "zhs" => new PowerLoc("幽灵匕首", "在下个回合，将1张幽灵匕首加入你的手牌。", "在下个回合，将[blue]{Amount}[/blue]张[gold]幽灵匕首[/gold]加入你的[gold]手牌[/gold]。"),
        _ => new PowerLoc("Ghost Dagger", "Next turn, add 1 Ghost Dagger to your hand.", "Next turn, add [blue]{Amount}[/blue] [gold]Ghost Daggers[/gold] to your [gold]hand[/gold].")
    };

    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, CombatState combatState)
    {
        if (player == Owner?.Player && AmountOnTurnStart > 0)
        {
            Flash();
            await GhostDagger.CreateInHand(player, Amount, combatState);
            await PowerCmd.Remove(this);
        }
    }
}

public class GhostDaggerPowerPlus : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override string? CustomPackedIconPath => "res://UltimateSilentCardExpansion/images/powers/usce_ghost_dagger_power.png";
    public override string? CustomBigIconPath => "res://UltimateSilentCardExpansion/images/powers/usce_ghost_dagger_power.png";

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<GhostDagger>(true)
    ];

    public override List<(string, string)>? Localization => LocManager.Instance.Language switch
    {
        "zhs" => new PowerLoc("幽灵匕首+", "在下个回合，将1张幽灵匕首+加入你的手牌。", "在下个回合，将[blue]{Amount}[/blue]张[gold]幽灵匕首+[/gold]加入你的[gold]手牌[/gold]。"),
        _ => new PowerLoc("Ghost Dagger+", "Next turn, add 1 Ghost Dagger+ to your hand.", "Next turn, add [blue]{Amount}[/blue] [gold]Ghost Daggers+[/gold] to your [gold]hand[/gold].")
    };

    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, CombatState combatState)
    {
        if (player == Owner?.Player && AmountOnTurnStart > 0)
        {
            Flash();
            var daggers = await GhostDagger.CreateInHand(player, Amount, combatState);
            foreach (var dagger in daggers)
            {
                CardCmd.Upgrade(dagger);
            }
            await PowerCmd.Remove(this);
        }
    }
}