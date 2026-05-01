using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using USCE.Scripts.Cards;

namespace USCE.Scripts.Powers;

public class BladeMountainPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.None;

    public override string? CustomPackedIconPath => "res://UltimateSilentCardExpansion/images/powers/usce_blade_mountain_power.png";
    public override string? CustomBigIconPath => "res://UltimateSilentCardExpansion/images/powers/usce_blade_mountain_power.png";

    public override List<(string, string)>? Localization => LocManager.Instance.Language switch
    {
        "zhs" => new PowerLoc("刀山", "所有获得小刀的效果改为获得1张巨刀。", "所有获得[gold]小刀[/gold]的效果改为获得1张[gold]巨刀[/gold]。"),
        _ => new PowerLoc("Blade Mountain", "All effects that create Shivs create 1 Great Blade instead.", "All effects that create [gold]Shivs[/gold] create 1 [gold]Great Blade[/gold] instead.")
    };

    public bool IsUpgraded { get; set; } = false;

    public async Task<IEnumerable<CardModel>> CreateGreatBladesInstead(Player owner, int count)
    {
        var combatState = owner.Creature.CombatState;
        if (combatState == null)
        {
            return System.Array.Empty<CardModel>();
        }
        
        if (IsUpgraded)
        {
            var blades = await GreatBlade.CreateInHand(owner, count, combatState);
            foreach (var blade in blades)
            {
                blade.UpgradeInternal();
                blade.FinalizeUpgradeInternal();
            }
            return blades;
        }
        return await GreatBlade.CreateInHand(owner, count, combatState);
    }
}
