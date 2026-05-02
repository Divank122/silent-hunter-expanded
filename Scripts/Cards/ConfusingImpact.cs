using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;

namespace USCE.Scripts.Cards;

[Pool(typeof(SilentCardPool))]
public class ConfusingImpact : SilentCardModel
{
    private const int energyCost = 2;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Rare;
    private const TargetType targetType = TargetType.AnyEnemy;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<PoisonPower>(7m)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<PoisonPower>()
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    public override List<(string, string)>? Localization => LocManager.Instance.Language switch
    {
        "zhs" => new CardLoc("迷惑冲击", "给予{PoisonPower:diff()}层[gold]中毒[/gold]。\n将敌人的意图变为造成12点伤害。"),
        _ => new CardLoc("Confusing Impact", "Apply {PoisonPower:diff()} [gold]Poison[/gold].\nChange the enemy's intent to deal 12 damage.")
    };

    public ConfusingImpact() : base(energyCost, type, rarity, targetType)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");

        await PowerCmd.Apply<PoisonPower>(cardPlay.Target, DynamicVars.Poison.IntValue, Owner.Creature, this);

        var monster = cardPlay.Target.Monster;
        if (monster != null && !cardPlay.Target.IsDead)
        {
            var stateLog = monster.MoveStateMachine.StateLog;
            string nextMoveId = string.Empty;
            if (stateLog.Count > 0)
            {
                nextMoveId = stateLog[^1]!.Id ?? string.Empty;
            }

            async Task ConfusingImpactMove(IReadOnlyList<Creature> targets)
            {
                await DamageCmd.Attack(12).FromMonster(monster).WithAttackerAnim("Attack", 0.35f)
                    .WithHitFx("vfx/vfx_attack_slash")
                    .Execute(null);
            }

            MoveState state = new MoveState("CONFUSING_IMPACT", ConfusingImpactMove, new SingleAttackIntent(12))
            {
                FollowUpStateId = nextMoveId,
                MustPerformOnceBeforeTransitioning = true
            };
            monster.SetMoveImmediate(state);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Poison.UpgradeValueBy(3m);
    }
}
