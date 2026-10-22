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
            Console.WriteLine("1 - бросить кубики, 2 - построить дом, 3 - мои владения, 4 - все игроки, 0 - выход");

            int choice = input.AskNumber("> ");
            switch (choice)
            {
                case 1:
                    game.PlayTurn();
                    break;
                case 2:
                    BuildMenu(game, player, input);
                    break;
                case 3:
                    ShowProperties(game, player);
                    break;
                case 4:
                    ShowPlayers(game);
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

    private static void BuildMenu(Game game, Player player, IInput input)
    {
        List<Street> streets = game.Board.GetPropertiesOf(player)
            .OfType<Street>()
            .Where(s => s.CanBuild(player, game.Board))
            .ToList();

        if (streets.Count == 0)
        {
            Console.WriteLine("Строить пока негде: нужна вся цветовая группа и деньги на дом.");
            return;
        }

        foreach (Street street in streets)
        {
            Console.WriteLine($"{street.Position}. {street.Name} (домов: {street.HouseCount}, цена дома: {street.HouseCost})");
        }

        int position = input.AskNumber("Номер клетки: ");
        Street? chosen = streets.FirstOrDefault(s => s.Position == position);

        if (chosen == null || !game.BuildHouse(player, chosen))
        {
            Console.WriteLine("Построить не получилось");
        }
    }

    private static void ShowProperties(Game game, Player player)
    {
        List<Property> properties = game.Board.GetPropertiesOf(player);

        if (properties.Count == 0)
        {
            Console.WriteLine("У вас пока ничего нет.");
            return;
        }

        foreach (Property property in properties)
        {
            string extra = "";
            if (property is Street street)
            {
                extra = street.HasHotel ? ", отель" : $", домов: {street.HouseCount}";
            }
            Console.WriteLine($"{property.Position}. {property.Name}{extra}");
        }
    }

    private static void ShowPlayers(Game game)
    {
        foreach (Player p in game.Players)
        {
            string status = p.Bankrupt ? " - банкрот" : p.InJail ? " - в тюрьме" : "";
            Console.WriteLine($"{p}{status}");
        }
    }
}
