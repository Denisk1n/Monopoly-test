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
}
