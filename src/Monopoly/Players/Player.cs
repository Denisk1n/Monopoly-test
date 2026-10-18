namespace Monopoly;

public class Player
{
    public const int BoardSize = 40;
    public const int GoReward = 200;

    public string Name { get; }
    public int Cash { get; private set; }
    public int Position { get; private set; }

    public Player(string name, int cash)
    {
        Name = name;
        Cash = cash;
    }

    // Двигаем игрока вперёд. Возвращает true, если он прошёл через Старт
    public bool Move(int steps)
    {
        int newPosition = Position + steps;
        bool passedGo = newPosition >= BoardSize;

        if (passedGo)
        {
            Cash += GoReward;
        }

        Position = newPosition % BoardSize;
        return passedGo;
    }

    // Переносим игрока на конкретную клетку (для карточек).
    // Если клетка "позади", значит игрок прошёл круг и получает деньги за Старт
    public bool MoveTo(int position)
    {
        bool passedGo = position < Position;

        if (passedGo)
        {
            Cash += GoReward;
        }

        Position = position;
        return passedGo;
    }

    public void Pay(int amount)
    {
        Cash -= amount;
    }

    public void Receive(int amount)
    {
        Cash += amount;
    }

    public bool CanAfford(int amount)
    {
        return Cash >= amount;
    }

    public override string ToString()
    {
        return $"{Name} (деньги: {Cash}, клетка: {Position})";
    }
}
