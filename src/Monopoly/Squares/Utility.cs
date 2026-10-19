namespace Monopoly;

public class Utility : Property
{
    public Utility(string name, int position, int price) : base(name, position, price)
    {
    }

    // сумма кубиков x4, если у владельца одно предприятие, x10 - если оба
    public override int CalculateRent(Game game)
    {
        if (Owner == null)
        {
            return 0;
        }

        int count = game.Board.CountUtilitiesOf(Owner);
        int multiplier = count >= 2 ? 10 : 4;
        return game.Dice.GetSum() * multiplier;
    }
}
