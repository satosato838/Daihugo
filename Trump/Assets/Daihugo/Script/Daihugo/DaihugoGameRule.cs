
public static class DaihugoGameRule
{
    public enum Number
    {
        Three = 0,
        Four,
        Five,
        Six,
        Seven,
        Eight,
        Nine,
        Ten,
        Jack,
        Queen,
        King,
        Ace,
        Two,
        Joker
    }

    public enum SuitType
    {
        Spade,
        Diamond,
        Clover,
        Heart,
        Joker
    }
    public enum Effect
    {
        None,
        Eight_Enders,
        Eleven_Back,
        Counter_Spade_3
    }
    public enum DaihugoState
    {
        None,
        Revolution,
    }

    public enum GameState
    {
        None,
        GamePlay,
        CardChange,
        Result,
    }
    public enum GameRank
    {
        Heimin = 0,
        DaiHugo,
        Hugo,
        Hinmin,
        DaiHinmin,
    }

    public static int KakumeiPlayCardCount => 4;

    public static SuitType[] SuitTypes => new SuitType[] { SuitType.Spade, SuitType.Diamond, SuitType.Clover, SuitType.Heart };
    public static Number[] Numbers => new Number[] {
        Number.Three,
        Number.Four,
        Number.Five,
        Number.Six,
        Number.Seven,
        Number.Eight,
        Number.Nine,
        Number.Ten,
        Number.Jack,
        Number.Queen,
        Number.King,
        Number.Ace,
        Number.Two,
        Number.Joker
    };

}
