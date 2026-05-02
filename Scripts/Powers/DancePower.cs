using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace USCE.Scripts.Powers;

public class DancePower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    public override string? CustomPackedIconPath => "res://UltimateSilentCardExpansion/images/powers/usce_dance_power.png";
    public override string? CustomBigIconPath => "res://UltimateSilentCardExpansion/images/powers/usce_dance_power.png";

    public override List<(string, string)>? Localization => LocManager.Instance.Language switch
    {
        "zhs" => new PowerLoc("起舞", "每回合第一次弃牌时，获得{energyPrefix:energyIcons(1)}。", "每回合第一次弃牌时，获得{energyPrefix:energyIcons(1)}。"),
        _ => new PowerLoc("Dance", "Gain {energyPrefix:energyIcons(1)} the first time you discard each turn.", "Gain {energyPrefix:energyIcons(1)} the first time you discard each turn.")
    };

    public override async Task AfterCardDiscarded(PlayerChoiceContext choiceContext, CardModel card)
    {
        if (card.Owner.Creature != Owner)
            return;

        int discardCount = CombatManager.Instance.History.Entries
            .OfType<CardDiscardedEntry>()
            .Count(e => e.HappenedThisTurn(CombatState) && e.Actor == Owner);

        if (discardCount == 1)
        {
            Flash();
            await PlayerCmd.GainEnergy(1, Owner.Player!);
        }
    }
}
