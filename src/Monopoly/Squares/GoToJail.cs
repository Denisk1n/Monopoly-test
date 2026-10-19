namespace Monopoly;

public class GoToJail : Square
{
    public GoToJail(string name, int position) : base(name, position)
    {
    }

    public override void OnLand(Player player, Game game)
    {
        game.Log($"{player.Name} отправляется в тюрьму!");
        player.SendToJail();
    }
}
