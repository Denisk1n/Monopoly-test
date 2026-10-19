namespace Monopoly;

// Деньги за Старт начисляются в Player.Move, сама клетка ничего не делает
public class Go : Square
{
    public Go(string name, int position) : base(name, position)
    {
    }

    public override void OnLand(Player player, Game game)
    {
        game.Log("Вы на клетке Старт.");
    }
}
