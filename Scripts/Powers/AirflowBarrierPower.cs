using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace USCE.Scripts.Powers;

public class AirflowBarrierPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.Static(StaticHoverTip.Block)
    ];

    public override string? CustomPackedIconPath => "res://UltimateSilentCardExpansion/images/powers/usce_airflow_barrier_power.png";
    public override string? CustomBigIconPath => "res://UltimateSilentCardExpansion/images/powers/usce_airflow_barrier_power.png";

    private bool HasDiscardedThisTurn
    {
        get => GetInternalData<TurnData>().HasDiscarded;
        set => GetInternalData<TurnData>().HasDiscarded = value;
    }

    protected override object? InitInternalData()
    {
        return new TurnData();
    }

    public override List<(string, string)>? Localization => LocManager.Instance.Language switch
    {
        "zhs" => new PowerLoc("气流屏障", "每回合第一次弃牌时，获得6点格挡。", "每回合第一次弃牌时，获得[blue]{Amount}[/blue]点格挡。"),
        _ => new PowerLoc("Airflow Barrier", "Gain 6 Block the first time you discard each turn.", "Gain [blue]{Amount}[/blue] Block the first time you discard each turn.")
    };

    public override async Task AfterEnergyReset(Player player)
    {
        if (player.Creature == Owner)
        {
            HasDiscardedThisTurn = false;
        }
    }

    public override async Task AfterCardDiscarded(PlayerChoiceContext choiceContext, CardModel card)
    {
        if (card.Owner.Creature == Owner && !HasDiscardedThisTurn)
        {
            HasDiscardedThisTurn = true;
            Flash();
            await CreatureCmd.GainBlock(Owner, Amount, ValueProp.Unpowered, null, fast: true);
        }
    }

    private class TurnData
    {
        public bool HasDiscarded;
    }
}
