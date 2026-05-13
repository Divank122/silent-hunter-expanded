using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using USCE.Scripts.Cards;

namespace USCE.Scripts.Powers;

public class BladeMountainPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(USCEKeywords.GreatBlade),
        HoverTipFactory.FromCard<Shiv>(),
        HoverTipFactory.FromCard<GreatBlade>()
    ];

    public override string? CustomPackedIconPath => "res://UltimateSilentCardExpansion/images/powers/usce_blade_mountain_power.png";
    public override string? CustomBigIconPath => "res://UltimateSilentCardExpansion/images/powers/usce_blade_mountain_power.png";

    public override List<(string, string)>? Localization => LocManager.Instance.Language switch
    {
        "zhs" => new PowerLoc("刀山", "每当你获得小刀时，改为获得巨刀。", "每当你获得[gold]小刀[/gold]时，改为获得[gold]巨刀[/gold]。"),
        _ => new PowerLoc("Blade Mountain", "Whenever you gain a Shiv, gain a Great Blade instead.", "Whenever you gain a [gold]Shiv[/gold], gain a [gold]Great Blade[/gold] instead.")
    };

    public async Task<IEnumerable<CardModel>> CreateGreatBladesInstead(Player owner, int count)
    {
        var combatState = owner.Creature.CombatState;
        if (combatState == null)
        {
            return System.Array.Empty<CardModel>();
        }
        return await GreatBlade.CreateInHand(owner, count, combatState);
    }
}

public class BladeMountainPowerPlus : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(USCEKeywords.GreatBlade),
        HoverTipFactory.FromCard<Shiv>(),
        HoverTipFactory.FromCard<GreatBlade>(true)
    ];

    public override string? CustomPackedIconPath => "res://UltimateSilentCardExpansion/images/powers/usce_blade_mountain_power.png";
    public override string? CustomBigIconPath => "res://UltimateSilentCardExpansion/images/powers/usce_blade_mountain_power.png";

    public override List<(string, string)>? Localization => LocManager.Instance.Language switch
    {
        "zhs" => new PowerLoc("刀山+", "每当你获得小刀时，改为获得巨刀+。", "每当你获得[gold]小刀[/gold]时，改为获得[gold]巨刀+[/gold]。"),
        _ => new PowerLoc("Blade Mountain+", "Whenever you gain a Shiv, gain a Great Blade+ instead.", "Whenever you gain a [gold]Shiv[/gold], gain a [gold]Great Blade+[/gold] instead.")
    };

    public async Task<IEnumerable<CardModel>> CreateGreatBladesInstead(Player owner, int count)
    {
        var combatState = owner.Creature.CombatState;
        if (combatState == null)
        {
            return System.Array.Empty<CardModel>();
        }
        var blades = await GreatBlade.CreateInHand(owner, count, combatState);
        foreach (var blade in blades)
        {
            blade.UpgradeInternal();
            blade.FinalizeUpgradeInternal();
        }
        return blades;
    }
}
