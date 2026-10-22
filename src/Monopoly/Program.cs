using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Monopoly;

[ExcludeFromCodeCoverage]
public static class Program
{
    public static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.InputEncoding = Encoding.UTF8;

        var input = new ConsoleInput();
        Console.WriteLine("=== МОНОПОЛИЯ ===");

        int count = input.AskNumber($"Количество игроков ({Game.MinPlayers}-{Game.MaxPlayers}): ");
        while (count < Game.MinPlayers || count > Game.MaxPlayers)
        {
            count = input.AskNumber($"Нужно от {Game.MinPlayers} до {Game.MaxPlayers}: ");
        }

        var names = new List<string>();
        for (int i = 1; i <= count; i++)
        {
            string name = input.AskText($"Имя игрока {i}: ");
            names.Add(name == "" ? $"Игрок {i}" : name);
        }

        var game = new Game(input, new Dice(), new Random());
        game.StartGame(names);

        while (game.Status == GameStatus.InProgress)
        {
            Player player = game.GetCurrentPlayer();
            Console.WriteLine();
            Console.WriteLine($"Ходит {player}");
            Console.WriteLine("1 - бросить кубики, 0 - выход");

            int choice = input.AskNumber("> ");
            switch (choice)
            {
                case 1:
                    game.PlayTurn();
                    break;
                case 0:
                    Console.WriteLine("Игра прервана.");
                    return;
                default:
                    Console.WriteLine("Нет такого пункта");
                    break;
            }
        }

        Console.WriteLine();
        Console.WriteLine($"Игра окончена! Победитель: {game.GetWinner()?.Name}");
    }
}
