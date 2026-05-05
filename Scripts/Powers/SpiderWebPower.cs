using System.Threading.Tasks;
using BaseLib.Abstracts;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace USCE.Scripts.Powers;

public class SpiderWebPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override string? CustomPackedIconPath => "res://UltimateSilentCardExpansion/images/powers/usce_spider_web_power.png";
    public override string? CustomBigIconPath => "res://UltimateSilentCardExpansion/images/powers/usce_spider_web_power.png";

    public Creature? SourcePlayer { get; set; }

    public override List<(string, string)>? Localization => LocManager.Instance.Language switch
    {
        "zhs" => new PowerLoc("蛛网", "本回合内每次受到伤害时，获得1层虚弱。", "本回合内每次受到伤害时，获得[blue]{Amount}[/blue]层[gold]虚弱[/gold]。"),
        _ => new PowerLoc("Spider Web", "Each time takes damage this turn, gain 1 Weak.", "Each time takes damage this turn, gain [blue]{Amount}[/blue] [gold]Weak[/gold].")
    };

    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target == Owner && result.TotalDamage > 0)
        {
            await PowerCmd.Apply<WeakPower>(Owner, Amount, SourcePlayer, null);
            Flash();
        }
    }

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side == Owner.Side)
        {
            await PowerCmd.Remove(this);
        }
    }
}
