namespace Monopoly;

// Общая логика для улиц, железных дорог и коммуналки.
// Подклассы меняют только расчёт аренды
public abstract class Property : Square
{
    public int Price { get; }
    public Player? Owner { get; private set; }

    protected Property(string name, int position, int price) : base(name, position)
    {
        Price = price;
    }

    public bool IsFree()
    {
        return Owner == null;
    }

    public Player? GetOwner()
    {
        return Owner;
    }

    public void SetOwner(Player? player)
    {
        Owner = player;
    }

    public override void OnLand(Player player, Game game)
    {
        if (Owner == null)
        {
            game.OfferPurchase(player, this);
            return;
        }

        if (Owner == player)
        {
            game.Log("Это ваша собственность, ничего не происходит.");
            return;
        }

        int rent = CalculateRent(game);
        game.Log($"{player.Name} платит аренду {rent} игроку {Owner.Name}");
        game.ChargePayment(player, Owner, rent);
    }

    public abstract int CalculateRent(Game game);
}
