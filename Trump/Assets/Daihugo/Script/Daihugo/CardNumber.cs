
public class CardNumber
{
    public Daihugo.Number Number;
    public Daihugo.Effect Effect;
    public CardNumber(int number)
    {
        switch (number)
        {
            case 1:
                Number = Daihugo.Number.Ace;
                Effect = Daihugo.Effect.None;
                break;
            case 2:
                Number = Daihugo.Number.Two;
                Effect = Daihugo.Effect.None;
                break;
            case 3:
                Number = Daihugo.Number.Three;
                Effect = Daihugo.Effect.None;
                break;
            case 4:
                Number = Daihugo.Number.Four;
                Effect = Daihugo.Effect.None;
                break;
            case 5:
                Number = Daihugo.Number.Five;
                Effect = Daihugo.Effect.None;
                break;
            case 6:
                Number = Daihugo.Number.Six;
                Effect = Daihugo.Effect.None;
                break;
            case 7:
                Number = Daihugo.Number.Seven;
                Effect = Daihugo.Effect.None;
                break;
            case 8:
                Number = Daihugo.Number.Eight;
                Effect = Daihugo.Effect.Eight_Enders;
                break;
            case 9:
                Number = Daihugo.Number.Nine;
                Effect = Daihugo.Effect.None;
                break;
            case 10:
                Number = Daihugo.Number.Ten;
                Effect = Daihugo.Effect.None;
                break;
            case 11:
                Number = Daihugo.Number.Jack;
                Effect = Daihugo.Effect.Eleven_Back;
                break;
            case 12:
                Number = Daihugo.Number.Queen;
                Effect = Daihugo.Effect.None;
                break;
            case 13:
                Number = Daihugo.Number.King;
                Effect = Daihugo.Effect.None;
                break;
            case 14:
                Number = Daihugo.Number.Joker;
                Effect = Daihugo.Effect.None;
                break;
        }

    }

    public string NumberName()
    {
        return Number switch
        {
            Daihugo.Number.Ace => "01",
            Daihugo.Number.Two => "02",
            Daihugo.Number.Three => "03",
            Daihugo.Number.Four => "04",
            Daihugo.Number.Five => "05",
            Daihugo.Number.Six => "06",
            Daihugo.Number.Seven => "07",
            Daihugo.Number.Eight => "08",
            Daihugo.Number.Nine => "09",
            Daihugo.Number.Ten => "10",
            Daihugo.Number.Jack => "11",
            Daihugo.Number.Queen => "12",
            Daihugo.Number.King => "13",
            _ => ""
        };
    }
}
