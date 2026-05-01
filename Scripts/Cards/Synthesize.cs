using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using USCE.Scripts.Patches;

namespace USCE.Scripts.Cards;

[Pool(typeof(SilentCardPool))]
public class Synthesize : SilentCardModel, ILocalizationProvider
{
    private const int energyCost = 2;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Rare;
    private const TargetType targetType = TargetType.Self;

    public override IEnumerable<CardKeyword> CanonicalKeywords => new List<CardKeyword> { CardKeyword.Exhaust };

    public override List<(string, string)>? Localization => LocManager.Instance.Language switch
    {
        "zhs" => new CardLoc("化合", "选择你[gold]手牌[/gold]中的一张攻击牌，将其变为耗能X以打出X次。"),
        _ => new CardLoc("Synthesize", "Choose an Attack in your [gold]hand[/gold], it costs X and is played X times.")
    };

    public Synthesize() : base(energyCost, type, rarity, targetType)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        GD.PrintRich("[color=cyan][USCE Synthesize] ========== OnPlay started ==========[/color]");
        
        CardSelectorPrefs prefs = new CardSelectorPrefs(SelectionScreenPrompt, 1) with { PretendCardsCanBePlayed = true };
        
        var result = await CardSelectCmd.FromHand(choiceContext, Owner, prefs, (CardModel c) => 
        {
            bool isAttack = c.Type == CardType.Attack;
            bool isPlayable = !c.Keywords.Contains(CardKeyword.Unplayable);
            return isAttack && isPlayable;
        }, this);
        
        CardModel card = result.FirstOrDefault();
        
        GD.PrintRich($"[color=cyan][USCE Synthesize] Selected card: {card?.Title ?? "null"}[/color]");

        if (card != null)
        {
            GD.PrintRich($"[color=cyan][USCE Synthesize] Setting card to X-cost...[/color]");
            
            card.EnergyCost.SetCostsX(true);
            CardModelPatch.MarkAsSynthesized(card);
            
            GD.PrintRich($"[color=cyan][USCE Synthesize] ========== Done! ==========[/color]");
        }
        else
        {
            GD.PrintRich($"[color=red][USCE Synthesize] No card selected! ==========[/color]");
        }
    }

    protected override void OnUpgrade()
    {
        EnergyCost.SetCustomBaseCost(1);
    }
}
