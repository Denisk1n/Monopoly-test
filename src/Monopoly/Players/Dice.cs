namespace Monopoly;

public class Dice
{
    private readonly Random _random;

    public int Value1 { get; protected set; }
    public int Value2 { get; protected set; }

    public Dice() : this(new Random())
    {
    }

    public Dice(Random random)
    {
        _random = random;
    }

    // virtual, чтобы в тестах можно было подставить кубики с нужными значениями
    public virtual int Roll()
    {
        Value1 = _random.Next(1, 7);
        Value2 = _random.Next(1, 7);
        return GetSum();
    }

    public int GetSum()
    {
        return Value1 + Value2;
    }
}
