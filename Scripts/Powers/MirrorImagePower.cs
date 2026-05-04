using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace USCE.Scripts.Powers;

public class MirrorImagePower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override string? CustomPackedIconPath => "res://UltimateSilentCardExpansion/images/powers/usce_mirror_image_power.png";
    public override string? CustomBigIconPath => "res://UltimateSilentCardExpansion/images/powers/usce_mirror_image_power.png";

    private static readonly HashSet<CardModel> _cardsPlayedByMirrorImage = new();

    public static bool WasPlayedByMirrorImage(CardModel card) => _cardsPlayedByMirrorImage.Contains(card);

    public static void MarkAsPlayedByMirrorImage(CardModel card) => _cardsPlayedByMirrorImage.Add(card);

    public static void ClearAll() => _cardsPlayedByMirrorImage.Clear();

    public override List<(string, string)>? Localization => LocManager.Instance.Language switch
    {
        "zhs" => new PowerLoc("镜中倒影", "每当你打出攻击牌时，随机打出你手牌中的另外1张攻击牌。", "每当你打出攻击牌时，随机打出你[gold]手牌[/gold]中的另外[blue]{Amount}[/blue]张攻击牌。"),
        _ => new PowerLoc("Mirror Image", "Whenever you play an Attack, play 1 other random Attack from your hand.", "Whenever you play an Attack, play [blue]{Amount}[/blue] other random Attack(s) from your [gold]hand[/gold].")
    };

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature != Owner)
            return;

        if (cardPlay.Card.Type != CardType.Attack)
            return;

        if (WasPlayedByMirrorImage(cardPlay.Card))
            return;

        var hand = PileType.Hand.GetPile(Owner.Player!).Cards;
        var otherAttacks = hand
            .Where(c => c.Type == CardType.Attack && c != cardPlay.Card && !WasPlayedByMirrorImage(c))
            .ToList();

        if (otherAttacks.Count == 0)
            return;

        int attacksToPlay = (int)Amount;
        if (attacksToPlay > otherAttacks.Count)
            attacksToPlay = otherAttacks.Count;

        Flash();

        for (int i = 0; i < attacksToPlay; i++)
        {
            var randomAttack = Owner.Player!.RunState.Rng.CombatTargets.NextItem(otherAttacks);
            if (randomAttack != null)
            {
                MarkAsPlayedByMirrorImage(randomAttack);
                await CardCmd.AutoPlay(context, randomAttack, null);
                otherAttacks.Remove(randomAttack);
            }
        }
    }
}
