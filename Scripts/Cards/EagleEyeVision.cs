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
public class EagleEyeVision : SilentCardModel, ILocalizationProvider
{
    private const int energyCost = 2;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<AccuracyPower>(),
        HoverTipFactory.FromPower<EagleEyeVisionPower>()
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new PowerVar<AccuracyPower>("AccuracyAmount", 7m)
    };

    public EagleEyeVision()
        : base(energyCost, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await PowerCmd.Apply<AccuracyPower>(Owner.Creature, DynamicVars["AccuracyAmount"].BaseValue, Owner.Creature, this);
        await PowerCmd.Apply<EagleEyeVisionPower>(Owner.Creature, 1m, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["AccuracyAmount"].UpgradeValueBy(2m);
    }

    public override List<(string, string)>? Localization => LocManager.Instance.Language switch
    {
        "zhs" => new CardLoc("鹰眼视觉", "获得{AccuracyAmount:diff()}层[gold]精准[/gold]。\n[gold]精准[/gold]可以增幅名字中有“打击”的牌。"),
        _ => new CardLoc("Eagle Eye Vision", "Gain {AccuracyAmount:diff()} [gold]Accuracy[/gold]. \n[gold]Accuracy[/gold] can affect cards with \"Strike\" in their name.")
    };
}
