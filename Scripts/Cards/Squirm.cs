using System.Collections.Generic;
using System.Linq;
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
using MegaCrit.Sts2.Core.ValueProps;

namespace USCE.Scripts.Cards;

[Pool(typeof(SilentCardPool))]
public class Squirm : SilentCardModel
{
    private const int energyCost = 1;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Uncommon;
    private const TargetType targetType = TargetType.AnyEnemy;

    public override bool GainsBlock => true;

    protected override bool ShouldGlowGoldInternal
    {
        get
        {
            if (CombatState == null)
            {
                return false;
            }
            return CombatState.HittableEnemies.Any(e => e.GetPower<PoisonPower>()?.Amount > 0);
        }
    }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(8m, ValueProp.Move),
        new DynamicVar("PoisonLoss", 3m),
        new BlockVar("BonusBlock", 6m, ValueProp.Move)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<PoisonPower>()
    ];

    public override List<(string, string)>? Localization => LocManager.Instance.Language switch
    {
        "zhs" => new CardLoc("蠕动", "获得{Block:diff()}点[gold]格挡[/gold]。\n如果敌方拥有[gold]中毒[/gold]，使其失去{PoisonLoss:diff()}层[gold]中毒[/gold]，额外获得{BonusBlock:diff()}点[gold]格挡[/gold]。"),
        _ => new CardLoc("Squirm", "Gain {Block:diff()} [gold]Block[/gold].\nIf the enemy has [gold]Poison[/gold], they lose {PoisonLoss:diff()} [gold]Poison[/gold] and you gain {BonusBlock:diff()} [gold]Block[/gold].")
    };

    public Squirm() : base(energyCost, type, rarity, targetType)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);

        if (cardPlay.Target != null)
        {
            var poisonPower = cardPlay.Target.GetPower<PoisonPower>();
            if (poisonPower != null && poisonPower.Amount > 0)
            {
                int poisonLoss = DynamicVars["PoisonLoss"].IntValue;

                int newAmount = poisonPower.Amount - poisonLoss;
                if (newAmount <= 0)
                {
                    await PowerCmd.Remove(poisonPower);
                }
                else
                {
                    poisonPower.SetAmount(newAmount);
                }

                var bonusBlockVar = (BlockVar)DynamicVars["BonusBlock"];
                await CreatureCmd.GainBlock(Owner.Creature, bonusBlockVar, cardPlay);
            }
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3m);
        DynamicVars["BonusBlock"].UpgradeValueBy(2m);
    }
}
