namespace Monopoly;

// Заготовка игры, чтобы клетки и карточки уже могли с ней работать.
// Полностью класс пишется в части 4
public class Game
{
    public Board Board { get; }
    public Dice Dice { get; }

    public Game(Board board, Dice dice)
    {
        Board = board;
        Dice = dice;
    }

    // TODO: покупка или аукцион (часть 4)
    public void OfferPurchase(Player player, Property property)
    {
    }

    // TODO: оплата и банкротство (часть 4)
    public void ChargePayment(Player payer, Player? receiver, int amount)
    {
    }

    public void Log(string message)
    {
        Console.WriteLine(message);
    }
}
