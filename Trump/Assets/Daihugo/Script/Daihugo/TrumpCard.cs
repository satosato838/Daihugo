public class TrumpCard
{
    private DaihugoGameRule.SuitType suitType;
    public DaihugoGameRule.SuitType Suit => suitType;
    private CardNumber cardNumber;
    public DaihugoGameRule.Number Number => cardNumber.Number;
    public string CardName;
    public bool IsSelect;
    public bool IsSelectable;
    public TrumpCard(DaihugoGameRule.SuitType suit, CardNumber number)
    {
        suitType = suit;
        RefreshIsSelectable(true);
        cardNumber = number;
        if (suitType == DaihugoGameRule.SuitType.Joker)
        {
            CardName = "Joker";
        }
        else
        {
            CardName = Suit switch
            {
                DaihugoGameRule.SuitType.Spade => "S",
                DaihugoGameRule.SuitType.Diamond => "D",
                DaihugoGameRule.SuitType.Clover => "C",
                DaihugoGameRule.SuitType.Heart => "H",
                _ => throw new System.Exception("Not found Suit Type")
            } + number.NumberName();
        }

        if (suit == DaihugoGameRule.SuitType.Spade && cardNumber.Number == DaihugoGameRule.Number.Three)
        {
            cardNumber.Effect = DaihugoGameRule.Effect.Counter_Spade_3;
        }
    }

    public void RefreshIsSelectable(bool val)
    {
        IsSelectable = val;
    }
}