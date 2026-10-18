namespace Monopoly;

// Базовый класс для всех клеток.
// Игра всегда вызывает OnLand, а что делать - решает сама клетка
public abstract class Square
{
    public string Name { get; }
    public int Position { get; }

    protected Square(string name, int position)
    {
        Name = name;
        Position = position;
    }

    public abstract void OnLand(Player player, Game game);
}
