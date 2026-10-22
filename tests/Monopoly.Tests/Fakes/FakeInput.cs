using Monopoly;

namespace Monopoly.Tests.Fakes;

// Заготовленные ответы вместо консоли.
// Если ответы закончились: "нет" и 0 (то есть пас на аукционе)
public class FakeInput : IInput
{
    public Queue<bool> YesNoAnswers { get; } = new Queue<bool>();
    public Queue<int> NumberAnswers { get; } = new Queue<int>();

    public FakeInput Yes()
    {
        YesNoAnswers.Enqueue(true);
        return this;
    }

    public FakeInput No()
    {
        YesNoAnswers.Enqueue(false);
        return this;
    }

    public FakeInput Number(int number)
    {
        NumberAnswers.Enqueue(number);
        return this;
    }

    public bool AskYesNo(string question)
    {
        return YesNoAnswers.Count > 0 && YesNoAnswers.Dequeue();
    }

    public int AskNumber(string question)
    {
        return NumberAnswers.Count > 0 ? NumberAnswers.Dequeue() : 0;
    }

    public string AskText(string question)
    {
        return "";
    }
}
