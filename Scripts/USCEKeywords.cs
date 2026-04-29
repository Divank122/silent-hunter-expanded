using BaseLib.Patches.Content;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace USCE.Scripts;

public class USCEKeywords
{
    [CustomEnum("Thirsty")] [KeywordProperties(AutoKeywordPosition.After)]
    public static CardKeyword Thirsty;
}
