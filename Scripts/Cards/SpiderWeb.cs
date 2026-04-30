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
        new DynamicVar("WeakAmount", 1m)
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    public override List<(string, string)>? Localization => LocManager.Instance.Language switch
    {
        "zhs" => new CardLoc("蛛网", "给予{WeakAmount:diff()}层[gold]虚弱[/gold]。\n你在这个回合内每次对该名敌人造成伤害时，额外给予{WeakAmount:diff()}层[gold]虚弱[/gold]。"),
        _ => new CardLoc("Spider Web", "Apply {WeakAmount:diff()} [gold]Weak[/gold].\nEach time you deal damage to that enemy this turn, apply {WeakAmount:diff()} additional [gold]Weak[/gold].")
    };

    public SpiderWeb() : base(energyCost, type, rarity, targetType)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Target == null) return;

        int weakAmount = DynamicVars["WeakAmount"].IntValue;
        await PowerCmd.Apply<WeakPower>(cardPlay.Target, weakAmount, Owner.Creature, this);
        
        var power = await PowerCmd.Apply<SpiderWebPower>(cardPlay.Target, weakAmount, Owner.Creature, this);
        if (power != null)
        {
            power.SourcePlayer = Owner.Creature;
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["WeakAmount"].UpgradeValueBy(1m);
    }
}
