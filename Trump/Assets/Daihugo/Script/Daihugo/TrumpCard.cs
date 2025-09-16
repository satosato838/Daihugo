public class TrumpCard
{
    public Daihugo.SuitType Suit;
    public CardNumber cardNumber;

    public string CardName;
    public bool IsFront;
    public TrumpCard(Daihugo.SuitType suit, CardNumber number)
    {
        Suit = suit;
        cardNumber = number;
        if (suit == Daihugo.SuitType.Joker)
        {
            CardName = "Joker";
        }
        else
        {
            CardName = Suit switch
            {
                Daihugo.SuitType.Spade => "S",
                Daihugo.SuitType.Diamond => "D",
                Daihugo.SuitType.Clover => "C",
                Daihugo.SuitType.Heart => "H",
                _ => ""
            } + number.NumberName();
        }

        if (suit == Daihugo.SuitType.Spade && cardNumber.Number == Daihugo.Number.Three)
        {
            cardNumber.Effect = Daihugo.Effect.Counter_Spade_3;
        }
    }
}