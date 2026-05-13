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

namespace USCE.Scripts.Cards;

[Pool(typeof(SilentCardPool))]
public class ElasticFiber : SilentCardModel, ILocalizationProvider
{
    private const int energyCost = 1;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromPower<PlatingPower>()
    };

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new PowerVar<PlatingPower>("PlayerPlating", 6m),
        new PowerVar<PlatingPower>("EnemyPlating", 6m)
    };

    public ElasticFiber()
        : base(energyCost, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<PlatingPower>(Owner.Creature, DynamicVars["PlayerPlating"].IntValue, Owner.Creature, this);

        var enemies = CombatState!.HittableEnemies;
        if (enemies.Count > 0)
        {
            Creature? randomEnemy = Owner.RunState.Rng.CombatTargets.NextItem(enemies);
            if (randomEnemy != null)
            {
                await PowerCmd.Apply<PlatingPower>(randomEnemy, DynamicVars["EnemyPlating"].IntValue, Owner.Creature, this);
            }
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["PlayerPlating"].UpgradeValueBy(2m);
    }

    public override List<(string, string)>? Localization => LocManager.Instance.Language switch
    {
        "zhs" => new CardLoc("弹性纤维", "获得{PlayerPlating:diff()}层[gold]覆甲[/gold]。\n随机敌人获得{EnemyPlating:diff()}层[gold]覆甲[/gold]。"),
        _ => new CardLoc("Elastic Fiber", "Gain {PlayerPlating:diff()} [gold]Plating[/gold].\nA random enemy gains {EnemyPlating:diff()} [gold]Plating[/gold].")
    };
}
