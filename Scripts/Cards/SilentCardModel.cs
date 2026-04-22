using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Logging;

namespace USCE.Scripts.Cards;

public abstract class SilentCardModel : CustomCardModel
{
    private static readonly Dictionary<string, string> _portraitCache = new();
    private static bool _debugLogged = false;

    public override string? CustomPortraitPath
    {
        get
        {
            string entryName = Id.Entry.ToLowerInvariant().Replace("-", "_");
            string cacheKey = entryName;

            Log.Info($"[USCE] === CustomPortraitPath GET called ===");
            Log.Info($"[USCE] Id.Entry = '{Id.Entry}'");
            Log.Info($"[USCE] entryName = '{entryName}'");

            if (_portraitCache.TryGetValue(cacheKey, out string? cachedPath))
            {
                Log.Info($"[USCE] Using cached path: {cachedPath}");
                return cachedPath;
            }

            string[] possiblePaths = new[]
            {
                $"res://images/cards/{entryName}.png",
                $"res://images/cards/{entryName}.jpg",
                $"res://UltimateSilentCardExpansion/images/cards/{entryName}.png",
                $"res://UltimateSilentCardExpansion/images/cards/{entryName}.jpg",
            };

            foreach (string path in possiblePaths)
            {
                Log.Info($"[USCE] Trying to load: {path}");
                try
                {
                    var tex = ResourceLoader.Load<Texture2D>(path, null, ResourceLoader.CacheMode.Ignore);
                    if (tex != null)
                    {
                        Log.Info($"[USCE] SUCCESS! Loaded texture from: {path}");
                        _portraitCache[cacheKey] = path;
                        return path;
                    }
                }
                catch (System.Exception e)
                {
                    Log.Info($"[USCE] Failed to load {path}: {e.Message}");
                }
            }

            Log.Warn($"[USCE] === ALL PATHS FAILED for {entryName} ===");

            if (!_debugLogged)
            {
                _debugLogged = true;
                Log.Warn($"[USCE] Listing res://images/cards/ directory:");
                try
                {
                    var files = DirAccess.GetFilesAt("res://images/cards/");
                    foreach (var f in files)
                    {
                        Log.Warn($"  [cards] {f}");
                    }
                }
                catch (System.Exception e)
                {
                    Log.Warn($"[USCE] Error listing files: {e.Message}");
                }
            }

            return null;
        }
    }

    protected SilentCardModel(int energyCost, CardType type, CardRarity rarity, TargetType targetType, bool shouldShowInCardLibrary = true)
        : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }
}
