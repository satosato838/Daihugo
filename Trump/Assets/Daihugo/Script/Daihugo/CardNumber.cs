
using System;

public class CardNumber
{
    public DaihugoGameRule.Number Number;
    public DaihugoGameRule.Effect Effect;
    public CardNumber(int number)
    {
        switch (number)
        {
            case 1:
                Number = DaihugoGameRule.Number.Ace;
                Effect = DaihugoGameRule.Effect.None;
                break;
            case 2:
                Number = DaihugoGameRule.Number.Two;
                Effect = DaihugoGameRule.Effect.None;
                break;
            case 3:
                Number = DaihugoGameRule.Number.Three;
                Effect = DaihugoGameRule.Effect.None;
                break;
            case 4:
                Number = DaihugoGameRule.Number.Four;
                Effect = DaihugoGameRule.Effect.None;
                break;
            case 5:
                Number = DaihugoGameRule.Number.Five;
                Effect = DaihugoGameRule.Effect.None;
                break;
            case 6:
                Number = DaihugoGameRule.Number.Six;
                Effect = DaihugoGameRule.Effect.None;
                break;
            case 7:
                Number = DaihugoGameRule.Number.Seven;
                Effect = DaihugoGameRule.Effect.None;
                break;
            case 8:
                Number = DaihugoGameRule.Number.Eight;
                Effect = DaihugoGameRule.Effect.Eight_Enders;
                break;
            case 9:
                Number = DaihugoGameRule.Number.Nine;
                Effect = DaihugoGameRule.Effect.None;
                break;
            case 10:
                Number = DaihugoGameRule.Number.Ten;
                Effect = DaihugoGameRule.Effect.None;
                break;
            case 11:
                Number = DaihugoGameRule.Number.Jack;
                Effect = DaihugoGameRule.Effect.Eleven_Back;
                break;
            case 12:
                Number = DaihugoGameRule.Number.Queen;
                Effect = DaihugoGameRule.Effect.None;
                break;
            case 13:
                Number = DaihugoGameRule.Number.King;
                Effect = DaihugoGameRule.Effect.None;
                break;
            case 14:
                Number = DaihugoGameRule.Number.Joker;
                Effect = DaihugoGameRule.Effect.None;
                break;
        }

    }
    public CardNumber(DaihugoGameRule.Number number)
    {
        switch (number)
        {
            case DaihugoGameRule.Number.Ace:
                Number = DaihugoGameRule.Number.Ace;
                Effect = DaihugoGameRule.Effect.None;
                break;
            case DaihugoGameRule.Number.Two:
                Number = DaihugoGameRule.Number.Two;
                Effect = DaihugoGameRule.Effect.None;
                break;
            case DaihugoGameRule.Number.Three:
                Number = DaihugoGameRule.Number.Three;
                Effect = DaihugoGameRule.Effect.None;
                break;
            case DaihugoGameRule.Number.Four:
                Number = DaihugoGameRule.Number.Four;
                Effect = DaihugoGameRule.Effect.None;
                break;
            case DaihugoGameRule.Number.Five:
                Number = DaihugoGameRule.Number.Five;
                Effect = DaihugoGameRule.Effect.None;
                break;
            case DaihugoGameRule.Number.Six:
                Number = DaihugoGameRule.Number.Six;
                Effect = DaihugoGameRule.Effect.None;
                break;
            case DaihugoGameRule.Number.Seven:
                Number = DaihugoGameRule.Number.Seven;
                Effect = DaihugoGameRule.Effect.None;
                break;
            case DaihugoGameRule.Number.Eight:
                Number = DaihugoGameRule.Number.Eight;
                Effect = DaihugoGameRule.Effect.Eight_Enders;
                break;
            case DaihugoGameRule.Number.Nine:
                Number = DaihugoGameRule.Number.Nine;
                Effect = DaihugoGameRule.Effect.None;
                break;
            case DaihugoGameRule.Number.Ten:
                Number = DaihugoGameRule.Number.Ten;
                Effect = DaihugoGameRule.Effect.None;
                break;
            case DaihugoGameRule.Number.Jack:
                Number = DaihugoGameRule.Number.Jack;
                Effect = DaihugoGameRule.Effect.Eleven_Back;
                break;
            case DaihugoGameRule.Number.Queen:
                Number = DaihugoGameRule.Number.Queen;
                Effect = DaihugoGameRule.Effect.None;
                break;
            case DaihugoGameRule.Number.King:
                Number = DaihugoGameRule.Number.King;
                Effect = DaihugoGameRule.Effect.None;
                break;
            case DaihugoGameRule.Number.Joker:
                Number = DaihugoGameRule.Number.Joker;
                Effect = DaihugoGameRule.Effect.None;
                break;
        }

    }

    public string NumberName()
    {
        return Number switch
        {
            DaihugoGameRule.Number.Ace => "01",
            DaihugoGameRule.Number.Two => "02",
            DaihugoGameRule.Number.Three => "03",
            DaihugoGameRule.Number.Four => "04",
            DaihugoGameRule.Number.Five => "05",
            DaihugoGameRule.Number.Six => "06",
            DaihugoGameRule.Number.Seven => "07",
            DaihugoGameRule.Number.Eight => "08",
            DaihugoGameRule.Number.Nine => "09",
            DaihugoGameRule.Number.Ten => "10",
            DaihugoGameRule.Number.Jack => "11",
            DaihugoGameRule.Number.Queen => "12",
            DaihugoGameRule.Number.King => "13",
            DaihugoGameRule.Number.Joker => "",
            _ => throw new Exception("Not found Number Name")
        };
    }
}
