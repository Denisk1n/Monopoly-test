namespace Monopoly;

public class Tax : Square
{
    public int Amount { get; }

    public Tax(string name, int position, int amount) : base(name, position)
    {
        Amount = amount;
    }

    // Банка нет, поэтому налог просто выводится из игры (получателя нет)
    public override void OnLand(Player player, Game game)
    {
        game.Log($"{player.Name} платит налог {Amount}");
        game.ChargePayment(player, null, Amount);
    }
}
