using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;

namespace USCE.Scripts.Powers;

public class FlayPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<PoisonPower>(),
        HoverTipFactory.FromPower<WeakPower>()
    ];

    public override string? CustomPackedIconPath => "res://UltimateSilentCardExpansion/images/powers/usce_flay_power.png";
    public override string? CustomBigIconPath => "res://UltimateSilentCardExpansion/images/powers/usce_flay_power.png";

    public override List<(string, string)>? Localization => LocManager.Instance.Language switch
    {
        "zhs" => new PowerLoc("剥皮", "每当你给予敌人中毒，额外给予1层虚弱。", "每当你给予敌人[gold]中毒[/gold]，额外给予[blue]{Amount}[/blue]层[gold]虚弱[/gold]。"),
        _ => new PowerLoc("Flay", "Whenever you apply Poison to an enemy, apply 1 Weak.", "Whenever you apply [gold]Poison[/gold] to an enemy, apply [blue]{Amount}[/blue] [gold]Weak[/gold].")
    };

    public override async Task AfterPowerAmountChanged(PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (applier == Owner && amount > 0m && power is PoisonPower && power.Owner.Side != Owner.Side)
        {
            Flash();
            await PowerCmd.Apply<WeakPower>(power.Owner, Amount, Owner, null);
        }
    }
}
