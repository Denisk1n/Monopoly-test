using System.Diagnostics.CodeAnalysis;

namespace Monopoly;

[ExcludeFromCodeCoverage]
public class ConsoleInput : IInput
{
    public bool AskYesNo(string question)
    {
        while (true)
        {
            Console.Write(question + " (д/н): ");
            string? line = Console.ReadLine();
            if (line == null)
            {
                return false; // ввод закончился
            }

            string answer = line.Trim().ToLower();

            if (answer == "д" || answer == "y" || answer == "да")
            {
                return true;
            }

            if (answer == "н" || answer == "n" || answer == "нет")
            {
                return false;
            }

            Console.WriteLine("Введите д или н");
        }
    }

    public int AskNumber(string question)
    {
        while (true)
        {
            Console.Write(question);
            string? answer = Console.ReadLine();
            if (answer == null)
            {
                return 0; // ввод закончился
            }

            if (int.TryParse(answer, out int number))
            {
                return number;
            }

            Console.WriteLine("Нужно ввести число");
        }
    }

    public string AskText(string question)
    {
        Console.Write(question);
        return (Console.ReadLine() ?? "").Trim();
    }
}
