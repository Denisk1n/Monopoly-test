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
}
