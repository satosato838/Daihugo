public class TrumpCard
{
    public DaihugoGameRule.SuitType Suit;
    public CardNumber cardNumber;

    public string CardName;
    public bool IsFront;
    public TrumpCard(DaihugoGameRule.SuitType suit, CardNumber number)
    {
        Suit = suit;
        cardNumber = number;
        if (suit == DaihugoGameRule.SuitType.Joker)
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
}