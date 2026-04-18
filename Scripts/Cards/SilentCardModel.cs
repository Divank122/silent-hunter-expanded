using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace SilentHunterExpanded.Scripts.Cards;

public abstract class SilentCardModel : CustomCardModel
{
    public override string PortraitPath => $"res://silent-hunter-expanded/images/cards/{Id.Entry.ToLowerInvariant()}.png";

    protected SilentCardModel(int energyCost, CardType type, CardRarity rarity, TargetType targetType, bool shouldShowInCardLibrary = true)
        : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }
}
