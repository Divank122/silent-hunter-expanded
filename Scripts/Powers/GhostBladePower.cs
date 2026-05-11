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

public class GhostBladePower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override string? CustomPackedIconPath => "res://UltimateSilentCardExpansion/images/powers/usce_ghost_blade_power.png";
    public override string? CustomBigIconPath => "res://UltimateSilentCardExpansion/images/powers/usce_ghost_blade_power.png";

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<GhostBlade>()
    ];

    public override List<(string, string)>? Localization => LocManager.Instance.Language switch
    {
        "zhs" => new PowerLoc("幽灵之刃", "在下个回合，将1张幽灵之刃加入你的手牌。", "在下个回合，将[blue]{Amount}[/blue]张[gold]幽灵之刃[/gold]加入你的[gold]手牌[/gold]。"),
        _ => new PowerLoc("Ghost Blade", "Next turn, add 1 Ghost Blade to your hand.", "Next turn, add [blue]{Amount}[/blue] [gold]Ghost Blades[/gold] to your [gold]hand[/gold].")
    };

    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, CombatState combatState)
    {
        if (player == Owner?.Player && AmountOnTurnStart > 0)
        {
            Flash();
            await GhostBlade.CreateInHand(player, Amount, combatState);
            await PowerCmd.Remove(this);
        }
    }
}
