using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace USCE.Scripts.Powers;

public class SpiderWebPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override string? CustomPackedIconPath => "res://UltimateSilentCardExpansion/images/powers/usce_spider_web_power.png";
    public override string? CustomBigIconPath => "res://UltimateSilentCardExpansion/images/powers/usce_spider_web_power.png";

    public Creature? TargetEnemy { get; set; }

    public override List<(string, string)>? Localization => LocManager.Instance.Language switch
    {
        "zhs" => new PowerLoc("蛛网", "你在这个回合内每次对该名敌人造成伤害时，额外给予1层虚弱。", "你在这个回合内每次对该名敌人造成伤害时，额外给予[blue]{Amount}[/blue]层[gold]虚弱[/gold]。"),
        _ => new PowerLoc("Spider Web", "Each time you deal damage to that enemy this turn, apply 1 Weak.", "Each time you deal damage to that enemy this turn, apply [blue]{Amount}[/blue] [gold]Weak[/gold].")
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

        if (TargetEnemy == null || TargetEnemy.IsDead)
        {
            return;
        }

        foreach (DamageResult result in command.Results)
        {
            if (result.Receiver == TargetEnemy && result.TotalDamage > 0)
            {
                await PowerCmd.Apply<WeakPower>(result.Receiver, Amount, Owner, null);
            }
        }

        Flash();
    }

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side == Owner.Side)
        {
            await PowerCmd.Remove(this);
        }
    }
}
