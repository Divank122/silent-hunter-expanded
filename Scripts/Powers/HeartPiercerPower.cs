using System.Linq;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace USCE.Scripts.Powers;

public class HeartPiercerPower : CustomPowerModel, ILocalizationProvider
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override string? CustomPackedIconPath => "res://UltimateSilentCardExpansion/images/powers/usce_heart_piercer_power.png";
    public override string? CustomBigIconPath => "res://UltimateSilentCardExpansion/images/powers/usce_heart_piercer_power.png";

    public override List<(string, string)>? Localization => LocManager.Instance.Language switch
    {
        "zhs" => new PowerLoc("钻心", "小刀额外给予1层中毒。", "小刀额外给予[blue]{Amount}[/blue]层[gold]中毒[/gold]。"),
        _ => new PowerLoc("Heart Piercer", "Shivs deal additional 1 Poison.", "Shivs deal additional [blue]{Amount}[/blue] [gold]Poison[/gold].")
    };

    public override async Task AfterAttack(AttackCommand command)
    {
        if (command.Attacker != Owner || command.TargetSide == Owner.Side)
        {
            return;
        }

        if (!command.DamageProps.IsPoweredAttack())
        {
            return;
        }

        if (command.ModelSource is not CardModel card || !card.Tags.Contains(CardTag.Shiv))
        {
            return;
        }

        foreach (DamageResult result in command.Results)
        {
            if (result.UnblockedDamage > 0 && !result.Receiver.IsDead)
            {
                await PowerCmd.Apply<PoisonPower>(result.Receiver, Amount, Owner, null);
            }
        }

        Flash();
    }
}
