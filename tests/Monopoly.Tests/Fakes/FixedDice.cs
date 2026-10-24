using Monopoly;

namespace Monopoly.Tests.Fakes;

// Кубики, которые выдают заранее заданные значения
public class FixedDice : Dice
{
    private readonly Queue<(int, int)> _rolls = new Queue<(int, int)>();

    public FixedDice(params (int, int)[] rolls)
    {
        foreach (var roll in rolls)
        {
            _rolls.Enqueue(roll);
        }
    }

    public override int Roll()
    {
        if (_rolls.Count > 0)
        {
            (Value1, Value2) = _rolls.Dequeue();
        }

        return GetSum();
    }
}
