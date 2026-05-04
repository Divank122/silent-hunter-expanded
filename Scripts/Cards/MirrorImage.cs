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
using USCE.Scripts.Powers;

namespace USCE.Scripts.Cards;

[Pool(typeof(SilentCardPool))]
public class MirrorImage : SilentCardModel, ILocalizationProvider
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<MirrorImagePower>(1m)
    ];

    public MirrorImage()
        : base(3, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await PowerCmd.Apply<MirrorImagePower>(Owner.Creature, DynamicVars["MirrorImagePower"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }

    public override List<(string, string)>? Localization => LocManager.Instance.Language switch
    {
        "zhs" => new CardLoc("镜中倒影", "每当你打出攻击牌时，随机打出你[gold]手牌[/gold]中的另外{MirrorImagePower:diff()}张攻击牌。"),
        _ => new CardLoc("Mirror Image", "Whenever you play an Attack, play {MirrorImagePower:diff()} other random Attack(s) from your [gold]hand[/gold].")
    };
}
