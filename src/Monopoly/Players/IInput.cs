namespace Monopoly;

// Через этот интерфейс игра спрашивает что-то у игрока.
// В программе это консоль, а в тестах - заготовленные ответы
public interface IInput
{
    bool AskYesNo(string question);
    int AskNumber(string question);
    string AskText(string question);
}
