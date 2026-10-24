namespace Monopoly;

public class FreeParking : Square
{
    public FreeParking(string name, int position) : base(name, position)
    {
    }

    public override void OnLand(Player player, Game game)
    {
        game.Log("Бесплатная стоянка, можно отдохнуть.");
    }
}
