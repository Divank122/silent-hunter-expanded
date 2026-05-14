using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using USCE.Scripts.Powers;

namespace USCE.Scripts.Cards;

[Pool(typeof(SilentCardPool))]
public class Bane : SilentCardModel, ILocalizationProvider
{
    private const int energyCost = 1;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Rare;
    private const TargetType targetType = TargetType.Self;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("IntangiblePower", 1m),
        new DynamicVar("PoisonPower", 9m),
        new DynamicVar("PoisonLoss", 2m)
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Retain, CardKeyword.Exhaust];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<IntangiblePower>(),
        HoverTipFactory.FromPower<PoisonPower>()
    ];

    public override List<(string, string)>? Localization => LocManager.Instance.Language switch
    {
        "zhs" => new CardLoc("厄咒", "获得{IntangiblePower:diff()}层[gold]无实体[/gold]和{PoisonPower:diff()}层[gold]中毒[/gold]。\n每当本回合你受到攻击时，失去{PoisonLoss:diff()}层[gold]中毒[/gold]。"),
        _ => new CardLoc("Bane", "Gain {IntangiblePower:diff()} [gold]Intangible[/gold] and {PoisonPower:diff()} [gold]Poison[/gold].\nWhenever you are attacked this turn, lose {PoisonLoss:diff()} [gold]Poison[/gold].")
    };

    public Bane() : base(energyCost, type, rarity, targetType)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int intangibleAmount = DynamicVars["IntangiblePower"].IntValue;
        int poisonAmount = DynamicVars["PoisonPower"].IntValue;
        int poisonLoss = DynamicVars["PoisonLoss"].IntValue;

        await PowerCmd.Apply<IntangiblePower>(choiceContext, Owner.Creature, intangibleAmount, Owner.Creature, this);
        await PowerCmd.Apply<PoisonPower>(choiceContext, Owner.Creature, poisonAmount, Owner.Creature, this);
        await PowerCmd.Apply<BanePower>(choiceContext, Owner.Creature, poisonLoss, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        base.EnergyCost.UpgradeBy(-1);
        DynamicVars["PoisonPower"].UpgradeValueBy(-2m);
    }
}
