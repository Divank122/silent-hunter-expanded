using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Rooms;
using USCE.Scripts.Cards;
using USCE.Scripts.Powers;

namespace USCE.Scripts.Patches;

/// <summary>
/// 处理"刀山"能力的Patch：将所有创建小刀的效果替换为创建巨刀
/// 
/// 核心逻辑：
/// 1. 拦截Shiv.CreateInHand的调用
/// 2. 检查玩家是否有刀山能力
/// 3. 如果有，创建巨刀代替小刀
/// 4. 处理升级版本的刀山能力（创建巨刀+）
/// 
/// 特殊处理：
/// - Power来源：每次都创建巨刀
/// - Card来源：每次打出只创建一次（防止多次触发）
/// - ReadyAndWaitingPower+：创建的巨刀自动升级
/// </summary>
[HarmonyPatch(typeof(Shiv))]
public static class ShivCreateInHandPatches
{
    private static readonly HashSet<Player> _playersWhoGotGreatBladeFromCardPlay = new();
    private static bool _shouldSkipUpgrade = false;

    public static bool ShouldSkipUpgrade => _shouldSkipUpgrade;

    public static void StartCardPlay()
    {
        _playersWhoGotGreatBladeFromCardPlay.Clear();
    }

    public static void EndCardPlay()
    {
        _playersWhoGotGreatBladeFromCardPlay.Clear();
    }

    public static void ClearAll()
    {
        _playersWhoGotGreatBladeFromCardPlay.Clear();
        _shouldSkipUpgrade = false;
    }

    private enum SourceType
    {
        None,
        Card,
        Power
    }

    private static SourceType FindNearestSource()
    {
        var stackTrace = new StackTrace();
        for (int i = 0; i < stackTrace.FrameCount; i++)
        {
            var method = stackTrace.GetFrame(i)?.GetMethod();
            if (method != null)
            {
                var declaringType = method.DeclaringType;
                if (declaringType != null)
                {
                    if (typeof(CardModel).IsAssignableFrom(declaringType))
                    {
                        return SourceType.Card;
                    }
                    if (typeof(PowerModel).IsAssignableFrom(declaringType))
                    {
                        return SourceType.Power;
                    }
                }
            }
        }
        return SourceType.None;
    }

    private static bool IsFromUpgradedSource()
    {
        var stackTrace = new StackTrace();
        for (int i = 0; i < stackTrace.FrameCount; i++)
        {
            var method = stackTrace.GetFrame(i)?.GetMethod();
            if (method != null)
            {
                var declaringType = method.DeclaringType;
                if (declaringType == typeof(ReadyAndWaitingPowerPlus))
                {
                    return true;
                }
            }
        }
        return false;
    }

    private static bool HasBladeMountainPlus(Creature creature)
    {
        return creature.GetPower<BladeMountainPowerPlus>() != null;
    }

    private static (int normalAmount, int plusAmount) GetBladeMountainPowers(Creature creature)
    {
        int normalAmount = 0;
        int plusAmount = 0;

        var power = creature.GetPower<BladeMountainPower>();
        if (power != null)
        {
            normalAmount = power.Amount;
        }

        var powerPlus = creature.GetPower<BladeMountainPowerPlus>();
        if (powerPlus != null)
        {
            plusAmount = powerPlus.Amount;
        }

        return (normalAmount, plusAmount);
    }

    [HarmonyPatch(nameof(Shiv.CreateInHand), typeof(Player), typeof(CombatState))]
    [HarmonyPrefix]
    public static bool PrefixSingle(Player owner, CombatState combatState, ref Task<CardModel?> __result)
    {
        if (owner == null || combatState == null)
        {
            _shouldSkipUpgrade = false;
            return true;
        }

        var (normalAmount, plusAmount) = GetBladeMountainPowers(owner.Creature);
        if (normalAmount == 0 && plusAmount == 0)
        {
            _shouldSkipUpgrade = false;
            return true;
        }

        bool fromUpgradedSource = IsFromUpgradedSource();
        bool hasBladeMountainPower = normalAmount > 0 || plusAmount > 0;

        if (fromUpgradedSource && hasBladeMountainPower)
        {
            _shouldSkipUpgrade = true;
        }
        else
        {
            _shouldSkipUpgrade = false;
        }

        var sourceType = FindNearestSource();
        if (sourceType == SourceType.Power)
        {
            __result = CreateGreatBlades(owner, 1, normalAmount, plusAmount);
            return false;
        }

        if (_playersWhoGotGreatBladeFromCardPlay.Contains(owner))
        {
            __result = Task.FromResult<CardModel?>(null);
            return false;
        }
        _playersWhoGotGreatBladeFromCardPlay.Add(owner);

        __result = CreateGreatBlades(owner, 1, normalAmount, plusAmount);
        return false;
    }

    [HarmonyPatch(nameof(Shiv.CreateInHand), typeof(Player), typeof(int), typeof(CombatState))]
    [HarmonyPrefix]
    public static bool PrefixMultiple(Player owner, int count, CombatState combatState, ref Task<IEnumerable<CardModel>> __result)
    {
        if (owner == null || combatState == null)
        {
            _shouldSkipUpgrade = false;
            return true;
        }

        var (normalAmount, plusAmount) = GetBladeMountainPowers(owner.Creature);
        if (normalAmount == 0 && plusAmount == 0)
        {
            _shouldSkipUpgrade = false;
            return true;
        }

        bool fromUpgradedSource = IsFromUpgradedSource();
        bool hasBladeMountainPower = normalAmount > 0 || plusAmount > 0;

        if (fromUpgradedSource && hasBladeMountainPower)
        {
            _shouldSkipUpgrade = true;
        }
        else
        {
            _shouldSkipUpgrade = false;
        }

        __result = CreateGreatBladesMultiple(owner, count, normalAmount, plusAmount);
        return false;
    }

    private static async Task<CardModel?> CreateGreatBlades(Player owner, int shivCount, int normalAmount, int plusAmount)
    {
        var combatState = owner.Creature.CombatState!;
        CardModel? firstBlade = null;

        if (normalAmount > 0)
        {
            var blades = await GreatBlade.CreateInHand(owner, shivCount * normalAmount, combatState);
            foreach (var blade in blades)
            {
                if (firstBlade == null)
                {
                    firstBlade = blade;
                }
            }
        }

        if (plusAmount > 0)
        {
            var blades = await GreatBlade.CreateInHand(owner, shivCount * plusAmount, combatState);
            foreach (var blade in blades)
            {
                blade.UpgradeInternal();
                blade.FinalizeUpgradeInternal();
                if (firstBlade == null)
                {
                    firstBlade = blade;
                }
            }
        }

        return firstBlade;
    }

    private static async Task<IEnumerable<CardModel>> CreateGreatBladesMultiple(Player owner, int shivCount, int normalAmount, int plusAmount)
    {
        var combatState = owner.Creature.CombatState!;
        var result = new List<CardModel>();

        if (normalAmount > 0)
        {
            var blades = await GreatBlade.CreateInHand(owner, shivCount * normalAmount, combatState);
            result.AddRange(blades);
        }

        if (plusAmount > 0)
        {
            var blades = await GreatBlade.CreateInHand(owner, shivCount * plusAmount, combatState);
            foreach (var blade in blades)
            {
                blade.UpgradeInternal();
                blade.FinalizeUpgradeInternal();
            }
            result.AddRange(blades);
        }

        return result;
    }
}

[HarmonyPatch(typeof(CardCmd), "Upgrade", typeof(IEnumerable<CardModel>), typeof(CardPreviewStyle))]
public static class CardCmdUpgradePatch
{
    [HarmonyPrefix]
    public static bool UpgradePrefix(IEnumerable<CardModel> cards)
    {
        if (ShivCreateInHandPatches.ShouldSkipUpgrade)
        {
            foreach (var card in cards)
            {
                if (card is GreatBlade)
                {
                    return false;
                }
            }
        }
        return true;
    }
}

[HarmonyPatch(typeof(Hook))]
public static class HookCardPlayPatch
{
    [HarmonyPatch(nameof(Hook.BeforeCardPlayed))]
    [HarmonyPrefix]
    public static void BeforeCardPlayed(CombatState combatState, CardPlay cardPlay)
    {
        ShivCreateInHandPatches.StartCardPlay();
    }

    [HarmonyPatch(nameof(Hook.AfterCardPlayed))]
    [HarmonyPostfix]
    public static void AfterCardPlayed()
    {
        ShivCreateInHandPatches.EndCardPlay();
    }
}

[HarmonyPatch(typeof(CombatManager), "SetUpCombat")]
public static class ShivCombatPatch
{
    public static void Postfix(CombatState state)
    {
        var instance = CombatManager.Instance;
        if (instance != null)
        {
            instance.CombatEnded += OnCombatEnded;
        }
    }

    private static void OnCombatEnded(CombatRoom room)
    {
        ShivCreateInHandPatches.ClearAll();

        var instance = CombatManager.Instance;
        if (instance != null)
        {
            instance.CombatEnded -= OnCombatEnded;
        }
    }
}
