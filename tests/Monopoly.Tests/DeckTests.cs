using Monopoly;

namespace Monopoly.Tests;

public class DeckTests
{
    private static Deck CreateDeck()
    {
        return new Deck("Тест", new List<Card>
        {
            new Card("Первая", EffectType.Bonus, amount: 10),
            new Card("Вторая", EffectType.Fine, amount: 20),
            new Card("Третья", EffectType.GoToJail)
        });
    }

    [Fact]
    public void DrawCard_ReturnsTopCard()
    {
        var deck = CreateDeck();

        Card card = deck.DrawCard();

        Assert.Equal("Первая", card.Description);
        Assert.Equal(2, deck.Count);
    }

    [Fact]
    public void ReturnToBottom_CardGoesLast()
    {
        var deck = CreateDeck();
        Card first = deck.DrawCard();

        deck.ReturnToBottom(first);
        deck.DrawCard();
        deck.DrawCard();

        Assert.Same(first, deck.DrawCard());
    }

    [Fact]
    public void DrawCard_EmptyDeck_Throws()
    {
        var deck = new Deck("Пустая", new List<Card>());

        Assert.Throws<InvalidOperationException>(() => deck.DrawCard());
    }

    [Fact]
    public void Shuffle_KeepsAllCards()
    {
        var deck = CreateDeck();

        deck.Shuffle(new Random(1));

        var names = new List<string>();
        while (deck.Count > 0)
        {
            names.Add(deck.DrawCard().Description);
        }
        Assert.Equal(3, names.Count);
        Assert.Contains("Первая", names);
        Assert.Contains("Вторая", names);
        Assert.Contains("Третья", names);
    }

    [Fact]
    public void StandardDecks_HaveFiveCards()
    {
        Assert.Equal(5, Deck.CreateChanceDeck().Count);
        Assert.Equal(5, Deck.CreateCommunityChestDeck().Count);
        Assert.Equal("Шанс", Deck.CreateChanceDeck().Name);
    }
}
