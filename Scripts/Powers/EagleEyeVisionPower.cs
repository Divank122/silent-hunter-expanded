using System.Collections.Generic;
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace USCE.Scripts.Powers;

public class EagleEyeVisionPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<AccuracyPower>()
    ];

    public override string? CustomPackedIconPath => "res://UltimateSilentCardExpansion/images/powers/usce_eagle_eye_vision_power.png";
    public override string? CustomBigIconPath => "res://UltimateSilentCardExpansion/images/powers/usce_eagle_eye_vision_power.png";

    public override List<(string, string)>? Localization => LocManager.Instance.Language switch
    {
        "zhs" => new PowerLoc("鹰眼视觉", "[gold]精准[/gold]可以增幅名字中有“打击”的牌。", "[gold]精准[/gold]可以增幅名字中有“打击”的牌。"),
        _ => new PowerLoc("Eagle Eye Vision", "[gold]Accuracy[/gold] can affect cards with \"Strike\" in their name.", "[gold]Accuracy[/gold] can affect cards with \"Strike\" in their name.")
    };
}
