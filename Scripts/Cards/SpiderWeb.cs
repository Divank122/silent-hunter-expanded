using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using USCE.Scripts.Powers;

namespace USCE.Scripts.Cards;

[Pool(typeof(SilentCardPool))]
public class SpiderWeb : SilentCardModel, ILocalizationProvider
{
    private const int energyCost = 1;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Uncommon;
    private const TargetType targetType = TargetType.AnyEnemy;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("InitialWeak", 1m),
        new DynamicVar("FollowUpWeak", 1m)
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    public override List<(string, string)>? Localization => LocManager.Instance.Language switch
    {
        "zhs" => new CardLoc("蛛网", "给予{InitialWeak:diff()}层[gold]虚弱[/gold]。\n你在这个回合内该名敌人每次受到伤害时，额外给予{FollowUpWeak:diff()}层[gold]虚弱[/gold]。"),
        _ => new CardLoc("Spider Web", "Apply {InitialWeak:diff()} [gold]Weak[/gold].\nEach time that enemy takes damage this turn, apply {FollowUpWeak:diff()} additional [gold]Weak[/gold].")
    };

    public SpiderWeb() : base(energyCost, type, rarity, targetType)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Target == null) return;

        int initialWeak = DynamicVars["InitialWeak"].IntValue;
        int followUpWeak = DynamicVars["FollowUpWeak"].IntValue;
        
        await PowerCmd.Apply<WeakPower>(cardPlay.Target, initialWeak, Owner.Creature, this);
        
        var power = await PowerCmd.Apply<SpiderWebPower>(cardPlay.Target, followUpWeak, Owner.Creature, this);
        if (power != null)
        {
            power.SourcePlayer = Owner.Creature;
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["InitialWeak"].UpgradeValueBy(1m);
    }
}
