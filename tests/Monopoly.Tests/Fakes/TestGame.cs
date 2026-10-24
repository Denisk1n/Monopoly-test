using Monopoly;

namespace Monopoly.Tests.Fakes;

public static class TestGame
{
    // Игра с двумя игроками: Андрей и Иван
    public static Game Create(FakeInput? input = null, Dice? dice = null)
    {
        var game = new Game(input ?? new FakeInput(), dice ?? new FixedDice());
        game.StartGame(new List<string> { "Андрей", "Иван" });
        return game;
    }

    public static T Square<T>(Game game, int position) where T : Square
    {
        return (T)game.Board.GetSquare(position);
    }
}
