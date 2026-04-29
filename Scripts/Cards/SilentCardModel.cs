using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Logging;

namespace USCE.Scripts.Cards;

public abstract class SilentCardModel : CustomCardModel
{
    private static readonly Dictionary<string, string> _portraitCache = new();

    public override string? CustomPortraitPath
    {
        get
        {
            string entryName = Id.Entry.ToLowerInvariant().Replace("-", "_");

            if (_portraitCache.TryGetValue(entryName, out string? cachedPath))
            {
                return cachedPath;
            }

            string path = $"res://UltimateSilentCardExpansion/images/cards/{entryName}.png";
            
            if (ResourceLoader.Exists(path))
            {
                _portraitCache[entryName] = path;
                return path;
            }

            return null;
        }
    }

    protected SilentCardModel(int energyCost, CardType type, CardRarity rarity, TargetType targetType, bool shouldShowInCardLibrary = true)
        : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }
}
