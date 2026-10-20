namespace Monopoly;

public class Deck
{
    private readonly List<Card> _cards;

    public string Name { get; }
    public int Count => _cards.Count;

    public Deck(string name, List<Card> cards)
    {
        Name = name;
        _cards = new List<Card>(cards);
    }

    public Card DrawCard()
    {
        if (_cards.Count == 0)
        {
            throw new InvalidOperationException("Колода пустая");
        }

        Card card = _cards[0];
        _cards.RemoveAt(0);
        return card;
    }

    public void ReturnToBottom(Card card)
    {
        _cards.Add(card);
    }

    public void Shuffle(Random random)
    {
        for (int i = _cards.Count - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            (_cards[i], _cards[j]) = (_cards[j], _cards[i]);
        }
    }

    public static Deck CreateChanceDeck()
    {
        var cards = new List<Card>
        {
            new Card("Штраф за превышение скорости: заплатите 15", EffectType.Fine, amount: 15),
            new Card("Вы выиграли конкурс кроссвордов: получите 100", EffectType.Bonus, amount: 100),
            new Card("Отправляйтесь на Старт", EffectType.MoveTo, targetPosition: 0),
            new Card("Отправляйтесь на ул. Арбат", EffectType.MoveTo, targetPosition: 39),
            new Card("Отправляйтесь в тюрьму", EffectType.GoToJail)
        };
        return new Deck("Шанс", cards);
    }

    public static Deck CreateCommunityChestDeck()
    {
        var cards = new List<Card>
        {
            new Card("Ошибка в расчётах в вашу пользу: получите 200", EffectType.Bonus, amount: 200),
            new Card("Оплата лечения: заплатите 50", EffectType.Fine, amount: 50),
            new Card("Возврат налога: получите 20", EffectType.Bonus, amount: 20),
            new Card("Отправляйтесь на Старт", EffectType.MoveTo, targetPosition: 0),
            new Card("Отправляйтесь в тюрьму", EffectType.GoToJail)
        };
        return new Deck("Общая казна", cards);
    }
}
